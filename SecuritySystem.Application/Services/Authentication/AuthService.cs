using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SecuritySystem.Application.Exceptions;
using SecuritySystem.Application.Helpers.Authentication;
using SecuritySystem.Application.Interfaces.Authentication;
using SecuritySystem.Application.Interfaces.Authentication.Dtos;
using SecuritySystem.Core.Entities;
using SecuritySystem.Core.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SecuritySystem.Application.Services.Authentication
{
    public class AuthService
    {
        private readonly IAppSigningKeyProvider _appSigningKeyProvider;
        private readonly IAppTokenPolicyProvider _appTokenPolicyProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtIssuer _jwtIssuer;
        private readonly IConfiguration _cfg;
        private readonly IPasswordHasher _hasher;
        private readonly IHttpContextAccessor _http;

        public AuthService(
            IUnitOfWork unitOfWork,
            IJwtIssuer jwtIssuer,
            IPasswordHasher hasher,
            IConfiguration cfg,
            IHttpContextAccessor http,
            IAppSigningKeyProvider appSigningKeyProvider,
            IAppTokenPolicyProvider appTokenPolicyProvider
        )
        {
            _unitOfWork = unitOfWork;
            _jwtIssuer = jwtIssuer;
            _hasher = hasher;
            _cfg = cfg;
            _http = http;
            _appSigningKeyProvider = appSigningKeyProvider;
            _appTokenPolicyProvider = appTokenPolicyProvider;
        }

        public async Task<LoginResult> LoginAsync(LoginRequest req, CancellationToken ct)
        {
            // 1) Validate user credentials (global user)
            var user = await GetUserByCredentialsAsync(req, ct);
            if (user == null)
                throw new InvalidCredentialsException("Invalid username or password.");

            // 2) Resolve application
            var app = await _unitOfWork.ApplicationRepository
                .FirstOrDefaultAsync(a => a.Code == req.ApplicationCode && a.RecordStatus == 1, ct)
                ?? throw new AppNotFoundException($"Application {req.ApplicationCode} does not exist or is inactive.");

            // 3) Ensure user is allowed in this application (user-app relation)
            await EnsureUserHasAccessToAppAsync(user.Id, app.Id, ct);

            // 4) Get signing key & token policy for this app
            var keyMaterial = await _appSigningKeyProvider.GetActiveSigningKeyAsync(app.Id, ct);
            var policy = await _appTokenPolicyProvider.GetPolicyAsync(app.Id, ct);

            var accessLifetime = TimeSpan.FromMinutes(policy.AccessTokenMinutes);
            var refreshLifetime = TimeSpan.FromDays(policy.RefreshTokenDays);

            var issuer = $"Authentication.API/{app.Code}";
            var audience = $"Authentication.Clients/{app.Code}";
            var jti = Guid.NewGuid().ToString("N");

            // 5) Build claims
            var claims = await BuildAccessClaimsAsync(user, ct);

            // 6) Load roles ONLY for this app
            var roleNames = await GetUserAppRolesAsync(user.Id, app.Id, ct);
            foreach (var r in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, r));
            }

            claims.Add(new Claim(
                "roles",
                JsonSerializer.Serialize(roleNames),
                Microsoft.IdentityModel.JsonWebTokens.JsonClaimValueTypes.Json));

            claims.Add(new Claim("app_code", app.Code));
            claims.Add(new Claim("app_id", app.Id.ToString()));

            // 7) Generate access token (placeholder, replace with real JWT)
            var accessToken = $"access_{Guid.NewGuid():N}";

            // 8) Generate refresh token (per user + app)
            var opaque = RefreshTokenHasher.GenerateOpaque();
            var hash = RefreshTokenHasher.Hash(opaque);

            await _unitOfWork.RefreshTokenRepository.InsertAsync(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ApplicationId = app.Id,                   // ✅ per app
                TokenHash = hash,
                ExpiresAt = DateTime.UtcNow.Add(refreshLifetime),
                TokenCreatedAt = DateTime.UtcNow,
                IPAddress = GetClientIp(),
                UserAgent = GetUserAgent()
            });

            await _unitOfWork.SaveChangesAsync();
            await RegisterLoginAttempt(req.Username, user.Id, true, "Login ok", ct);

            // 9) Return tokens + roles for this app
            return new LoginResult
            {
                AccessToken = accessToken,
                RefreshToken = opaque,
                ApplicationId = app.Id,
                ApplicationCode = app.Code,
                Roles = roleNames
            };
        }

        #region Private helpers

        private sealed class AuthUser
        {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public string PasswordHash { get; set; } = string.Empty;
            public int RecordStatus { get; set; }
        }

        private async Task<AuthUser?> GetUserByCredentialsAsync(LoginRequest req, CancellationToken ct)
        {
            // TODO: replace with your real UserRepository query
            var fakeUser = new AuthUser
            {
                Id = 1,
                Username = req.Username,
                PasswordHash = _hasher.Hash("123456"),
                RecordStatus = 1
            };

            if (!_hasher.Verify(req.Password, fakeUser.PasswordHash))
                return null;

            return fakeUser;
        }

        private Task<List<Claim>> BuildAccessClaimsAsync(AuthUser user, CancellationToken ct)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            return Task.FromResult(claims);
        }

        private Task EnsureUserHasAccessToAppAsync(int userId, int applicationId, CancellationToken ct)
        {
            // TODO: implement real check:
            // query your UserApplication / UserRole table to ensure user is linked to this app.
            return Task.CompletedTask;
        }

        private Task<List<string>> GetUserAppRolesAsync(int userId, int applicationId, CancellationToken ct)
        {
            // TODO: implement real query to get roles for this user in this app
            var roles = new List<string>();
            return Task.FromResult(roles);
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
    }
}
