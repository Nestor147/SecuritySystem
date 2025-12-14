using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SecuritySystem.Application.Exceptions;
using SecuritySystem.Application.Helpers.Authentication;
using SecuritySystem.Application.Interfaces.Authentication;
using SecuritySystem.Application.Interfaces.Authentication.Dtos;
using SecuritySystem.Core.Entities;
using SecuritySystem.Core.Entities.SealedAuthentication;
using SecuritySystem.Core.Interfaces.Core;
using System.Security.Claims;
using System.Text.Json;

namespace SecuritySystem.Application.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IAppSigningKeyProvider _appSigningKeyProvider;
        private readonly IAppTokenPolicyProvider _appTokenPolicyProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _cfg;
        private readonly IPasswordHasher _hasher;
        private readonly IHttpContextAccessor _http;
        private readonly IJwtIssuer _jwtIssuer;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher hasher,
            IConfiguration cfg,
            IHttpContextAccessor http,
            IAppSigningKeyProvider appSigningKeyProvider,
            IAppTokenPolicyProvider appTokenPolicyProvider,
            IJwtIssuer jwtIssuer
        )
        {
            _unitOfWork = unitOfWork;
            _hasher = hasher;
            _cfg = cfg;
            _http = http;
            _appSigningKeyProvider = appSigningKeyProvider;
            _appTokenPolicyProvider = appTokenPolicyProvider;
            _jwtIssuer = jwtIssuer;
        }

        public async Task<LoginResult> LoginAsync(LoginRequest req, CancellationToken ct)
        {
            try
            {
                var user = await GetUserByCredentialsAsync(req, ct)
                    ?? throw new InvalidCredentialsException("Invalid username or password.");

                var app = await _unitOfWork.ApplicationRepository
                    .FirstOrDefaultAsync(a => a.Id == req.ApplicationId && a.RecordStatus == 1, ct)
                    ?? throw new AppNotFoundException($"Application {req.ApplicationId} does not exist or is inactive.");

                await EnsureUserHasAccessToAppAsync(user.Id, app.Id, ct);

                // 1) Signing key + policy
                var keyMaterial = await _appSigningKeyProvider.GetActiveSigningKeyAsync(app.Id, ct);
                ValidateSigningKeyMaterial(keyMaterial, app.Id);

                var policy = await _appTokenPolicyProvider.GetPolicyAsync(app.Id, ct);

                // 2) Roles + claims
                var roleNames = await GetUserAppRolesAsync(user.Id, app.Id, ct);
                var jti = Guid.NewGuid().ToString("N");

                var descriptor = new TokenDescriptor(
                    subject: user.Id.ToString(),
                    issuer: $"Authentication.API/{app.Code}",
                    audience: $"Authentication.Clients/{app.Code}",
                    jti: jti,
                    lifetime: TimeSpan.FromMinutes(policy.AccessTokenMinutes),
                    roles: roleNames,
                    claims: new Dictionary<string, object>
                    {
                        ["username"] = user.Username,
                        ["email"] = user.Email ?? string.Empty,
                        ["app_code"] = app.Code
                    },
                    extraClaims: await BuildAccessClaimsAsync(user, app, roleNames, jti, ct)
                );

                // 3) Create access token
                var accessToken = _jwtIssuer.CreateAccessToken(descriptor, keyMaterial);

                // 4) Create refresh token (opaque) + persist hash
                var opaque = RefreshTokenHasher.GenerateOpaque();
                var hash = RefreshTokenHasher.Hash(opaque);

                await _unitOfWork.RefreshTokenRepository.InsertAsync(new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    ApplicationId = app.Id,
                    TokenHash = hash,
                    ExpiresAt = DateTime.UtcNow.AddDays(policy.RefreshTokenDays),
                    TokenCreatedAt = DateTime.UtcNow,
                    IPAddress = GetClientIp(),
                    UserAgent = GetUserAgent(),
                    Used = false,
                    Revoked = false
                });

                await SaveChangesWithRetryAsync(ct);

                // 5) Audit is NON-blocking
                _ = RegisterLoginAttemptSafeAsync(req.Username, user.Id, true, "Login ok", ct);

                return new LoginResult
                {
                    AccessToken = accessToken,
                    RefreshToken = opaque
                };
            }
            catch
            {
                // Do not hide exceptions here; let controller handle the message
                throw;
            }
        }

        public async Task<LoginResult> RefreshAsync(RefreshRequest req, CancellationToken ct)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(req.RefreshToken))
                    throw new InvalidCredentialsException("Refresh token is required.");

                var hash = RefreshTokenHasher.Hash(req.RefreshToken);

                // 1) Find refresh token
                var tokenEntity = await _unitOfWork.RefreshTokenRepository
                    .FirstOrDefaultAsync(t => t.TokenHash == hash, ct);

                if (tokenEntity == null || tokenEntity.ExpiresAt <= DateTime.UtcNow || tokenEntity.Revoked || tokenEntity.Used)
                    throw new InvalidCredentialsException("Refresh token is invalid or expired.");

                // 2) Load user
                var userEntity = await _unitOfWork.UserRepository
                    .FirstOrDefaultAsync(u => u.Id == tokenEntity.UserId && u.RecordStatus == 1, ct)
                    ?? throw new InvalidCredentialsException("User not found or inactive.");

                var user = new AuthUser
                {
                    Id = userEntity.Id,
                    Username = userEntity.Username,
                    Email = userEntity.Email,
                    PasswordHash = userEntity.PasswordHash,
                    RecordStatus = userEntity.RecordStatus
                };

                // 3) Load app
                var app = await _unitOfWork.ApplicationRepository
                    .FirstOrDefaultAsync(a => a.Id == tokenEntity.ApplicationId && a.RecordStatus == 1, ct)
                    ?? throw new AppNotFoundException($"Application {tokenEntity.ApplicationId} does not exist or is inactive.");

                // 4) Key + policy
                var keyMaterial = await _appSigningKeyProvider.GetActiveSigningKeyAsync(app.Id, ct);
                ValidateSigningKeyMaterial(keyMaterial, app.Id);

                var policy = await _appTokenPolicyProvider.GetPolicyAsync(app.Id, ct);

                var jti = Guid.NewGuid().ToString("N");
                var roleNames = await GetUserAppRolesAsync(user.Id, app.Id, ct);

                var descriptor = new TokenDescriptor(
                    subject: user.Id.ToString(),
                    issuer: $"Authentication.API/{app.Code}",
                    audience: $"Authentication.Clients/{app.Code}",
                    jti: jti,
                    lifetime: TimeSpan.FromMinutes(policy.AccessTokenMinutes),
                    roles: roleNames,
                    claims: new Dictionary<string, object>
                    {
                        ["username"] = user.Username,
                        ["email"] = user.Email ?? string.Empty,
                        ["app_code"] = app.Code
                    },
                    extraClaims: await BuildAccessClaimsAsync(user, app, roleNames, jti, ct)
                );

                var newAccessToken = _jwtIssuer.CreateAccessToken(descriptor, keyMaterial);

                // 5) Rotate refresh token (secure)
                var newOpaque = RefreshTokenHasher.GenerateOpaque();
                var newHash = RefreshTokenHasher.Hash(newOpaque);

                tokenEntity.TokenHash = newHash;
                tokenEntity.TokenCreatedAt = DateTime.UtcNow;
                tokenEntity.ExpiresAt = DateTime.UtcNow.AddDays(policy.RefreshTokenDays);
                tokenEntity.IPAddress = GetClientIp();
                tokenEntity.UserAgent = GetUserAgent();
                tokenEntity.Used = false;
                tokenEntity.Revoked = false;

                _unitOfWork.RefreshTokenRepository.Update(tokenEntity);

                await SaveChangesWithRetryAsync(ct);

                return new LoginResult
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newOpaque
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task LogoutAsync(LogoutRequest req, CancellationToken ct)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(req.RefreshToken))
                    return; // idempotent

                var hash = RefreshTokenHasher.Hash(req.RefreshToken);

                var tokenEntity = await _unitOfWork.RefreshTokenRepository
                    .FirstOrDefaultAsync(t => t.TokenHash == hash, ct);

                if (tokenEntity == null)
                    return;

                tokenEntity.Revoked = true;
                tokenEntity.Used = true;

                _unitOfWork.RefreshTokenRepository.Update(tokenEntity);

                await SaveChangesWithRetryAsync(ct);
            }
            catch
            {
                throw;
            }
        }

        #region Private helpers

        private sealed class AuthUser
        {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string PasswordHash { get; set; } = string.Empty;
            public int RecordStatus { get; set; }
        }

        private async Task<AuthUser?> GetUserByCredentialsAsync(LoginRequest req, CancellationToken ct)
        {
            var entity = await _unitOfWork.UserRepository
                .FirstOrDefaultAsync(u =>
                    u.RecordStatus == 1 &&
                    (u.Username == req.Username || u.Email == req.Username),
                    ct);

            if (entity == null)
                return null;

            if (!_hasher.Verify(req.Password, entity.PasswordHash))
                return null;

            return new AuthUser
            {
                Id = entity.Id,
                Username = entity.Username,
                Email = entity.Email,
                PasswordHash = entity.PasswordHash,
                RecordStatus = entity.RecordStatus
            };
        }

        private Task<List<Claim>> BuildAccessClaimsAsync(
            AuthUser user,
            Applications app,
            List<string> roleNames,
            string jti,
            CancellationToken ct)
        {
            var claims = new List<Claim>();

            foreach (var role in roleNames)
                claims.Add(new Claim(ClaimTypes.Role, role));

            claims.Add(new Claim(
                "roles",
                JsonSerializer.Serialize(roleNames),
                Microsoft.IdentityModel.JsonWebTokens.JsonClaimValueTypes.Json));

            claims.Add(new Claim("username", user.Username));

            if (!string.IsNullOrWhiteSpace(user.Email))
                claims.Add(new Claim("email", user.Email));

            // Optional but useful:
            claims.Add(new Claim("app_code", app.Code));

            return Task.FromResult(claims);
        }

        private Task EnsureUserHasAccessToAppAsync(int userId, int applicationId, CancellationToken ct)
        {
            // TODO: implement real check if you have a linking table.
            return Task.CompletedTask;
        }

        private async Task<List<string>> GetUserAppRolesAsync(int userId, int applicationId, CancellationToken ct)
        {
            // NOTE: your RoleUser entity doesn't have ApplicationId, so we filter roles by app afterwards.
            var roleUsers = await _unitOfWork.RoleUserRepository
                .GetByCustomQuery(q => q.Where(ru => ru.UserId == userId && ru.RecordStatus == 1));

            var roleIds = roleUsers.Select(ru => ru.RoleId).Distinct().ToList();
            if (!roleIds.Any())
                return new List<string>();

            var roles = await _unitOfWork.RoleRepository
                .WhereAsync(r => roleIds.Contains(r.Id) &&
                                 r.ApplicationId == applicationId &&
                                 r.RecordStatus == 1, ct);

            return roles.Select(r => r.Name).Distinct().ToList();
        }

        private string GetClientIp()
        {
            return _http.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
        }

        private string GetUserAgent()
        {
            return _http.HttpContext?.Request?.Headers["User-Agent"].ToString() ?? "unknown";
        }

        private static void ValidateSigningKeyMaterial(AppKeyMaterial keyMaterial, int applicationId)
        {
            if (keyMaterial == null)
                throw new InvalidOperationException($"Signing key material is null for ApplicationId={applicationId}.");

            // Your issuer needs PrivateKeyPem + Kid
            if (string.IsNullOrWhiteSpace(keyMaterial.PrivateKeyPem))
                throw new InvalidOperationException($"Signing private key PEM is missing for ApplicationId={applicationId}.");

            if (string.IsNullOrWhiteSpace(keyMaterial.Kid))
                throw new InvalidOperationException($"Signing key KID is missing for ApplicationId={applicationId}.");
        }

        private async Task SaveChangesWithRetryAsync(CancellationToken ct)
        {
            const int maxAttempts = 2;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    await _unitOfWork.SaveChangesAsync();
                    return;
                }
                catch when (attempt < maxAttempts)
                {
                    await Task.Delay(150, ct);
                }
            }
        }

        private async Task RegisterLoginAttemptSafeAsync(string username, int userId, bool success, string message, CancellationToken ct)
        {
            try
            {
                await RegisterLoginAttempt(username, userId, success, message, ct);
            }
            catch
            {
                // swallow intentionally - audit must not break auth
            }
        }

        private Task RegisterLoginAttempt(string username, int userId, bool success, string message, CancellationToken ct)
        {
            // TODO: Persist LoginAttempt/LoginAudit if needed.
            return Task.CompletedTask;
        }

        #endregion
    }
}
