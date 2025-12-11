using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuritySystem.Application.Exceptions;
using SecuritySystem.Application.Interfaces.Authentication;
using SecuritySystem.Application.Interfaces.Authentication.Dtos;
using SecuritySystem.Application.Services.Authentication;
using SecuritySystem.Core.Entities.SealedAuthentication;
using System.Net;

namespace SecuritySystem.Web.Controllers.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Login by Application + Username + Password
        /// </summary>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.LoginAsync(request, cancellationToken);

                return Ok(result);
            }
            catch (InvalidCredentialsException ex)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new
                {
                    message = ex.Message
                });
            }
            catch (AppNotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new
                {
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    message = "Unexpected error during login."
                });
            }
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh(
           [FromBody] RefreshRequest request,
           CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.RefreshAsync(request, cancellationToken);
                return Ok(result);
            }
            catch (InvalidCredentialsException ex)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new
                {
                    message = ex.Message
                });
            }
            catch (AppNotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new
                {
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    message = "Unexpected error during token refresh."
                });
            }
        }

        /// <summary>
        /// Logout by revoking the refresh token.
        /// </summary>
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout(
            [FromBody] LogoutRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _authService.LogoutAsync(request, cancellationToken);

                return Ok(new
                {
                    message = "Logout successful."
                });
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    message = "Unexpected error during logout."
                });
            }
        }
    }
}
