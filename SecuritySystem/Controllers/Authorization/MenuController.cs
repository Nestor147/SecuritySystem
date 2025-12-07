using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuritySystem.Application.Interfaces.Authorization;
using SecuritySystem.Core.GetQueryFilter.Autorization;
using SecuritySystem.Core.QueryFilters.Autorization;
using SecuritySystem.Core.QueryFilters.Helper;

namespace SecuritySystem.Controllers.Authorization
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(
            IMenuService menuService
        )
        {
            _menuService = menuService;
        }

        #region Methods V2

        [HttpPost]
        [Route("CreateMenu")]
        public async Task<IActionResult> CreateMenu(
            [FromBody] MenuV2QueryFilter menuQueryFilter)
        {
            try
            {
                var menu = await _menuService.CreateResourceMenu(menuQueryFilter);
                return StatusCode((int)menu.StatusCode, menu);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetMenuByApplication")]
        public async Task<IActionResult> GetMenuByApplication(
            [FromQuery] IdStringFilter applicationIdStringFilter)
        {
            try
            {
                var menu = await _menuService.GetMenuByApplication(applicationIdStringFilter.Id);
                return StatusCode((int)menu.StatusCode, menu);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpPost]
        [Route("InsertRoleResource")]
        public async Task<IActionResult> InsertRoleResource(
            [FromBody] InsertRoleResourceQueryFilter queryFilter)
        {
            try
            {
                var resources = await _menuService.InsertRoleResource(queryFilter);
                return StatusCode((int)resources.StatusCode, resources);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetRoleResources")]
        public async Task<IActionResult> GetRoleResources(
            [FromQuery] GetRoleMenuQueryFilter queryFilter)
        {
            try
            {
                var menu = await _menuService.GetRoleResources(queryFilter);
                return StatusCode((int)menu.StatusCode, menu);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetMenuByRole")]
        public async Task<IActionResult> GetMenuByRole(
            [FromQuery] GetMenuQueryFilter getMenuQueryFilter)
        {
            try
            {
                var menu = await _menuService.GetMenuByRole(getMenuQueryFilter);
                return StatusCode((int)menu.StatusCode, menu);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetRolesByResourceId")]
        public async Task<IActionResult> GetRolesByResourceId(
            [FromQuery] string resourceId)
        {
            try
            {
                var roles = await _menuService.GetRolesByResourceId(resourceId);
                return StatusCode((int)roles.StatusCode, roles);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("ValidateResourceMenuChanges")]
        public async Task<IActionResult> ValidateResourceMenuChanges(
            [FromQuery] ValidateMenuQueryFilter menuQueryFilter)
        {
            try
            {
                var menu = await _menuService.ValidateResourceMenuChanges(menuQueryFilter);
                return StatusCode((int)menu.StatusCode, menu);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetMenuResourcesByUser")]
        public async Task<IActionResult> GetMenuResourcesByUser(
            [FromQuery] GetMenuQueryFilter getMenuQueryFilter)
        {
            try
            {
                var menu = await _menuService.GetMenuResourcesByUser(getMenuQueryFilter);
                return StatusCode((int)menu.StatusCode, menu);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetMenuByUserRoles")]
        public async Task<IActionResult> GetMenuByUserRoles(
            [FromQuery] GetMenuQueryFilter getMenuQueryFilter)
        {
            try
            {
                var menu = await _menuService.GetMenuByUserRoles(getMenuQueryFilter);
                return StatusCode((int)menu.StatusCode, menu);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        #endregion
    }
}
