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
    public class ResourceController : ControllerBase
    {
        #region Attributes
        private readonly IResourceService _resourceService;
        #endregion

        #region Constructor
        public ResourceController(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }
        #endregion

        #region Methods

        [HttpPost]
        [Route("CreateNode")]
        public async Task<IActionResult> InsertResource([FromBody] ResourceQueryFilter resourceQueryFilter)
        {
            try
            {
                resourceQueryFilter.ResourceType = resourceQueryFilter.IsGhost
                    ? (int)ResourceType.GhostNode
                    : (int)ResourceType.Node;

                var resource = await _resourceService.InsertResource(resourceQueryFilter);
                return StatusCode((int)resource.StatusCode, resource);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpPost]
        [Route("UpdateNode")]
        public async Task<IActionResult> UpdateResource([FromBody] ResourceQueryFilter resourceQueryFilter)
        {
            try
            {
                resourceQueryFilter.ResourceType = resourceQueryFilter.IsGhost
                    ? (int)ResourceType.GhostNode
                    : (int)ResourceType.Node;

                var resource = await _resourceService.UpdateResource(resourceQueryFilter);
                return StatusCode((int)resource.StatusCode, resource);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetNodeById")]
        public async Task<IActionResult> GetResourceById([FromQuery] string nodeId)
        {
            try
            {
                var node = await _resourceService.GetResourceById(nodeId);
                return StatusCode((int)node.StatusCode, node);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpPost]
        [Route("DeleteNode")]
        public async Task<IActionResult> DeleteResourceById([FromBody] IdStringFilter resourceId)
        {
            try
            {
                int resourceType = (int)ResourceType.Node;
                var node = await _resourceService.DeleteResourceById(resourceId, resourceType);
                return StatusCode((int)node.StatusCode, node);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetNodeList")]
        public async Task<IActionResult> GetResourceList([FromQuery] GetResourcesQueryFilter queryFilter)
        {
            try
            {
                var isGhost = bool.TryParse(queryFilter.IsGhost, out var value) ? value : false;
                queryFilter.ResourceType = isGhost
                    ? (int)ResourceType.GhostNode
                    : (int)ResourceType.Node;

                var nodes = await _resourceService.GetResourceList(queryFilter);

                if (nodes is ResponseGet)
                    return StatusCode((int)HttpStatusCode.NotFound, nodes);

                if (nodes is ResponseGetObject)
                {
                    var data = nodes as ResponseGetObject;
                    var response = new ApiResponse<object>(data.Datos)
                    {
                        Mensajes = data.Mensajes
                    };
                    return StatusCode((int)data.StatusCode, response);
                }
                else if (nodes is ResponseGetPagination)
                {
                    var data = nodes as ResponseGetPagination;
                    var pagination = new Pagination(data.Paginacion);

                    var response = new ApiResponse<object>(data.Paginacion)
                    {
                        Mensajes = data.Mensajes,
                        Paginacion = pagination,
                    };

                    return StatusCode((int)data.StatusCode, response);
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
                        }
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
        [Route("GetNodes")]
        public async Task<IActionResult> GetResources([FromQuery] GetResourcesQueryFilter queryFilter)
        {
            try
            {
                var isGhost = bool.TryParse(queryFilter.IsGhost, out var value) ? value : false;
                queryFilter.ResourceType = isGhost
                    ? (int)ResourceType.GhostNode
                    : (int)ResourceType.Node;

                var nodes = await _resourceService.GetResources(queryFilter);
                return StatusCode((int)nodes.StatusCode, nodes);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpPost]
        [Route("CreatePage")]
        public async Task<IActionResult> CreatePage([FromBody] ResourceQueryFilter resourceQueryFilter)
        {
            try
            {
                resourceQueryFilter.ResourceType = resourceQueryFilter.IsGhost
                    ? (int)ResourceType.GhostPage
                    : (int)ResourceType.Page;

                var resource = await _resourceService.InsertResource(resourceQueryFilter);
                return StatusCode((int)resource.StatusCode, resource);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpPost]
        [Route("UpdatePage")]
        public async Task<IActionResult> UpdatePage([FromBody] ResourceQueryFilter resourceQueryFilter)
        {
            try
            {
                resourceQueryFilter.ResourceType = resourceQueryFilter.IsGhost
                    ? (int)ResourceType.GhostPage
                    : (int)ResourceType.Page;

                var resource = await _resourceService.UpdateResource(resourceQueryFilter);
                return StatusCode((int)resource.StatusCode, resource);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetPageById")]
        public async Task<IActionResult> GetPageById([FromQuery] string pageId)
        {
            try
            {
                var page = await _resourceService.GetResourceById(pageId);
                return StatusCode((int)page.StatusCode, page);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpPost]
        [Route("DeletePage")]
        public async Task<IActionResult> DeletePage([FromBody] IdStringFilter resourceId)
        {
            try
            {
                int resourceType = (int)ResourceType.Page;
                var page = await _resourceService.DeleteResourceById(resourceId, resourceType);
                return StatusCode((int)page.StatusCode, page);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpGet]
        [Route("GetPageList")]
        public async Task<IActionResult> GetPageList([FromQuery] GetResourcesQueryFilter queryFilter)
        {
            try
            {
                var isGhost = bool.TryParse(queryFilter.IsGhost, out var value) ? value : false;
                queryFilter.ResourceType = isGhost
                    ? (int)ResourceType.GhostPage
                    : (int)ResourceType.Page;

                var pages = await _resourceService.GetResourceList(queryFilter);

                if (pages is ResponseGet)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, pages);
                }

                if (pages is ResponseGetObject)
                {
                    var responseDataObjectWithoutPag = pages as ResponseGetObject;
                    var response = new ApiResponse<object>(responseDataObjectWithoutPag.Datos)
                    {
                        Mensajes = responseDataObjectWithoutPag.Mensajes
                    };
                    return StatusCode((int)responseDataObjectWithoutPag.StatusCode, response);
                }

                else if (pages is ResponseGetPagination)
                {
                    var responseDataPagination = pages as ResponseGetPagination;
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
        [Route("GetPages")]
        public async Task<IActionResult> GetPages([FromQuery] GetResourcesQueryFilter queryFilter)
        {
            try
            {
                var isGhost = bool.TryParse(queryFilter.IsGhost, out var value) ? value : false;
                queryFilter.ResourceType = isGhost
                    ? (int)ResourceType.GhostPage
                    : (int)ResourceType.Page;

                var pages = await _resourceService.GetResources(queryFilter);
                return StatusCode((int)pages.StatusCode, pages);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        #endregion
    }
}
