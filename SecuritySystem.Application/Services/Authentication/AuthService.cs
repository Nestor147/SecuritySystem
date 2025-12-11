using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SecuritySystem.Application.Exceptions;
using SecuritySystem.Application.Helpers.Authentication;
using SecuritySystem.Application.Interfaces.Authentication;
using SecuritySystem.Application.Interfaces.Authentication.Dtos;
using SecuritySystem.Core.Entities;
using SecuritySystem.Core.Entities.SealedAuthentication;
using SecuritySystem.Core.Interfaces.Core;
using System.Linq;
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
        private readonly IJwtIssuer _jwtIssuer;  // <- NUEVO

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher hasher,
            IConfiguration cfg,
            IHttpContextAccessor http,
            IAppSigningKeyProvider appSigningKeyProvider,
            IAppTokenPolicyProvider appTokenPolicyProvider,
            IJwtIssuer jwtIssuer   // <- NUEVO
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
            // 1) Validate user credentials (global user)
            var user = await GetUserByCredentialsAsync(req, ct);
            if (user == null)
                throw new InvalidCredentialsException("Invalid username or password.");

            // 2) Resolve application (by Code)
            var app = await _unitOfWork.ApplicationRepository
                .FirstOrDefaultAsync(a => a.Id == req.ApplicationId && a.RecordStatus == 1, ct)
                ?? throw new AppNotFoundException($"Application {req.ApplicationId} does not exist or is inactive.");


            // 3) Ensure user is allowed in this application
            // IMPORTANT: for now we DON'T block if the user has no roles.
            await EnsureUserHasAccessToAppAsync(user.Id, app.Id, ct);

            // 4) Get signing key & token policy
            var keyMaterial = await _appSigningKeyProvider.GetActiveSigningKeyAsync(app.Id, ct);
            var policy = await _appTokenPolicyProvider.GetPolicyAsync(app.Id, ct);

            var accessLifetime = TimeSpan.FromMinutes(policy.AccessTokenMinutes);
            var refreshLifetime = TimeSpan.FromDays(policy.RefreshTokenDays);

            var issuer = $"Authentication.API/{app.Code}";
            var audience = $"Authentication.Clients/{app.Code}";
            var jti = Guid.NewGuid().ToString("N");

            // 5) Load roles ONLY for this app
            var roleNames = await GetUserAppRolesAsync(user.Id, app.Id, ct);

            // 6) Extra claims (username, email, JSON roles, etc.)
            var extraClaims = await BuildAccessClaimsAsync(user, app, roleNames, jti, ct);

            // 7) Build TokenDescriptor for RSA JWT
            // 7) Build TokenDescriptor for RSA JWT
            var descriptor = new TokenDescriptor(
                subject: user.Id.ToString(),
                issuer: issuer,
                audience: audience,
                jti: jti,
                lifetime: accessLifetime,
                roles: roleNames,
                claims: new Dictionary<string, object>
                {
                    ["username"] = user.Username,
                    ["email"] = user.Email ?? string.Empty,
                    ["app_code"] = app.Code
                },
                extraClaims: extraClaims
            );


            var accessToken = _jwtIssuer.CreateAccessToken(descriptor, keyMaterial);

            // 8) Refresh token per user + app
            var opaque = RefreshTokenHasher.GenerateOpaque();
            var hash = RefreshTokenHasher.Hash(opaque);

            await _unitOfWork.RefreshTokenRepository.InsertAsync(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ApplicationId = app.Id,
                TokenHash = hash,
                ExpiresAt = DateTime.UtcNow.Add(refreshLifetime),
                TokenCreatedAt = DateTime.UtcNow,
                IPAddress = GetClientIp(),
                UserAgent = GetUserAgent()
            });

            await _unitOfWork.SaveChangesAsync();
            await RegisterLoginAttempt(req.Username, user.Id, true, "Login ok", ct);

            // 9) Return tokens + roles (per app)
            return new LoginResult
            {
                AccessToken = accessToken,
                RefreshToken = opaque,
                //ApplicationId = app.Id,
                //ApplicationCode = app.Code,
                //Roles = roleNames
            };
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
            // We allow login by Username OR Email
            var entity = await _unitOfWork.UserRepository
                .FirstOrDefaultAsync(u =>
                    u.RecordStatus == 1 &&
                    (u.Username == req.Username || u.Email == req.Username),
                    ct);

            if (entity == null)
                return null;

            // Verify password against BCrypt hash
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

            // Standard role claims
            foreach (var role in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // JSON list of roles (for frontend/backend convenience)
            claims.Add(new Claim(
                "roles",
                JsonSerializer.Serialize(roleNames),
                Microsoft.IdentityModel.JsonWebTokens.JsonClaimValueTypes.Json));

            // Username & email
            claims.Add(new Claim("username", user.Username));
            if (!string.IsNullOrWhiteSpace(user.Email))
                claims.Add(new Claim("email", user.Email));

            return Task.FromResult(claims);
        }



        private Task EnsureUserHasAccessToAppAsync(int userId, int applicationId, CancellationToken ct)
        {
            // TODO: implement real check:
            // query your UserApplication / UserRole table to ensure user is linked to this app.
            return Task.CompletedTask;
        }

        private async Task<List<string>> GetUserAppRolesAsync(int userId, int applicationId, CancellationToken ct)
        {
            var roleUsers = await _unitOfWork.RoleUserRepository
                .GetByCustomQuery(roles=>roles.Where(ru => ru.UserId == userId && ru.RecordStatus == 1));
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

        private Task RegisterLoginAttempt(string username, int userId, bool success, string message, CancellationToken ct)
        {
            // TODO: persist login attempt if needed
            return Task.CompletedTask;
        }

        #endregion


        public async Task<LoginResult> RefreshAsync(RefreshRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.RefreshToken))
                throw new InvalidCredentialsException("Refresh token is required.");

            var hash = RefreshTokenHasher.Hash(req.RefreshToken);

            // 1) Buscar el refresh token en BD
            var tokenEntity = await _unitOfWork.RefreshTokenRepository
                .FirstOrDefaultAsync(t => t.TokenHash == hash, ct);

            if (tokenEntity == null || tokenEntity.ExpiresAt <= DateTime.UtcNow)
                throw new InvalidCredentialsException("Refresh token is invalid or expired.");

            // 2) Cargar usuario
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

            // 3) Cargar aplicación
            var app = await _unitOfWork.ApplicationRepository
                .FirstOrDefaultAsync(a => a.Id == tokenEntity.ApplicationId && a.RecordStatus == 1, ct)
                ?? throw new AppNotFoundException($"Application {tokenEntity.ApplicationId} does not exist or is inactive.");

            // 4) Llave RSA y política
            var keyMaterial = await _appSigningKeyProvider.GetActiveSigningKeyAsync(app.Id, ct);
            var policy = await _appTokenPolicyProvider.GetPolicyAsync(app.Id, ct);

            var accessLifetime = TimeSpan.FromMinutes(policy.AccessTokenMinutes);
            var refreshLifetime = TimeSpan.FromDays(policy.RefreshTokenDays);

            var issuer = $"Authentication.API/{app.Code}";
            var audience = $"Authentication.Clients/{app.Code}";
            var jti = Guid.NewGuid().ToString("N");

            // 5) Roles del usuario para esta app
            var roleNames = await GetUserAppRolesAsync(user.Id, app.Id, ct);

            // 6) Claims extra
            var extraClaims = await BuildAccessClaimsAsync(user, app, roleNames, jti, ct);

            // 7) Descriptor y nuevo access token
            var descriptor = new TokenDescriptor(
                subject: user.Id.ToString(),
                issuer: issuer,
                audience: audience,
                jti: jti,
                lifetime: accessLifetime,
                roles: roleNames,
                claims: new Dictionary<string, object>
                {
                    ["username"] = user.Username,
                    ["email"] = user.Email ?? string.Empty,
                    ["app_code"] = app.Code
                },
                extraClaims: extraClaims
            );

            var newAccessToken = _jwtIssuer.CreateAccessToken(descriptor, keyMaterial);

            // 8) Rotar refresh token (más seguro)
            var newOpaque = RefreshTokenHasher.GenerateOpaque();
            var newHash = RefreshTokenHasher.Hash(newOpaque);

            tokenEntity.TokenHash = newHash;
            tokenEntity.TokenCreatedAt = DateTime.UtcNow;
            tokenEntity.ExpiresAt = DateTime.UtcNow.Add(refreshLifetime);
            tokenEntity.IPAddress = GetClientIp();
            tokenEntity.UserAgent = GetUserAgent();

             _unitOfWork.RefreshTokenRepository.Update(tokenEntity);
            await _unitOfWork.SaveChangesAsync();

            // 9) Devolver nuevo access + refresh
            return new LoginResult
            {
                AccessToken = newAccessToken,
                RefreshToken = newOpaque,

            };
        }

        public async Task LogoutAsync(LogoutRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.RefreshToken))
                return; // logout idempotente

            var hash = RefreshTokenHasher.Hash(req.RefreshToken);

            var tokenEntity = await _unitOfWork.RefreshTokenRepository
                .FirstOrDefaultAsync(t => t.TokenHash == hash, ct);

            if (tokenEntity == null)
                return;

            // Puedes borrar o marcar como revocado; aquí lo borramos
             _unitOfWork.RefreshTokenRepository.Delete(tokenEntity);
            await _unitOfWork.SaveChangesAsync();
        }


    }
}
