using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuritySystem.Application.Interfaces.Authorization;
using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi;
using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;
using SecuritySystem.Core.Enums;
using SecuritySystem.Core.GetQueryFilter.Autorization;
using SecuritySystem.Core.QueryFilters.Autorization;
using SecuritySystem.Core.QueryFilters.Helper;
using System.Net;

namespace SecuritySystem.Web.Controllers.Authorization
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        #region Methods

        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> InsertRole(
            [FromBody] RoleQueryFilter roleQueryFilter,
            [FromHeader] string token)
        {
            try
            {
                var role = await _roleService.InsertRole(roleQueryFilter, token);
                return StatusCode((int)role.StatusCode, role);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpPost]
        [Route("UpdateRole")]
        public async Task<IActionResult> UpdateRole(
            [FromBody] RoleQueryFilter roleQueryFilter,
            [FromHeader] string token)
        {
            try
            {
                var role = await _roleService.UpdateRole(roleQueryFilter, token);
                return StatusCode((int)role.StatusCode, role);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetRoleById")]
        public async Task<IActionResult> GetRoleById([FromQuery] string roleId)
        {
            try
            {
                var role = await _roleService.GetRoleById(roleId);
                return StatusCode((int)role.StatusCode, role);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpPost]
        [Route("DeleteRole")]
        public async Task<IActionResult> DeleteRoleById([FromBody] IdStringFilter roleId)
        {
            try
            {
                var role = await _roleService.DeleteRoleById(roleId);
                return StatusCode((int)role.StatusCode, role);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetRoleList")]
        public async Task<IActionResult> GetRoleList(
            [FromQuery] GetRoleQueryFilter getRoleQueryFilter)
        {
            try
            {
                var roles = await _roleService.GetRoleList(getRoleQueryFilter);

                if (roles is ResponseGet)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, roles);
                }

                if (roles is ResponseGetObject)
                {
                    var responseDataObjectWithoutPag = roles as ResponseGetObject;
                    var response = new ApiResponse<object>(responseDataObjectWithoutPag.Datos)
                    {
                        Mensajes = responseDataObjectWithoutPag.Mensajes
                    };
                    return StatusCode((int)responseDataObjectWithoutPag.StatusCode, response);
                }
                else if (roles is ResponseGetPagination)
                {
                    var responseDataPagination = roles as ResponseGetPagination;
                    var pagination = new Pagination(responseDataPagination.Paginacion);
                    var response = new ApiResponse<object>(responseDataPagination.Paginacion)
                    {
                        Mensajes = responseDataPagination.Mensajes,
                        Paginacion = pagination,
                    };
                    return StatusCode((int)responseDataPagination.StatusCode, response);
                }
                else
                {
                    var response = new ResponseGet()
                    {
                        Mensajes = new Message[]
                        {
                            new Message()
                            {
                                Tipo = TypeMessage.error.ToString(),
                                Descripcion = "Response format type not supported"
                            }
                        },
                    };
                    return StatusCode(400, response);
                }
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetRolesCombo")]
        public async Task<IActionResult> GetRolesCombo(
            [FromQuery] GetRoleQueryFilter getRoleQueryFilter)
        {
            try
            {
                var roles = await _roleService.GetRolesCombo(getRoleQueryFilter);
                return StatusCode((int)roles.StatusCode, roles);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetUsersByRole")]
        public async Task<IActionResult> GetUsersByRole(
            [FromQuery] GetUsersByRoleQueryFilter getUsersByRoleQueryFilter)
        {
            try
            {
                var result = await _roleService.GetUsersByRole(getUsersByRoleQueryFilter);

                if (result is ResponseGet)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, result);
                }

                if (result is ResponseGetObject)
                {
                    var responseDataObjectWithoutPag = result as ResponseGetObject;
                    var response = new ApiResponse<object>(responseDataObjectWithoutPag.Datos)
                    {
                        Mensajes = responseDataObjectWithoutPag.Mensajes
                    };
                    return StatusCode((int)responseDataObjectWithoutPag.StatusCode, response);
                }
                else if (result is ResponseGetPagination)
                {
                    var responseDataPagination = result as ResponseGetPagination;
                    var pagination = new Pagination(responseDataPagination.Paginacion);
                    var response = new ApiResponse<object>(responseDataPagination.Paginacion)
                    {
                        Mensajes = responseDataPagination.Mensajes,
                        Paginacion = pagination,
                    };
                    return StatusCode((int)responseDataPagination.StatusCode, response);
                }
                else
                {
                    var response = new ResponseGet()
                    {
                        Mensajes = new Message[]
                        {
                            new Message()
                            {
                                Tipo = TypeMessage.error.ToString(),
                                Descripcion = "Response format type not supported"
                            }
                        },
                    };
                    return StatusCode(400, response);
                }
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetRolesByUser")]
        public async Task<IActionResult> GetRolesByUser(
            [FromQuery] GetUsersByRoleQueryFilter getUsersByRoleQueryFilter)
        {
            try
            {
                var result = await _roleService.GetRolesByUser(getUsersByRoleQueryFilter);

                if (result is ResponseGet)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, result);
                }

                if (result is ResponseGetObject)
                {
                    var responseDataObjectWithoutPag = result as ResponseGetObject;
                    var response = new ApiResponse<object>(responseDataObjectWithoutPag.Datos)
                    {
                        Mensajes = responseDataObjectWithoutPag.Mensajes
                    };
                    return StatusCode((int)responseDataObjectWithoutPag.StatusCode, response);
                }
                else if (result is ResponseGetPagination)
                {
                    var responseDataPagination = result as ResponseGetPagination;
                    var pagination = new Pagination(responseDataPagination.Paginacion);
                    var response = new ApiResponse<object>(responseDataPagination.Paginacion)
                    {
                        Mensajes = responseDataPagination.Mensajes,
                        Paginacion = pagination,
                    };
                    return StatusCode((int)responseDataPagination.StatusCode, response);
                }
                else
                {
                    var response = new ResponseGet()
                    {
                        Mensajes = new Message[]
                        {
                            new Message()
                            {
                                Tipo = TypeMessage.error.ToString(),
                                Descripcion = "Response format type not supported"
                            }
                        },
                    };
                    return StatusCode(400, response);
                }
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpPost]
        [Route("DeleteUserRole")]
        public async Task<IActionResult> DeleteUserRoleById(
            [FromBody] IdStringFilter userRoleId)
        {
            try
            {
                var role = await _roleService.DeleteUserRoleById(userRoleId);
                return StatusCode((int)role.StatusCode, role);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetRolesUsers")]
        public async Task<IActionResult> GetRolesUsers(
            [FromBody] GetUserRoles queryFilter)
        {
            try
            {
                var result = await _roleService.GetRolesUsers(queryFilter);
                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpPost]
        [Route("CreateUserRoles")]
        public async Task<IActionResult> CreateUserRoles(
            [FromBody] List<InsertUserRoleQueryFilter> queryFilter)
        {
            try
            {
                var result = await _roleService.InsertUserRoles(queryFilter);
                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetRolesForMultipleUsers")]
        public async Task<IActionResult> GetRolesForMultipleUsers(
            [FromQuery] GetUserRoles queryFilter)
        {
            try
            {
                var result = await _roleService.GetRolesForMultipleUsers(queryFilter);
                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("SearchUsersByDocOrName")]
        public async Task<IActionResult> SearchUsersByDocOrName(
            [FromQuery] SearchUsersQueryFilter queryFilter)
        {
            try
            {
                var result = await _roleService.SearchUsersByDocOrName(queryFilter);
                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        #endregion
    }
}
