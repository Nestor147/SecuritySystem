using Microsoft.Extensions.Options;
using SecuritySystem.Application.Interfaces.Authorization;
using SecuritySystem.Core.Custom.DisplayFormat;
using SecuritySystem.Core.Entities;
using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi;
using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;
using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.DisplayFormat;
using SecuritySystem.Core.Enums;
using SecuritySystem.Core.GetQueryFilter.Autorization;
using SecuritySystem.Core.Interfaces;
using SecuritySystem.Core.Interfaces.Core;
using SecuritySystem.Core.Interfaces.Validators;
using SecuritySystem.Core.Interfaces.Validators.Helpers;
using SecuritySystem.Core.QueryFilters.Autorization;
using SecuritySystem.Core.QueryFilters.Helper;
using System.Collections;
using System.Net;

namespace SecuritySystem.Application.Services.Authorization
{
    public class ResourceService : IResourceService
    {
        #region Fields
        private readonly PaginationOptions _paginationOptions;
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly IAuthorizationRepositoryValidator _authorizationRepositoryValidator;
        private readonly IHelperProcessVal _helperProcessValidator;
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region Constructor
        public ResourceService(
            IOptions<PaginationOptions> options,
            IAuthorizationRepository authorizationRepository,
            IAuthorizationRepositoryValidator authorizationRepositoryValidator,
            IHelperProcessVal helperProcessValidator,
            IUnitOfWork unitOfWork)
        {
            _paginationOptions = options.Value;
            _authorizationRepository = authorizationRepository;
            _authorizationRepositoryValidator = authorizationRepositoryValidator;
            _helperProcessValidator = helperProcessValidator;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Methods

        public async Task<ResponsePost> InsertResource(ResourceQueryFilter resourceFilter)
        {
            try
            {
                #region Resource Validator
                if (resourceFilter.ResourceType == (int)ResourceType.GhostNode ||
                    resourceFilter.ResourceType == (int)ResourceType.Node)
                {
                    var validation = _authorizationRepositoryValidator.ValidateCreateNode(resourceFilter);
                    if (!validation.IsValid)
                    {
                        return new ResponsePost
                        {
                            Mensajes = validation.ValidationMessages.ToArray(),
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }
                }
                else
                {
                    var validation = _authorizationRepositoryValidator.ValidateCreatePage(resourceFilter);
                    if (!validation.IsValid)
                    {
                        return new ResponsePost
                        {
                            Mensajes = validation.ValidationMessages.ToArray(),
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }
                }
                #endregion

                #region Business Rules
                resourceFilter.ApplicationId = resourceFilter.ApplicationId;

                var duplicatedResource = await _authorizationRepository.GetDuplicateResources(resourceFilter);

                if (duplicatedResource.Any())
                {
                    if (resourceFilter.ResourceType == (int)ResourceType.Node &&
                        duplicatedResource.Any(a => a.Description == resourceFilter.Description))
                    {
                        return new ResponsePost
                        {
                            Mensajes = new[]
                            {
                                new Message
                                {
                                    Tipo        = TypeMessage.warning.ToString(),
                                    Descripcion = "There is another node with this name in this application."
                                }
                            },
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }

                    if (resourceFilter.ResourceType == (int)ResourceType.Page &&
                        duplicatedResource.Any(a => a.Description == resourceFilter.Description))
                    {
                        return new ResponsePost
                        {
                            Mensajes = new[]
                            {
                                new Message
                                {
                                    Tipo        = TypeMessage.warning.ToString(),
                                    Descripcion = "There is another page with this name in this application."
                                }
                            },
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }

                    if (resourceFilter.ResourceType == (int)ResourceType.Page &&
                        duplicatedResource.Any(a => a.Page == resourceFilter.Page))
                    {
                        return new ResponsePost
                        {
                            Mensajes = new[]
                            {
                                new Message
                                {
                                    Tipo        = TypeMessage.warning.ToString(),
                                    Descripcion = "There is another page with this URL in this application."
                                }
                            },
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }
                }
                #endregion

                resourceFilter.ApplicationId = resourceFilter.ApplicationId;
                resourceFilter.CreatedBy = resourceFilter.CreatedBy;

                var insertResult = _unitOfWork.ResourceRepository.Insert(new Resource
                {
                    ApplicationId = Convert.ToInt32(resourceFilter.ApplicationId),
                    Page = resourceFilter.Page,
                    Name = resourceFilter.Description,
                    Description = resourceFilter.Detail,
                    ResourceType = resourceFilter.ResourceType,
                    IconName = resourceFilter.IconName,
                    IsNew = resourceFilter.IsNew,
                    CreatedBy = resourceFilter.CreatedBy.ToUpper()
                });

                if (insertResult.RowsAffected > 0)
                {
                    if (resourceFilter.ResourceType == (int)ResourceType.Node)
                    {
                        return new ResponsePost
                        {
                            Respuesta = insertResult,
                            StatusCode = HttpStatusCode.OK,
                            Mensajes = new[]
                            {
                                new Message
                                {
                                    Tipo        = TypeMessage.information.ToString(),
                                    Descripcion = "The node was registered successfully."
                                }
                            }
                        };
                    }

                    return new ResponsePost
                    {
                        Respuesta = insertResult,
                        StatusCode = HttpStatusCode.OK,
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo        = TypeMessage.information.ToString(),
                                Descripcion = "The page was registered successfully."
                            }
                        }
                    };
                }

                return new ResponsePost
                {
                    Respuesta = insertResult,
                    StatusCode = HttpStatusCode.NoContent
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponsePost> UpdateResource(ResourceQueryFilter resourceFilter)
        {
            try
            {
                #region Resource Validator
                if (resourceFilter.ResourceType == (int)ResourceType.Node ||
                    resourceFilter.ResourceType == (int)ResourceType.GhostNode)
                {
                    var validation = _authorizationRepositoryValidator.ValidateUpdateNode(resourceFilter);
                    if (!validation.IsValid)
                    {
                        return new ResponsePost
                        {
                            Mensajes = validation.ValidationMessages.ToArray(),
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }
                }
                else
                {
                    var validation = _authorizationRepositoryValidator.ValidateUpdatePage(resourceFilter);
                    if (!validation.IsValid)
                    {
                        return new ResponsePost
                        {
                            Mensajes = validation.ValidationMessages.ToArray(),
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }
                }
                #endregion

                #region Business Rules
                resourceFilter.ApplicationId = resourceFilter.ApplicationId;
                resourceFilter.Id = resourceFilter.Id;

                var duplicatedResource = await _authorizationRepository.GetDuplicateResources(resourceFilter);

                if (duplicatedResource.Any())
                {
                    if (resourceFilter.ResourceType == (int)ResourceType.Node &&
                        duplicatedResource.Any(a => a.Description == resourceFilter.Description))
                    {
                        return new ResponsePost
                        {
                            Mensajes = new[]
                            {
                                new Message
                                {
                                    Tipo        = TypeMessage.warning.ToString(),
                                    Descripcion = "There is another node with this name in this application."
                                }
                            },
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }

                    if (resourceFilter.ResourceType == (int)ResourceType.Page &&
                        duplicatedResource.Any(a => a.Description == resourceFilter.Description))
                    {
                        return new ResponsePost
                        {
                            Mensajes = new[]
                            {
                                new Message
                                {
                                    Tipo        = TypeMessage.warning.ToString(),
                                    Descripcion = "There is another page with this name in this application."
                                }
                            },
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }

                    if (resourceFilter.ResourceType == (int)ResourceType.Page &&
                        duplicatedResource.Any(a => a.Page == resourceFilter.Page))
                    {
                        return new ResponsePost
                        {
                            Mensajes = new[]
                            {
                                new Message
                                {
                                    Tipo        = TypeMessage.warning.ToString(),
                                    Descripcion = "There is another page with this URL in this application."
                                }
                            },
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }
                }
                #endregion

                var updateResult = _unitOfWork.ResourceRepository.UpdateCustom(
                    new Resource
                    {
                        Id = Convert.ToInt32(resourceFilter.Id),
                        Page = resourceFilter.Page,
                        Name = resourceFilter.Description,
                        Description = resourceFilter.Detail,
                        ResourceType = resourceFilter.ResourceType,
                        IconName = resourceFilter.IconName,
                        IsNew = resourceFilter.IsNew,
                        CreatedAt = DateTime.Now,
                        CreatedBy = resourceFilter.CreatedBy.ToUpper()
                    },
                    r => r.Page,
                    r => r.Name,
                    r => r.Description,
                    r => r.ResourceType,
                    r => r.IconName,
                    r => r.IsNew,
                    r => r.CreatedAt,
                    r => r.CreatedBy
                );

                if (updateResult.RowsAffected > 0)
                {
                    if (resourceFilter.ResourceType == (int)ResourceType.Node)
                    {
                        return new ResponsePost
                        {
                            Respuesta = updateResult,
                            StatusCode = HttpStatusCode.OK,
                            Mensajes = new[]
                            {
                                new Message
                                {
                                    Tipo        = TypeMessage.information.ToString(),
                                    Descripcion = "The node was updated successfully."
                                }
                            }
                        };
                    }

                    return new ResponsePost
                    {
                        Respuesta = updateResult,
                        StatusCode = HttpStatusCode.OK,
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo        = TypeMessage.information.ToString(),
                                Descripcion = "The page was updated successfully."
                            }
                        }
                    };
                }

                return new ResponsePost
                {
                    Respuesta = updateResult,
                    StatusCode = HttpStatusCode.NoContent
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseGetObject> GetResourceById(string resourceId)
        {
            try
            {
                #region Id Validator
                var validation = _helperProcessValidator.ValidateStringId(resourceId);
                if (!validation.IsValid)
                {
                    return new ResponseGetObject
                    {
                        Mensajes = validation.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
                #endregion

                var resource = await _unitOfWork.ResourceRepository.GetByIdAsync(Convert.ToInt32(resourceId));

                if (resource == null)
                {
                    return new ResponseGetObject
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo        = TypeMessage.information.ToString(),
                                Descripcion = "Resource not found."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                var isGhost =
                    resource.ResourceType == (int)ResourceType.GhostNode ||
                    resource.ResourceType == (int)ResourceType.GhostPage;

                var resourceFilter = new ResourceQueryFilter
                {
                    ApplicationId = resource.ApplicationId.ToString(),
                    Page = resource.Page,
                    Description = resource.Name,
                    Detail = resource.Description,
                    IsNew = resource.IsNew,
                    IconName = resource.IconName,
                    CreatedBy = resource.CreatedBy,
                    ResourceType = resource.ResourceType,
                    CreatedAt = resource.CreatedAt,
                    RecordStatus = Convert.ToInt32(resource.RecordStatus),
                    IsGhost = isGhost
                };

                if (resource.ResourceType == (int)ResourceType.Page ||
                    resource.ResourceType == (int)ResourceType.GhostPage)
                {
                    resourceFilter.PageId = resource.Id.ToString();
                }

                if (resource.ResourceType == (int)ResourceType.Node ||
                    resource.ResourceType == (int)ResourceType.GhostNode)
                {
                    resourceFilter.NodeId = resource.Id.ToString();
                }

                resourceFilter.Id = null;

                return new ResponseGetObject
                {
                    Datos = resourceFilter,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                //var correlationId = Guid.NewGuid().ToString("N");
                //var diag = ExceptionDiagnostics.BuildDetail(ex, correlationId);

                return new ResponseGetObject
                {
                    Mensajes = new[]
                    {
                        new Message
                        {
                            Tipo        = TypeMessage.error.ToString(),
                            Descripcion = $"Unexpected error."
                        },
                    },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ResponsePost> DeleteResourceById(IdStringFilter resourceId, int resourceType)
        {
            try
            {
                #region Id Validator
                var validation = _helperProcessValidator.ValidateStringId(resourceId.Id);
                if (!validation.IsValid)
                {
                    return new ResponsePost
                    {
                        Mensajes = validation.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
                #endregion

                resourceId.Id = resourceId.Id;

                var deleteResult = _unitOfWork.ResourceRepository.UpdateCustom(
                    new Resource
                    {
                        Id = Convert.ToInt32(resourceId.Id),
                        RecordStatus = 0
                    },
                    r => r.RecordStatus
                );

                if (deleteResult.RowsAffected > 0)
                {
                    if (resourceType == (int)ResourceType.Node)
                    {
                        return new ResponsePost
                        {
                            Respuesta = deleteResult,
                            StatusCode = HttpStatusCode.OK,
                            Mensajes = new[]
                            {
                                new Message
                                {
                                    Tipo        = TypeMessage.information.ToString(),
                                    Descripcion = "The node was deleted."
                                }
                            }
                        };
                    }

                    return new ResponsePost
                    {
                        Respuesta = deleteResult,
                        StatusCode = HttpStatusCode.OK,
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo        = TypeMessage.information.ToString(),
                                Descripcion = "The page was deleted."
                            }
                        }
                    };
                }

                return new ResponsePost
                {
                    Respuesta = deleteResult,
                    StatusCode = HttpStatusCode.NoContent
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> GetResourceList(GetResourcesQueryFilter queryFilter)
        {
            try
            {
                #region Get Resources Validator
                var validation = _authorizationRepositoryValidator.ValidateGetResources(queryFilter);
                if (!validation.IsValid)
                {
                    return new ResponseGet
                    {
                        Mensajes = validation.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
                #endregion

                queryFilter.PageNumber =
                    queryFilter.PageNumber == 0
                        ? _paginationOptions.InitialPageNumber
                        : queryFilter.PageNumber;

                queryFilter.PageSize =
                    queryFilter.PageSize == 0
                        ? _paginationOptions.InitialPageSize
                        : queryFilter.PageSize;

                var resources = await _authorizationRepository.GetResources();
                queryFilter.ApplicationId = queryFilter.ApplicationId;

                resources = FilterResources(resources, queryFilter);

                if (!resources.Any())
                {
                    return new ResponseGet
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo        = TypeMessage.information.ToString(),
                                Descripcion = "No data found for the selected filters."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                foreach (ResourceQueryFilter resource in resources)
                {
                    resource.ApplicationId = resource.ApplicationId;
                    resource.Id = resource.Id;
                    resource.ResourceId = resource.Id;
                    resource.IconName = resource.IconName;
                    resource.Page = resource.Page;
                    resource.IsGhost = resource.IsGhost;
                    resource.SubLinks = new ArrayList();
                    resource.Id = null;
                }

                Pagination paginationData = null;
                IEnumerable<object> dtoList = null;

                var pagedList =
                    resources.Count() > queryFilter.PageSize
                        ? PaginatedList<ResourceQueryFilter>.Paginate(
                            resources,
                            queryFilter.PageNumber,
                            queryFilter.PageSize,
                            out paginationData)
                        : resources;

                if (!string.IsNullOrEmpty(queryFilter.ResultType) &&
                    queryFilter.ResultType.Equals("Resource", StringComparison.OrdinalIgnoreCase))
                {
                    dtoList = pagedList;
                }
                else
                {
                    dtoList = pagedList.Select((r, index) =>
                    {
                        var cells = new List<ColumnCellFormat>();
                        var rowNumber = (queryFilter.PageNumber - 1) *
                                        queryFilter.PageSize + index + 1;

                        cells.Add(new ColumnCellFormat
                        {
                            ColumnName = "No.",
                            CellContent = new[]
                            {
                                new RowModel
                                {
                                    TipoContenido = TipoContenidoRowModel.Texto,
                                    Contenido     = $"{rowNumber}"
                                }
                            }
                        });

                        cells.Add(new ColumnCellFormat
                        {
                            ColumnName = "Page",
                            CellContent = new[]
                            {
                                new RowModel
                                {
                                    TipoContenido = TipoContenidoRowModel.Texto,
                                    Contenido     = $"{r.Page}"
                                }
                            }
                        });

                        cells.Add(new ColumnCellFormat
                        {
                            ColumnName = "Application",
                            CellContent = new[]
                            {
                                new RowModel
                                {
                                    TipoContenido = TipoContenidoRowModel.Texto,
                                    Contenido     = $"{r.ApplicationName}"
                                }
                            }
                        });

                        cells.Add(new ColumnCellFormat
                        {
                            ColumnName = "Description",
                            CellContent = new[]
                            {
                                new RowModel
                                {
                                    TipoContenido = TipoContenidoRowModel.Texto,
                                    Contenido     = $"{r.Description}"
                                }
                            }
                        });

                        cells.Add(new ColumnCellFormat
                        {
                            ColumnName = "ResourceType",
                            CellContent = new[]
                            {
                                new RowModel
                                {
                                    TipoContenido = TipoContenidoRowModel.Texto,
                                    Contenido     = $"{r.ResourceType}"
                                }
                            }
                        });

                        cells.Add(new ColumnCellFormat
                        {
                            ColumnName = "Actions",
                            CellContent = new[]
                            {
                                new RowModel
                                {
                                    TipoContenido = TipoContenidoRowModel.Texto,
                                    Contenido     = "Edit",
                                    Accion        = TipoAccionRowModel.Eliminar,
                                    Parametros    = new[]
                                    {
                                        new ParameterModel
                                        {
                                            NombreParametro = "NodeId",
                                            ValorParametro  = r.NodeId.ToString()
                                        }
                                    }
                                },
                                new RowModel
                                {
                                    TipoContenido = TipoContenidoRowModel.Texto,
                                    Contenido     = "Delete",
                                    Accion        = TipoAccionRowModel.Eliminar,
                                    Parametros    = new[]
                                    {
                                        new ParameterModel
                                        {
                                            NombreParametro = "NodeId",
                                            ValorParametro  = r.NodeId.ToString()
                                        }
                                    }
                                }
                            }
                        });

                        return cells;
                    });
                }

                if (paginationData is not null)
                {
                    return new ResponseGetPagination
                    {
                        Paginacion = new PaginatedList<object>(dtoList, paginationData),
                        StatusCode = HttpStatusCode.OK
                    };
                }

                return new ResponseGetObject
                {
                    Datos = dtoList,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ResponseGetObject
                {
                    Mensajes = new[]
                    {
                        new Message
                        {
                            Tipo        = TypeMessage.error.ToString(),
                            Descripcion = $"Unexpected error ."
                        },
                    },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ResponseGet> GetResources(GetResourcesQueryFilter queryFilter)
        {
            try
            {
                #region Get Resources Validator
                var validation = _authorizationRepositoryValidator.ValidateGetResources(queryFilter);
                if (!validation.IsValid)
                {
                    return new ResponseGet
                    {
                        Mensajes = validation.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
                #endregion

                var resources = await _authorizationRepository.GetResources();
                queryFilter.ApplicationId = queryFilter.ApplicationId;

                resources = FilterResources(resources, queryFilter);

                if (!resources.Any())
                {
                    return new ResponseGet
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo        = TypeMessage.information.ToString(),
                                Descripcion = "No resources found."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                var comboList = new List<RowModelCmb>();
                var index = 0;

                foreach (var resource in resources)
                {
                    comboList.Add(
                        new RowModelCmb
                        {
                            Valor = resource.Id.ToString(),
                            Descripcion = resource.Description,
                            EstaSeleccionado = (index == 0)
                        });

                    index++;
                }

                return new ResponseGet
                {
                    Datos = comboList,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region Filters
        public static IEnumerable<ResourceQueryFilter> FilterResources(
            IEnumerable<ResourceQueryFilter> resources,
            GetResourcesQueryFilter queryFilter)
        {
            IEnumerable<ResourceQueryFilter> filteredResources = resources;

            try
            {
                try
                {
                    filteredResources = filteredResources
                        .Where(x =>
                            x.ResourceType != 0 &&
                            x.ResourceType.Equals(queryFilter.ResourceType))
                        .ToList();
                }
                catch (NullReferenceException)
                {
                    filteredResources = Enumerable.Empty<ResourceQueryFilter>();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                if (!string.IsNullOrEmpty(queryFilter.ModuleId))
                {
                    try
                    {
                        filteredResources = filteredResources
                            .Where(x =>
                                x.ModuleId != null &&
                                x.ModuleId.Contains(queryFilter.ModuleId,
                                    StringComparison.CurrentCultureIgnoreCase))
                            .ToList();
                    }
                    catch (NullReferenceException)
                    {
                        filteredResources = Enumerable.Empty<ResourceQueryFilter>();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }

                if (!string.IsNullOrEmpty(queryFilter.ApplicationId))
                {
                    try
                    {
                        filteredResources = filteredResources
                            .Where(x =>
                                x.ApplicationId != null &&
                                x.ApplicationId.Contains(queryFilter.ApplicationId,
                                    StringComparison.CurrentCultureIgnoreCase))
                            .ToList();
                    }
                    catch (NullReferenceException)
                    {
                        filteredResources = Enumerable.Empty<ResourceQueryFilter>();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }

                if (!string.IsNullOrEmpty(queryFilter.Description))
                {
                    try
                    {
                        filteredResources = filteredResources
                            .Where(x =>
                                x.Description != null &&
                                x.Description.Contains(queryFilter.Description,
                                    StringComparison.CurrentCultureIgnoreCase))
                            .ToList();
                    }
                    catch (NullReferenceException)
                    {
                        filteredResources = Enumerable.Empty<ResourceQueryFilter>();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }

                if (!string.IsNullOrEmpty(queryFilter.Detail))
                {
                    try
                    {
                        filteredResources = filteredResources
                            .Where(x =>
                                x.Detail != null &&
                                x.Detail.Contains(queryFilter.Detail,
                                    StringComparison.CurrentCultureIgnoreCase))
                            .ToList();
                    }
                    catch (NullReferenceException)
                    {
                        filteredResources = Enumerable.Empty<ResourceQueryFilter>();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }

                return filteredResources;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #endregion
    }
}
