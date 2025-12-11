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

namespace SecuritySystem.Controllers.Authorization
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationsService _applicationsService;

        public ApplicationsController(
            IApplicationsService applicationsService
        )
        {
            _applicationsService = applicationsService;
        }

        #region Methods

        [HttpPost]
        [Route("CreateApplication")]
        public async Task<IActionResult> CreateApplication(
            [FromBody] ApplicationQueryFilter applicationQueryFilter,
            [FromHeader] string token)
        {
            try
            {
                var application = await _applicationsService.CreateApplication(applicationQueryFilter, token);
                return StatusCode((int)application.StatusCode, application);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpPost]
        [Route("UpdateApplication")]
        public async Task<IActionResult> UpdateApplication(
            [FromBody] ApplicationQueryFilter applicationQueryFilter,
            [FromHeader] string token)
        {
            try
            {
                var application = await _applicationsService.UpdateApplication(applicationQueryFilter, token);
                return StatusCode((int)application.StatusCode, application);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpPost]
        [Route("DeleteApplication")]
        public async Task<IActionResult> DeleteApplication(
            [FromBody] IdStringFilter idFilter,
            [FromHeader] string token)
        {
            try
            {
                var application = await _applicationsService.DeleteApplicationById(idFilter, token);
                return StatusCode((int)application.StatusCode, application);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetApplicationById")]
        public async Task<IActionResult> GetApplicationById([FromQuery] string applicationId)
        {
            try
            {
                var application = await _applicationsService.GetApplicationById(applicationId);
                return StatusCode((int)application.StatusCode, application);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetApplicationList")]
        public async Task<IActionResult> GetApplicationList(
            [FromQuery] GetApplicationsQueryFilter queryFilter)
        {
            try
            {
                var applications = await _applicationsService.GetApplicationList(queryFilter);

                if (applications is ResponseGet)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, applications);
                }

                if (applications is ResponseGetObject)
                {
                    var responseDataObjectWithoutPag = applications as ResponseGetObject;
                    var response = new ApiResponse<object>(responseDataObjectWithoutPag.Datos)
                    {
                        Mensajes = responseDataObjectWithoutPag.Mensajes
                    };
                    return StatusCode((int)responseDataObjectWithoutPag.StatusCode, response);
                }
                else if (applications is ResponseGetPagination)
                {
                    var responseDataPagination = applications as ResponseGetPagination;
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
                        Messages = new Message[]
                        {
                            new Message()
                            {
                                Type = TypeMessage.error.ToString(),
                                Description = "Response format type not supported"
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
        [Route("GetApplications")]
        public async Task<IActionResult> GetApplications(
            [FromQuery] GetApplicationsQueryFilter getApplicationsQueryFilter)
        {
            try
            {
                var applications = await _applicationsService.GetApplications(getApplicationsQueryFilter);
                return StatusCode((int)applications.StatusCode, applications);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetApplicationsByUser")]
        public async Task<IActionResult> GetApplicationsByUser(
            [FromHeader] string token)
        {
            try
            {
                var applications = await _applicationsService.GetApplicationsByUser(token);
                return StatusCode((int)applications.StatusCode, applications);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        #endregion
    }
}
