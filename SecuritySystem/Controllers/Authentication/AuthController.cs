using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecuritySystem.Application.Exceptions;
using SecuritySystem.Application.Interfaces.Authentication;
using SecuritySystem.Application.Interfaces.Authentication.Dtos;
using SecuritySystem.Core.Entities.SealedAuthentication;
using System.Net;
using System.Security.Cryptography;

namespace SecuritySystem.Web.Controllers.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.LoginAsync(request, cancellationToken);
                return Ok(result);
            }
            catch (InvalidCredentialsException ex)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new { message = ex.Message });
            }
            catch (AppNotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new { message = ex.Message });
            }
            // Casos frecuentes que suelen ser “intermitentes”
            catch (DbUpdateException ex)
            {
                var traceId = HttpContext.TraceIdentifier;
                _logger.LogError(ex,
                    "LOGIN DbUpdateException. TraceId={TraceId} AppId={AppId} Username={Username}",
                    traceId, request?.ApplicationId, request?.Username);

                return StatusCode(500, new
                {
                    message = "Temporary database error during login. Please retry.",
                    traceId
                });
            }
            catch (CryptographicException ex)
            {
                var traceId = HttpContext.TraceIdentifier;
                _logger.LogError(ex,
                    "LOGIN CryptographicException. TraceId={TraceId} AppId={AppId} Username={Username}",
                    traceId, request?.ApplicationId, request?.Username);

                return StatusCode(500, new
                {
                    message = "Cryptographic error during login.",
                    traceId
                });
            }
            catch (Exception ex)
            {
                var traceId = HttpContext.TraceIdentifier;

                _logger.LogError(ex,
                    "LOGIN Unexpected error. TraceId={TraceId} AppId={AppId} Username={Username}",
                    traceId, request?.ApplicationId, request?.Username);

                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    message = "Unexpected error during login.",
                    traceId
                });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.RefreshAsync(request, cancellationToken);
                return Ok(result);
            }
            catch (InvalidCredentialsException ex)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new { message = ex.Message });
            }
            catch (AppNotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                var traceId = HttpContext.TraceIdentifier;
                _logger.LogError(ex, "REFRESH Unexpected error. TraceId={TraceId}", traceId);

                return StatusCode(500, new
                {
                    message = "Unexpected error during token refresh.",
                    traceId
                });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _authService.LogoutAsync(request, cancellationToken);
                return Ok(new { message = "Logout successful." });
            }
            catch (Exception ex)
            {
                var traceId = HttpContext.TraceIdentifier;
                _logger.LogError(ex, "LOGOUT Unexpected error. TraceId={TraceId}", traceId);

                return StatusCode(500, new
                {
                    message = "Unexpected error during logout.",
                    traceId
                });
            }
        }
    }
}
