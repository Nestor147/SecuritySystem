using Microsoft.AspNetCore.Mvc;
using SecuritySystem.Application.Interfaces.Authentication;
using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi;
using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;
using SecuritySystem.Core.Entities.SealedAuthentication;
using SecuritySystem.Core.Enums;
using System.Net;

namespace SecuritySystem.Web.Controllers.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Creates a new user in SecuritySystem using:
        /// - ExternalUserId (optional)
        /// - Email (used as Username)
        /// - Password
        /// </summary>
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateUser(
            [FromBody] CreateUserRequest request,
            CancellationToken cancellationToken)
        {
            ResponsePost response = await _userService.CreateUserAsync(request, cancellationToken);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> SearchUsers(
            [FromQuery] string criteria,
            CancellationToken ct)
        {
            try
            {
                var response = await _userService.SearchUsersAsync(criteria, ct);

                // ResponseGet: Data, Messages, StatusCode
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ResponseGet
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Data = Array.Empty<object>(),
                    Messages = new[]
                    {
                        new Message
                        {
                            Type = TypeMessage.error.ToString(),
                            Description = $"An unexpected error occurred in controller while searching users: {ex.Message}"
                        }
                    }
                };

                return StatusCode((int)errorResponse.StatusCode, errorResponse);
            }
        }
    }
}
