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
using SecuritySystem.Core.QueryFilters.Autorization.Request;
using SecuritySystem.Core.QueryFilters.Helper;
using System.Net;

namespace SecuritySystem.Application.Services.Authorization
{
    public class ApplicationsService : IApplicationsService
    {
        #region Fields

        private readonly PaginationOptions _paginationOptions;
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly IAuthorizationRepositoryValidator _authorizationRepositoryVal;
        private readonly IHelperProcessVal _helperProcessVal;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SecureSettings _secureSettings;

        private List<RowModelCmb> _comboRowModelList;
        private ResponseGet _responseGet;
        private int _comboIndex;

        #endregion

        #region Constructor

        public ApplicationsService(
            IOptions<PaginationOptions> options,
            IAuthorizationRepository authorizationRepository,
            IAuthorizationRepositoryValidator authorizationRepositoryVal,
            IHelperProcessVal helperProcessVal,
            IOptions<SecureSettings> secureSettings,
            IUnitOfWork unitOfWork
        )
        {
            _paginationOptions = options.Value;
            _authorizationRepository = authorizationRepository;
            _authorizationRepositoryVal = authorizationRepositoryVal;
            _helperProcessVal = helperProcessVal;
            _secureSettings = secureSettings.Value;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        public async Task<ResponsePost> CreateApplication(ApplicationQueryFilter applicationQueryFilter, string token)
        {
            try
            {
                #region Validator

                var responseModel = _authorizationRepositoryVal.ValidateCreateApplication(applicationQueryFilter);
                if (!responseModel.IsValid)
                {
                    return new ResponsePost
                    {
                        Mensajes = responseModel.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                #region Business Rules

                var duplicatedApplication = await _authorizationRepository.GetDuplicateApplications(applicationQueryFilter);

                if (duplicatedApplication.Any())
                {
                    return new ResponsePost
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.warning.ToString(),
                                Descripcion = "An application with this code or name is already registered."
                            }
                        },
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                var application = _unitOfWork.ApplicationRepository.Insert(new Applications
                {
                    Code = applicationQueryFilter.Code,
                    Description = applicationQueryFilter.Description,
                    Url = applicationQueryFilter.Url,
                    Icon = applicationQueryFilter.Icon,
                    CreatedBy = "SECURITY_SYSTEM"
                });

                if (application.RowsAffected > 0)
                {
                    return new ResponsePost
                    {
                        StatusCode = HttpStatusCode.OK,
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "The application was created successfully."
                            }
                        }
                    };
                }

                return new ResponsePost
                {
                    StatusCode = HttpStatusCode.NoContent
                };
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<ResponsePost> UpdateApplication(ApplicationQueryFilter applicationQueryFilter, string token)
        {
            try
            {
                #region Validator

                var responseModel = _authorizationRepositoryVal.ValidateUpdateApplication(applicationQueryFilter);
                if (!responseModel.IsValid)
                {
                    return new ResponsePost
                    {
                        Mensajes = responseModel.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                #region Business Rules

                var duplicatedApplication = await _authorizationRepository.GetDuplicateApplications(applicationQueryFilter);

                if (duplicatedApplication.Any())
                {
                    return new ResponsePost
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.warning.ToString(),
                                Descripcion = "An application with this code and name is already registered."
                            }
                        },
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                var application = _unitOfWork.ApplicationRepository.UpdateCustom(
                    new Applications
                    {
                        Id = Convert.ToInt32(applicationQueryFilter.Id),
                        Code = applicationQueryFilter.Code,
                        Description = applicationQueryFilter.Description,
                        Url = applicationQueryFilter.Url,
                        Icon = applicationQueryFilter.Icon,
                        CreatedBy = string.IsNullOrWhiteSpace(applicationQueryFilter.CreatedBy)
                            ? "SECURITY_SYSTEM"
                            : applicationQueryFilter.CreatedBy,
                        CreatedAt = DateTime.UtcNow
                    },
                    a => a.Code,
                    b => b.Description,
                    c => c.Url,
                    d => d.Icon,
                    e => e.CreatedAt,
                    f => f.CreatedBy
                );

                if (application.RowsAffected > 0)
                {
                    return new ResponsePost
                    {
                        StatusCode = HttpStatusCode.OK,
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "The application was updated successfully."
                            }
                        }
                    };
                }

                return new ResponsePost
                {
                    StatusCode = HttpStatusCode.NoContent
                };
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<ResponsePost> DeleteApplicationById(IdStringFilter idFilter, string token)
        {
            try
            {
                #region Id Validator

                var responseModel = _helperProcessVal.ValidateStringId(idFilter.Id);
                if (!responseModel.IsValid)
                {
                    return new ResponsePost
                    {
                        Mensajes = responseModel.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                var application = _unitOfWork.ApplicationRepository.UpdateCustom(
                    new Applications
                    {
                        Id = Convert.ToInt32(idFilter.Id),
                        RecordStatus = 0
                    },
                    a => a.RecordStatus
                );

                if (application.RowsAffected > 0)
                {
                    return new ResponsePost
                    {
                        StatusCode = HttpStatusCode.OK,
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "The application was deleted."
                            }
                        }
                    };
                }

                return new ResponsePost
                {
                    StatusCode = HttpStatusCode.NoContent
                };
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<ResponseGetObject> GetApplicationById(string applicationId)
        {
            try
            {
                #region Id Validator

                var responseModel = _helperProcessVal.ValidateStringId(applicationId);
                if (!responseModel.IsValid)
                {
                    return new ResponseGetObject
                    {
                        Mensajes = responseModel.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                var application = _unitOfWork.ApplicationRepository.GetById(Convert.ToInt32(applicationId));

                if (application != null)
                {
                    var dto = new ApplicationQueryFilter
                    {
                        Id = application.Id.ToString(),
                        Code = application.Code,
                        Description = application.Description,
                        Url = application.Url,
                        Icon = application.Icon,
                        RecordStatus = application.RecordStatus,
                        CreatedAt = application.CreatedAt,
                        CreatedBy = application.CreatedBy
                    };

                    return new ResponseGetObject
                    {
                        Datos = dto,
                        StatusCode = HttpStatusCode.OK
                    };
                }

                return new ResponseGetObject
                {
                    Mensajes = new[]
                    {
                        new Message
                        {
                            Tipo = TypeMessage.information.ToString(),
                            Descripcion = "No application was found."
                        }
                    },
                    StatusCode = HttpStatusCode.NotFound
                };
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<object> GetApplicationList(GetApplicationsQueryFilter getApplicationsQueryFilter)
        {
            try
            {
                getApplicationsQueryFilter.PageNumber =
                    getApplicationsQueryFilter.PageNumber == 0
                        ? _paginationOptions.InitialPageNumber
                        : getApplicationsQueryFilter.PageNumber;

                getApplicationsQueryFilter.PageSize =
                    getApplicationsQueryFilter.PageSize == 0
                        ? _paginationOptions.InitialPageSize
                        : getApplicationsQueryFilter.PageSize;

                var applications = await _authorizationRepository.GetApplications();
                applications = FilterApplications(applications, getApplicationsQueryFilter);

                if (!applications.Any())
                {
                    return new ResponseGet
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "No data found for the selected filters."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                Pagination paginationData = null;
                IEnumerable<object> dtoListResponse = null;

                var paginatedList = applications.Count() > getApplicationsQueryFilter.PageSize? PaginatedList<ApplicationQueryFilter>.Paginate(
                        applications,
                        getApplicationsQueryFilter.PageNumber,
                        getApplicationsQueryFilter.PageSize,
                        out paginationData
                    )
                    : applications;

                if (!string.IsNullOrEmpty(getApplicationsQueryFilter.ResultType) &&
                    getApplicationsQueryFilter.ResultType.Equals("Object",
                        StringComparison.OrdinalIgnoreCase))
                {
                    dtoListResponse = paginatedList;
                }
                else
                {
                    dtoListResponse = paginatedList.Select((app, index) =>
                    {
                        var tableCells = new List<ColumnCellFormat>();
                        var rowNumber = (getApplicationsQueryFilter.PageNumber - 1) *
                                        getApplicationsQueryFilter.PageSize + index + 1;

                        tableCells.Add(new ColumnCellFormat
                        {
                            ColumnName = "No.",
                            CellContent = new[]
                            {
                                new RowModel
                                {
                                    TipoContenido = TipoContenidoRowModel.Texto,
                                    Contenido = $"{rowNumber}"
                                }
                            }
                        });

                        tableCells.Add(new ColumnCellFormat
                        {
                            ColumnName = "Code",
                            CellContent = new[]
                            {
                                new RowModel
                                {
                                    TipoContenido = TipoContenidoRowModel.Texto,
                                    Contenido = $"{app.Code}"
                                }
                            }
                        });

                        tableCells.Add(new ColumnCellFormat
                        {
                            ColumnName = "Name",
                            CellContent = new[]
                            {
                                new RowModel
                                {
                                    TipoContenido = TipoContenidoRowModel.Texto,
                                    Contenido = $"{app.Description}"
                                }
                            }
                        });

                        tableCells.Add(new ColumnCellFormat
                        {
                            ColumnName = "Url",
                            CellContent = new[]
                            {
                                new RowModel
                                {
                                    TipoContenido = TipoContenidoRowModel.Texto,
                                    Contenido = $"{app.Url}"
                                }
                            }
                        });

                        tableCells.Add(new ColumnCellFormat
                        {
                            ColumnName = "Icon",
                            CellContent = new[]
                            {
                                new RowModel
                                {
                                    TipoContenido = TipoContenidoRowModel.Texto,
                                    Contenido = $"{app.Icon}"
                                }
                            }
                        });

                        tableCells.Add(new ColumnCellFormat
                        {
                            ColumnName = "Actions",
                            CellContent = new[]
                            {
                                new RowModel
                                {
                                    TipoContenido = TipoContenidoRowModel.Texto,
                                    Contenido = "Edit",
                                    Accion = TipoAccionRowModel.Editar,
                                    Parametros = new[]
                                    {
                                        new ParameterModel
                                        {
                                            NombreParametro = "Id",
                                            ValorParametro = app.Id.ToString()
                                        }
                                    }
                                },
                                new RowModel
                                {
                                    TipoContenido = TipoContenidoRowModel.Texto,
                                    Contenido = "Delete",
                                    Accion = TipoAccionRowModel.Eliminar,
                                    Parametros = new[]
                                    {
                                        new ParameterModel
                                        {
                                            NombreParametro = "Id",
                                            ValorParametro = app.Id.ToString()
                                        }
                                    }
                                }
                            }
                        });

                        return tableCells;
                    });
                }

                if (paginationData is not null)
                {
                    return new ResponseGetPagination
                    {
                        Paginacion = new PaginatedList<object>(dtoListResponse, paginationData),
                        StatusCode = HttpStatusCode.OK
                    };
                }

                return new ResponseGetObject
                {
                    Datos = dtoListResponse,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<ResponseGet> GetApplications(GetApplicationsQueryFilter getApplicationsQueryFilter)
        {
            try
            {
                var applications = await _authorizationRepository.GetApplications();

                if (applications.Any())
                {
                    _comboRowModelList = new List<RowModelCmb>();
                    _comboIndex = 0;

                    foreach (var app in applications)
                    {
                        _comboRowModelList.Add(new RowModelCmb
                        {
                            Value = app.Id.ToString(),
                            Description = app.Description,
                            IsSelected = _comboIndex == 0
                        });

                        _comboIndex++;
                    }

                    _responseGet = new ResponseGet
                    {
                        Datos = _comboRowModelList,
                        StatusCode = HttpStatusCode.OK
                    };
                }
                else
                {
                    _responseGet = new ResponseGet
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "No applications were found."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                return _responseGet;
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<ResponseGet> GetApplicationsByUser(string token)
        {
            try
            {
                #region Validate input

                if (string.IsNullOrEmpty(token))
                {
                    return new ResponseGet
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.warning.ToString(),
                                Descripcion = "The token parameter is required."
                            }
                        },
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                // NOTE: here you should decrypt/validate the token and extract the user id.
                // For now, this is left as a placeholder.
                var key = string.Empty;
                var parts = key.Split('|');

                int userId = 0;
                if (parts.Length >= 6)
                {
                    userId = Convert.ToInt32(parts[5]);
                }

                var request = new ApplicationsByUserRequest
                {
                    UserId = userId.ToString()
                };

                var applications = await _authorizationRepository.GetApplicationsByUserId(request.UserId);
                applications = FilterUserApplications(applications, request);

                if (applications.Any())
                {
                    foreach (var app in applications)
                    {
                        app.Id = app.Id;
                        app.UserId = app.UserId;
                    }

                    return new ResponseGet
                    {
                        Datos = applications,
                        StatusCode = HttpStatusCode.OK
                    };
                }

                return new ResponseGet
                {
                    Mensajes = new[]
                    {
                        new Message
                        {
                            Tipo = TypeMessage.information.ToString(),
                            Descripcion = "No data found for the selected filters."
                        }
                    },
                    StatusCode = HttpStatusCode.NotFound
                };
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        #endregion

        #region Filters

        public static IEnumerable<ApplicationQueryFilter> FilterApplications(
            IEnumerable<ApplicationQueryFilter> applications,
            GetApplicationsQueryFilter filter)
        {
            IEnumerable<ApplicationQueryFilter> filteredApplications = applications;

            try
            {
                if (!string.IsNullOrEmpty(filter.Description))
                {
                    try
                    {
                        filteredApplications = filteredApplications
                            .Where(x =>
                                x.Description != null &&
                                x.Description.Contains(filter.Description, StringComparison.CurrentCultureIgnoreCase))
                            .ToList();
                    }
                    catch (NullReferenceException)
                    {
                        filteredApplications = Enumerable.Empty<ApplicationQueryFilter>();
                    }
                }

                if (!string.IsNullOrEmpty(filter.Code))
                {
                    try
                    {
                        filteredApplications = filteredApplications
                            .Where(x =>
                                x.Code != null &&
                                x.Code.Contains(filter.Code, StringComparison.CurrentCultureIgnoreCase))
                            .ToList();
                    }
                    catch (NullReferenceException)
                    {
                        filteredApplications = Enumerable.Empty<ApplicationQueryFilter>();
                    }
                }

                return filteredApplications;
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public static IEnumerable<ApplicationsByUserRequest> FilterUserApplications(
            IEnumerable<ApplicationsByUserRequest> userApplications,
            ApplicationsByUserRequest filter)
        {
            IEnumerable<ApplicationsByUserRequest> filteredUserApplications = userApplications;

            try
            {
                if (!string.IsNullOrEmpty(filter.UserId))
                {
                    try
                    {
                        filteredUserApplications = filteredUserApplications
                            .Where(x =>
                                x.UserId != null &&
                                x.UserId.Equals(filter.UserId, StringComparison.CurrentCultureIgnoreCase))
                            .ToList();
                    }
                    catch (NullReferenceException)
                    {
                        filteredUserApplications = Enumerable.Empty<ApplicationsByUserRequest>();
                    }
                }

                if (!string.IsNullOrEmpty(filter.ApplicationId))
                {
                    try
                    {
                        filteredUserApplications = filteredUserApplications
                            .Where(x =>
                                x.ApplicationId != null &&
                                x.ApplicationId.Equals(filter.ApplicationId,
                                    StringComparison.CurrentCultureIgnoreCase))
                            .ToList();
                    }
                    catch (NullReferenceException)
                    {
                        filteredUserApplications = Enumerable.Empty<ApplicationsByUserRequest>();
                    }
                }

                if (!string.IsNullOrEmpty(filter.ApplicationName))
                {
                    try
                    {
                        filteredUserApplications = filteredUserApplications
                            .Where(x =>
                                x.ApplicationName != null &&
                                x.ApplicationName.Contains(filter.ApplicationName,
                                    StringComparison.CurrentCultureIgnoreCase))
                            .ToList();
                    }
                    catch (NullReferenceException)
                    {
                        filteredUserApplications = Enumerable.Empty<ApplicationsByUserRequest>();
                    }
                }

                return filteredUserApplications;
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        #endregion
    }
}
