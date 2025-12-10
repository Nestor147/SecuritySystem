using Microsoft.AspNetCore.Mvc;
using SecuritySystem.Application.Interfaces.Authentication;
using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi;
using SecuritySystem.Core.Entities.SealedAuthentication;

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
    }
}
