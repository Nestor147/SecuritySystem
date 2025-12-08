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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Application.Services.Authorization
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOptions _paginationOptions;
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly IAuthorizationRepositoryValidator _authorizationRepositoryVal;
        private readonly IHelperProcessVal _helperProcessVal;

        public RoleService(
            IOptions<PaginationOptions> options,
            IAuthorizationRepository authorizationRepository,
            IAuthorizationRepositoryValidator authorizationRepositoryVal,
            IHelperProcessVal helperProcessVal,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
            _authorizationRepository = authorizationRepository;
            _helperProcessVal = helperProcessVal;
            _authorizationRepositoryVal = authorizationRepositoryVal;
        }

        public static IEnumerable<RoleQueryFilter> FilterRoles(
            IEnumerable<RoleQueryFilter> roles,
            GetRoleQueryFilter getRoleQueryFilter)
        {
            IEnumerable<RoleQueryFilter> filteredRoles = roles;
            try
            {
                // If in the future you use RegionId again, adapt it here

                if (!string.IsNullOrEmpty(getRoleQueryFilter.ApplicationId))
                {
                    try
                    {
                        filteredRoles = filteredRoles
                            .Where(x =>
                                x.ApplicationId != null &&
                                x.ApplicationId.Equals(
                                    getRoleQueryFilter.ApplicationId,
                                    StringComparison.CurrentCultureIgnoreCase))
                            .ToList();
                    }
                    catch (NullReferenceException)
                    {
                        filteredRoles = Enumerable.Empty<RoleQueryFilter>();
                    }
                    catch (Exception err)
                    {
                        throw new Exception(err.Message);
                    }
                }

                if (!string.IsNullOrEmpty(getRoleQueryFilter.Description))
                {
                    try
                    {
                        filteredRoles = filteredRoles
                            .Where(x =>
                                x.Description != null &&
                                x.Description.Contains(
                                    getRoleQueryFilter.Description,
                                    StringComparison.CurrentCultureIgnoreCase))
                            .ToList();
                    }
                    catch (NullReferenceException)
                    {
                        filteredRoles = Enumerable.Empty<RoleQueryFilter>();
                    }
                    catch (Exception err)
                    {
                        throw new Exception(err.Message);
                    }
                }

                if (!string.IsNullOrEmpty(getRoleQueryFilter.Name))
                {
                    try
                    {
                        filteredRoles = filteredRoles
                            .Where(x =>
                                x.Name != null &&
                                x.Name.Contains(
                                    getRoleQueryFilter.Name,
                                    StringComparison.CurrentCultureIgnoreCase))
                            .ToList();
                    }
                    catch (NullReferenceException)
                    {
                        filteredRoles = Enumerable.Empty<RoleQueryFilter>();
                    }
                    catch (Exception err)
                    {
                        throw new Exception(err.Message);
                    }
                }

                return filteredRoles;
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        #region Methods V2

        public async Task<ResponsePost> InsertRole(RoleQueryFilter queryFilter, string token)
        {
            try
            {
                // #region Token Decrypt (kept commented)
                // string registeredUser = string.Empty;
                // TokenResponse tokenResponse;
                // var resToken = _procesosAuthenticationService.DesencriptarToken(token);
                // if (resToken.StatusCode == HttpStatusCode.OK)
                // {
                //     tokenResponse = (TokenResponse)resToken.Datos;
                //     registeredUser = _encryptionHelper.DecryptParameterId(tokenResponse.Usuario);
                //     queryFilter.RegisteredUser = registeredUser;
                // }
                // else
                // {
                //     return new ResponsePost()
                //     {
                //         Mensajes = new[]
                //         {
                //             new Message
                //             {
                //                 Tipo = TypeMessage.warning.ToString(),
                //                 Descripcion = "Invalid token."
                //             }
                //         },
                //         StatusCode = HttpStatusCode.BadRequest
                //     };
                // }
                // #endregion

                #region Role Validator

                var responseModel = _authorizationRepositoryVal.ValidateCreateRole(queryFilter);
                if (!responseModel.IsValid)
                {
                    return new ResponsePost()
                    {
                        Mensajes = responseModel.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                #region Business Rules

                //queryFilter.ApplicationId = _encryptionHelper.DecryptParameterId(queryFilter.ApplicationId);
                //queryFilter.RegisteredUser = _encryptionHelper.DecryptParameterId(queryFilter.RegisteredUser);
                //queryFilter.ApplicationId = queryFilter.ApplicationId;
                //queryFilter.RegisteredUser = queryFilter.RegisteredUser;

                var duplicated = await _unitOfWork.RoleRepository.GetByCustomQuery(b =>
                    b.Where(a =>
                            (a.ApplicationId.ToString() == queryFilter.ApplicationId &&
                             a.Name == queryFilter.Name) ||
                            (a.ApplicationId.ToString() == queryFilter.ApplicationId &&
                             a.Description == queryFilter.Description)
                            && a.RecordStatus == 1)
                     .ToList());

                if (duplicated.Any())
                {
                    return new ResponsePost()
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.warning.ToString(),
                                Descripcion = "A role with this name and description is already registered in this application."
                            }
                        },
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                var record = _unitOfWork.RoleRepository.Insert(new Role
                {
                    ApplicationId = Convert.ToInt32(queryFilter.ApplicationId),
                    Name = queryFilter.Name,
                    Description = queryFilter.Description,
                    CreatedBy= queryFilter.CreatedBy.ToUpper()
                });

                if (record.RowsAffected > 0)
                {
                    return new ResponsePost()
                    {
                        Respuesta = record,
                        StatusCode = HttpStatusCode.OK,
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "The role was successfully created."
                            }
                        }
                    };
                }
                else
                {
                    return new ResponsePost()
                    {
                        Respuesta = record,
                        StatusCode = HttpStatusCode.NoContent
                    };
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<ResponsePost> UpdateRole(RoleQueryFilter roleQueryFilter, string token)
        {
            try
            {
                #region Token Decrypt (kept commented)

                string registeredUser = string.Empty;
                // TokenResponse tokenResponse;
                // var resToken = _procesosAuthenticationService.DesencriptarToken(token);
                // if (resToken.StatusCode == HttpStatusCode.OK)
                // {
                //     tokenResponse = (TokenResponse)resToken.Datos;
                //     registeredUser = _encryptionHelper.DecryptParameterId(tokenResponse.Usuario);
                //     roleQueryFilter.RegisteredUser = registeredUser;
                // }
                // else
                // {
                //     return new ResponsePost()
                //     {
                //         Mensajes = new[]
                //         {
                //             new Message
                //             {
                //                 Tipo = TypeMessage.warning.ToString(),
                //                 Descripcion = "Invalid token."
                //             }
                //         },
                //         StatusCode = HttpStatusCode.BadRequest
                //     };
                // }

                #endregion

                #region Role Validator

                var responseModel = _authorizationRepositoryVal.ValidateUpdateRole(roleQueryFilter);
                if (!responseModel.IsValid)
                {
                    return new ResponsePost()
                    {
                        Mensajes = responseModel.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                #region Business Rules

                //roleQueryFilter.Id = _encryptionHelper.DecryptParameterId(roleQueryFilter.Id);
                //roleQueryFilter.ApplicationId = _encryptionHelper.DecryptParameterId(roleQueryFilter.ApplicationId);
                //roleQueryFilter.RegisteredUser = _encryptionHelper.DecryptParameterId(roleQueryFilter.RegisteredUser);
                //roleQueryFilter.Id = roleQueryFilter.Id;
                //roleQueryFilter.ApplicationId = roleQueryFilter.ApplicationId;
                //roleQueryFilter.RegisteredUser = roleQueryFilter.RegisteredUser;

                var duplicated = await _authorizationRepository.GetDuplicateRoles(roleQueryFilter);

                if (duplicated.Any())
                {
                    return new ResponsePost()
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.warning.ToString(),
                                Descripcion = "A role with this name and description is already registered in this application."
                            }
                        },
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                var role = _unitOfWork.RoleRepository.UpdateCustom(
                    new Role
                    {
                        Id = Convert.ToInt32(roleQueryFilter.Id),
                        ApplicationId = Convert.ToInt32(roleQueryFilter.ApplicationId),
                        Name = roleQueryFilter.Name,
                        Description = roleQueryFilter.Description,
                        CreatedAt = DateTime.Now,
                        CreatedBy = roleQueryFilter.CreatedBy.ToUpper()
                    },
                    b => b.ApplicationId,
                    c => c.Name,
                    d => d.Description,
                    e => e.CreatedAt,
                    f => f.CreatedBy);

                if (role.RowsAffected > 0)
                {
                    return new ResponsePost()
                    {
                        Respuesta = role,
                        StatusCode = HttpStatusCode.OK,
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "The role was successfully updated."
                            }
                        }
                    };
                }
                else
                {
                    return new ResponsePost()
                    {
                        Respuesta = role,
                        StatusCode = HttpStatusCode.NoContent
                    };
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<ResponseGetObject> GetRoleById(string roleId)
        {
            try
            {
                #region Id Validator

                var responseModel = _helperProcessVal.ValidateStringId(roleId);
                if (!responseModel.IsValid)
                {
                    return new ResponseGetObject()
                    {
                        Mensajes = responseModel.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                //roleId = _encryptionHelper.DecryptParameterId(roleId);
                roleId = roleId;

                var role = await _unitOfWork.RoleRepository.GetByIdAsync(Convert.ToInt32(roleId));

                if (role != null)
                {
                    var roleQueryFilter = new RoleQueryFilter()
                    {
                        //Id = _encryptionHelper.EncryptParameterId(role.Id.ToString()),
                        //ApplicationId = _encryptionHelper.EncryptParameterId(role.ApplicationId.ToString()),
                        Id = role.Id.ToString(),
                        ApplicationId = role.ApplicationId.ToString(),
                        Name = role.Name,
                        Description = role.Description
                    };

                    return new ResponseGetObject()
                    {
                        Datos = roleQueryFilter,
                        StatusCode = HttpStatusCode.OK
                    };
                }
                else
                {
                    return new ResponseGetObject()
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "Role not found."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<ResponsePost> DeleteRoleById(IdStringFilter roleId)
        {
            try
            {
                #region Id Validator

                var responseModel = _helperProcessVal.ValidateStringId(roleId.Id);
                if (!responseModel.IsValid)
                {
                    return new ResponsePost()
                    {
                        Mensajes = responseModel.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                //roleId.Id = _encryptionHelper.DecryptParameterId(roleId.Id);
                roleId.Id = roleId.Id;

                #region Business Rules

                var roleUsers = await _authorizationRepository.GetUserDataByRoleId(roleId.Id);
                if (roleUsers.Any())
                {
                    var usersList = roleUsers
                        .Select(ur => $"{ur.LastName1} {ur.LastName2} {ur.FirstName} - ID: {ur.DocumentNumber}")
                        .ToList();

                    if (usersList.Count < 5)
                    {
                        var usersMessage = string.Join(", ", usersList);
                        return new ResponsePost()
                        {
                            Mensajes = new[]
                            {
                                new Message
                                {
                                    Tipo = TypeMessage.warning.ToString(),
                                    Descripcion =
                                        $"The role cannot be deleted because there are {roleUsers.Count()} user(s) assigned to it. Users with this role: {usersMessage}"
                                }
                            },
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }
                    else
                    {
                        return new ResponsePost()
                        {
                            Mensajes = new[]
                            {
                                new Message
                                {
                                    Tipo = TypeMessage.warning.ToString(),
                                    Descripcion =
                                        $"The role cannot be deleted because there are {roleUsers.Count()} users assigned to it."
                                }
                            },
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }
                }

                #endregion

                var role = _unitOfWork.RoleRepository.UpdateCustom(
                    new Role
                    {
                        Id = Convert.ToInt32(roleId.Id),
                        RecordStatus = 0
                    },
                    a => a.RecordStatus);

                if (role.RowsAffected > 0)
                {
                    return new ResponsePost()
                    {
                        Respuesta = role,
                        StatusCode = HttpStatusCode.OK,
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "The role was deleted."
                            }
                        }
                    };
                }
                else
                {
                    return new ResponsePost()
                    {
                        Respuesta = role,
                        StatusCode = HttpStatusCode.NoContent
                    };
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<object> GetRoleList(GetRoleQueryFilter getRoleQueryFilter)
        {
            try
            {
                #region Roles Validator

                var responseModel = _authorizationRepositoryVal.ValidateGetRoles(getRoleQueryFilter);
                if (!responseModel.IsValid)
                {
                    return new ResponseGet()
                    {
                        Mensajes = responseModel.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                getRoleQueryFilter.PageNumber =
                    getRoleQueryFilter.PageNumber == 0
                        ? _paginationOptions.InitialPageNumber
                        : getRoleQueryFilter.PageNumber;

                getRoleQueryFilter.PageSize =
                    getRoleQueryFilter.PageSize == 0
                        ? _paginationOptions.InitialPageSize
                        : getRoleQueryFilter.PageSize;

                //getRoleQueryFilter.ApplicationId = _encryptionHelper.DecryptParameterId(getRoleQueryFilter.ApplicationId);
                getRoleQueryFilter.ApplicationId = getRoleQueryFilter.ApplicationId;

                var roles = await _authorizationRepository.GetRolesByApplication(getRoleQueryFilter.ApplicationId);
                roles = FilterRoles(roles, getRoleQueryFilter);

                if (roles.Any())
                {
                    Pagination pagination = null;
                    IEnumerable<object> responseList = null;

                    var pagedList = roles.Count() > getRoleQueryFilter.PageSize
                        ? PaginatedList<RoleQueryFilter>.Paginate(
                            roles,
                            getRoleQueryFilter.PageNumber,
                            getRoleQueryFilter.PageSize,
                            out pagination)
                        : roles;

                    if (getRoleQueryFilter.ResultType != null &&
                        getRoleQueryFilter.ResultType.ToLower() == "object")
                    {
                        foreach (var role in roles)
                        {
                            //role.ApplicationId = _encryptionHelper.EncryptParameterId(role.ApplicationId);
                            //role.Id = _encryptionHelper.EncryptParameterId(role.Id);
                            role.ApplicationId = role.ApplicationId;
                            role.Id = role.Id;
                        }

                        responseList = pagedList;
                    }
                    else
                    {
                        responseList = pagedList.Select((pp, index) =>
                        {
                            var tableCells = new List<ColumnCellFormat>();
                            var rowNumber = (getRoleQueryFilter.PageNumber - 1) * getRoleQueryFilter.PageSize + index + 1;

                            tableCells.Add(new ColumnCellFormat
                            {
                                ColumnName = "No.",
                                CellContent = new[]
                                {
                                    new RowModel
                                    {
                                        TipoContenido = TipoContenidoRowModel.Texto,
                                        Contenido = $"{index + 1}"
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
                                        Contenido = $"{pp.Name}"
                                    }
                                }
                            });

                            tableCells.Add(new ColumnCellFormat
                            {
                                ColumnName = "Description",
                                CellContent = new[]
                                {
                                    new RowModel
                                    {
                                        TipoContenido = TipoContenidoRowModel.Texto,
                                        Contenido = $"{pp.Description}"
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
                                        Accion = TipoAccionRowModel.Eliminar,
                                        Parametros = new[]
                                        {
                                            new ParameterModel
                                            {
                                                NombreParametro = "RoleId",
                                                ValorParametro = pp.Id.ToString()
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
                                                NombreParametro = "RoleId",
                                                ValorParametro = pp.Id.ToString()
                                            }
                                        }
                                    }
                                }
                            });

                            return tableCells;
                        });
                    }

                    if (pagination is not null)
                    {
                        return new ResponseGetPagination
                        {
                            Paginacion = new PaginatedList<object>(responseList, pagination),
                            StatusCode = HttpStatusCode.OK
                        };
                    }
                    else
                    {
                        return new ResponseGetObject()
                        {
                            Datos = responseList,
                            StatusCode = HttpStatusCode.OK
                        };
                    }
                }
                else
                {
                    return new ResponseGet()
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "No data found that matches the selected filters."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<ResponseGet> GetRolesCombo(GetRoleQueryFilter getRoleQueryFilter)
        {
            try
            {
                #region Get Roles Validator

                var responseModel = _authorizationRepositoryVal.ValidateGetRoles(getRoleQueryFilter);
                if (!responseModel.IsValid)
                {
                    return new ResponseGet()
                    {
                        Mensajes = responseModel.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                //getRoleQueryFilter.ApplicationId = _encryptionHelper.DecryptParameterId(getRoleQueryFilter.ApplicationId);
                getRoleQueryFilter.ApplicationId = getRoleQueryFilter.ApplicationId;

                var roles = await _authorizationRepository.GetRoles(getRoleQueryFilter);
                var responseGet = new ResponseGet();

                if (roles.Any())
                {
                    var cmbList = new List<RowModelCmb>();
                    var idx = 0;

                    foreach (var role in roles)
                    {
                        cmbList.Add(new RowModelCmb
                        {
                            //Valor = _encryptionHelper.EncryptParameterId(role.Id.ToString()),
                            Valor = role.Id.ToString(),
                            Descripcion = role.Name,
                            EstaSeleccionado = idx == 0
                        });
                        idx++;
                    }

                    responseGet = new ResponseGet()
                    {
                        Datos = cmbList,
                        StatusCode = HttpStatusCode.OK
                    };
                }
                else
                {
                    responseGet = new ResponseGet()
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "No roles were found."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                return responseGet;
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<object> GetRolesByUser(GetUsersByRoleQueryFilter getUsersByRoleQueryFilter)
        {
            try
            {
                #region Get Roles Validator

                var responseModel = _authorizationRepositoryVal.ValidateGetRolesByUser(getUsersByRoleQueryFilter);
                if (!responseModel.IsValid)
                {
                    return new ResponseGet()
                    {
                        Mensajes = responseModel.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                getUsersByRoleQueryFilter.PageNumber =
                    getUsersByRoleQueryFilter.PageNumber == 0
                        ? _paginationOptions.InitialPageNumber
                        : getUsersByRoleQueryFilter.PageNumber;

                getUsersByRoleQueryFilter.PageSize =
                    getUsersByRoleQueryFilter.PageSize == 0
                        ? _paginationOptions.InitialPageSize
                        : getUsersByRoleQueryFilter.PageSize;

                //getUsersByRoleQueryFilter.UserId = getUsersByRoleQueryFilter.UserId is not null ? _encryptionHelper.DecryptParameterId(getUsersByRoleQueryFilter.UserId) : null;
                getUsersByRoleQueryFilter.UserId =
                    getUsersByRoleQueryFilter.UserId is not null
                        ? getUsersByRoleQueryFilter.UserId
                        : null;

                var userRoles = await _authorizationRepository.GetUserRoles(getUsersByRoleQueryFilter);

                if (userRoles.Any())
                {
                    Pagination pagination = null;
                    IEnumerable<object> responseList = null;

                    var users = userRoles
                        .GroupBy(p => p.Id)
                        .Select(r => new UserRoleQueryFilter
                        {
                            UserId = r.First().UserId,
                            Id = r.First().Id,
                            ApplicationName = r.First().ApplicationName,
                            RoleName = r.First().RoleName,
                            CreatedBy = r.First().CreatedBy,
                            RecordStatus = r.First().RecordStatus
                        })
                        .ToList();

                    foreach (var role in users)
                    {
                        //role.UserId = _encryptionHelper.EncryptParameterId(role.UserId);
                        //role.Id = _encryptionHelper.EncryptParameterId(role.Id);

                        role.UserId = role.UserId;
                        role.Id = role.Id;
                    }

                    var pagedList = users.Count() > getUsersByRoleQueryFilter.PageSize
                        ? PaginatedList<UserRoleQueryFilter>.Paginate(
                            users,
                            getUsersByRoleQueryFilter.PageNumber,
                            getUsersByRoleQueryFilter.PageSize,
                            out pagination)
                        : users;

                    if (getUsersByRoleQueryFilter.ResultType != null &&
                        getUsersByRoleQueryFilter.ResultType.ToLower() == "object")
                    {
                        responseList = pagedList;
                    }
                    else
                    {
                        responseList = pagedList.Select((pp, index) =>
                        {
                            var tableCells = new List<ColumnCellFormat>();
                            var rowNumber = (getUsersByRoleQueryFilter.PageNumber - 1) *
                                            getUsersByRoleQueryFilter.PageSize + index + 1;

                            tableCells.Add(new ColumnCellFormat
                            {
                                ColumnName = "No.",
                                CellContent = new[]
                                {
                                    new RowModel
                                    {
                                        TipoContenido = TipoContenidoRowModel.Texto,
                                        Contenido = $"{index + 1}"
                                    }
                                }
                            });

                            tableCells.Add(new ColumnCellFormat
                            {
                                ColumnName = "Role",
                                CellContent = new[]
                                {
                                    new RowModel
                                    {
                                        TipoContenido = TipoContenidoRowModel.Texto,
                                        Contenido = $"{pp.RoleName}"
                                    }
                                }
                            });

                            tableCells.Add(new ColumnCellFormat
                            {
                                ColumnName = "Application",
                                CellContent = new[]
                                {
                                    new RowModel
                                    {
                                        TipoContenido = TipoContenidoRowModel.Texto,
                                        Contenido = $"{pp.ApplicationName}"
                                    }
                                }
                            });

                            return tableCells;
                        });
                    }

                    if (pagination is not null)
                    {
                        return new ResponseGetPagination
                        {
                            Paginacion = new PaginatedList<object>(responseList, pagination),
                            StatusCode = HttpStatusCode.OK
                        };
                    }
                    else
                    {
                        return new ResponseGetObject()
                        {
                            Datos = responseList,
                            StatusCode = HttpStatusCode.OK
                        };
                    }
                }
                else
                {
                    return new ResponseGet()
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "No data found that matches the selected filters."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<object> GetUsersByRole(GetUsersByRoleQueryFilter getUsersByRoleQueryFilter)
        {
            try
            {
                #region Get Users Validator

                var responseModel = _authorizationRepositoryVal.ValidateGetUsersByRole(getUsersByRoleQueryFilter);
                if (!responseModel.IsValid)
                {
                    return new ResponseGet()
                    {
                        Mensajes = responseModel.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                getUsersByRoleQueryFilter.PageNumber =
                    getUsersByRoleQueryFilter.PageNumber == 0
                        ? _paginationOptions.InitialPageNumber
                        : getUsersByRoleQueryFilter.PageNumber;

                getUsersByRoleQueryFilter.PageSize =
                    getUsersByRoleQueryFilter.PageSize == 0
                        ? _paginationOptions.InitialPageSize
                        : getUsersByRoleQueryFilter.PageSize;

                //getUsersByRoleQueryFilter.RoleId = _encryptionHelper.DecryptParameterId(getUsersByRoleQueryFilter.RoleId);
                getUsersByRoleQueryFilter.RoleId = getUsersByRoleQueryFilter.RoleId;

                var userRoles = await _authorizationRepository.GetUserRoles(getUsersByRoleQueryFilter);

                if (userRoles.Any())
                {
                    Pagination pagination = null;
                    IEnumerable<object> responseList = null;

                    var users = userRoles
                        .GroupBy(p => p.UserId)
                        .Select(r => new UserRoleQueryFilter
                        {
                            UserId = r.Key,
                            RoleId = r.First().RoleId,
                            Id = r.First().Id,
                            DocumentNumber = r.First().DocumentNumber,
                            LastNames = r.First().LastNames,
                            FirstNames = r.First().FirstNames,
                            CreatedBy = r.First().CreatedBy,
                            RecordStatus = r.First().RecordStatus
                        })
                        .ToList();

                    foreach (var role in users)
                    {
                        //role.Id = _encryptionHelper.EncryptParameterId(role.Id);
                        //role.RoleId = _encryptionHelper.EncryptParameterId(role.RoleId);
                        //role.UserId = _encryptionHelper.EncryptParameterId(role.UserId);

                        role.Id = role.Id;
                        role.RoleId = role.RoleId;
                        role.UserId = role.UserId;
                    }

                    var pagedList = users.Count() > getUsersByRoleQueryFilter.PageSize
                        ? PaginatedList<UserRoleQueryFilter>.Paginate(
                            users,
                            getUsersByRoleQueryFilter.PageNumber,
                            getUsersByRoleQueryFilter.PageSize,
                            out pagination)
                        : users;

                    if (getUsersByRoleQueryFilter.ResultType != null &&
                        getUsersByRoleQueryFilter.ResultType.ToLower() == "object")
                    {
                        responseList = pagedList;
                    }
                    else
                    {
                        responseList = pagedList.Select((pp, index) =>
                        {
                            var tableCells = new List<ColumnCellFormat>();
                            var rowNumber = (getUsersByRoleQueryFilter.PageNumber - 1) *
                                            getUsersByRoleQueryFilter.PageSize + index + 1;

                            tableCells.Add(new ColumnCellFormat
                            {
                                ColumnName = "No.",
                                CellContent = new[]
                                {
                                    new RowModel
                                    {
                                        TipoContenido = TipoContenidoRowModel.Texto,
                                        Contenido = $"{index + 1}"
                                    }
                                }
                            });

                            tableCells.Add(new ColumnCellFormat
                            {
                                ColumnName = "Identity document",
                                CellContent = new[]
                                {
                                    new RowModel
                                    {
                                        TipoContenido = TipoContenidoRowModel.Texto,
                                        Contenido = $"{pp.DocumentNumber}"
                                    }
                                }
                            });

                            tableCells.Add(new ColumnCellFormat
                            {
                                ColumnName = "Last name",
                                CellContent = new[]
                                {
                                    new RowModel
                                    {
                                        TipoContenido = TipoContenidoRowModel.Texto,
                                        Contenido = $"{pp.LastNames}"
                                    }
                                }
                            });

                            tableCells.Add(new ColumnCellFormat
                            {
                                ColumnName = "First name",
                                CellContent = new[]
                                {
                                    new RowModel
                                    {
                                        TipoContenido = TipoContenidoRowModel.Texto,
                                        Contenido = $"{pp.FirstNames}"
                                    }
                                }
                            });

                            return tableCells;
                        });
                    }

                    if (pagination is not null)
                    {
                        return new ResponseGetPagination
                        {
                            Paginacion = new PaginatedList<object>(responseList, pagination),
                            StatusCode = HttpStatusCode.OK
                        };
                    }
                    else
                    {
                        return new ResponseGetObject()
                        {
                            Datos = responseList,
                            StatusCode = HttpStatusCode.OK
                        };
                    }
                }
                else
                {
                    return new ResponseGet()
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "No data found that matches the selected filters."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<ResponsePost> DeleteUserRoleById(IdStringFilter userRoleId)
        {
            try
            {
                #region Id Validator

                var responseModel = _helperProcessVal.ValidateStringId(userRoleId.Id);
                if (!responseModel.IsValid)
                {
                    return new ResponsePost()
                    {
                        Mensajes = responseModel.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                #endregion

                //userRoleId.Id = _encryptionHelper.DecryptParameterId(userRoleId.Id);
                userRoleId.Id = userRoleId.Id;

                var userRole = _unitOfWork.RoleUserRepository.UpdateCustom(
                    new RoleUser()
                    {
                        Id = Convert.ToInt32(userRoleId.Id),
                        RecordStatus = 0
                    },
                    a => a.RecordStatus);

                if (userRole.RowsAffected > 0)
                {
                    return new ResponsePost()
                    {
                        Respuesta = userRole,
                        StatusCode = HttpStatusCode.OK,
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "The user-role assignment was deleted."
                            }
                        }
                    };
                }
                else
                {
                    return new ResponsePost()
                    {
                        Respuesta = userRole,
                        StatusCode = HttpStatusCode.NoContent
                    };
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<ResponseGetObject> GetRolesUsers(GetUserRoles queryFilter)
        {
            try
            {
                //queryFilter.Users = queryFilter.Users.Select(a => _encryptionHelper.DecryptParameterId(a));
                //queryFilter.Roles = queryFilter.Roles.Select(a => _encryptionHelper.DecryptParameterId(a));

                queryFilter.Users = queryFilter.Users.Select(a => a);
                queryFilter.Roles = queryFilter.Roles.Select(a => a);

                var usersArray = string.Join(",", queryFilter.Users);

                var userRoles =
                    await _authorizationRepository.GetRolesForMultipleUsers(usersArray);

                userRoles = userRoles.Where(a => a.RecordStatus == 1);

                if (userRoles.Any())
                {
                    userRoles = userRoles.Where(a =>
                        queryFilter.Roles.Any(b => b == a.RoleId.ToString()));

                    var users = userRoles
                        .GroupBy(a => a.UserId)
                        .Select(g => new
                        {
                            user = g.Key,
                            roles = g.Select(r => r.RoleId).ToList()
                        })
                        .ToList();

                    return new ResponseGetObject()
                    {
                        Datos = users,
                        StatusCode = HttpStatusCode.OK
                    };
                }
                else
                {
                    return new ResponseGetObject()
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "No data found that matches the selected filters."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<ResponsePost> InsertUserRoles(List<InsertUserRoleQueryFilter> queryFilter)
        {
            try
            {
                foreach (var item in queryFilter)
                {
                    var responseModel =
                        _authorizationRepositoryVal.ValidateInsertUserRoles(item);

                    if (!responseModel.IsValid)
                    {
                        return new ResponsePost
                        {
                            Mensajes = responseModel.ValidationMessages.ToArray(),
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }
                }

                foreach (var u in queryFilter)
                {
                    //u.UserId = _encryptionHelper.DecryptParameterId(u.UserId);
                    u.UserId = u.UserId;
                }

                var desired = queryFilter
                    .SelectMany(u => u.Roles.Select(r => new
                    {
                        UserId = u.UserId,
                        //RoleId = _encryptionHelper.DecryptParameterId(r.RoleId),
                        RoleId = r.RoleId,
                        IsSelected = r.IsSelected,
                        Inspector = r.IsInspector
                    }))
                    .Select(x => new
                    {
                        x.UserId,
                        x.RoleId,
                        x.IsSelected,
                        Inspector = x.IsSelected ? x.Inspector : false
                    })
                    .GroupBy(x => new { x.UserId, x.RoleId })
                    .Select(g => g
                        .OrderByDescending(z => z.IsSelected)
                        .ThenByDescending(z => z.Inspector)
                        .First())
                    .ToList();

                if (!desired.Any())
                {
                    return new ResponsePost
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "No data found to process."
                            }
                        },
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                var usersInsert = string.Join(",", desired.Select(a => a.UserId).Distinct());
                var existing = await _authorizationRepository.GetRolesForMultipleUsers(usersInsert);

                var existingMap = existing
                    .GroupBy(e => (UserId: e.UserId.ToString(), RoleId: e.RoleId.ToString()))
                    .ToDictionary(g => g.Key, g => g.First());

                var listUserInsert = new List<RoleUser>();
                var listUserUpdate = new List<RoleUser>();

                var now = DateTime.Now;

                foreach (var item in desired)
                {
                    var key = (UserId: item.UserId.ToString(), RoleId: item.RoleId.ToString());

                    if (!existingMap.TryGetValue(key, out var existingItem))
                    {
                        if (item.IsSelected)
                        {
                            listUserInsert.Add(new RoleUser
                            {
                                UserId = Convert.ToInt32(item.UserId),
                                RoleId = Convert.ToInt32(item.RoleId),
                                RecordStatus = 1,
                                IsInspector = item.Inspector ? 1: 0,
                                CreatedBy = "AUTHORIZATION",
                                CreatedAt = now
                            });
                        }
                    }
                    else
                    {
                        var newStatus = item.IsSelected;
                        var newInspector = item.IsSelected ? item.Inspector : false;

                        var currentStatus = existingItem.RecordStatus == 1;
                        bool.TryParse(existingItem.Inspector?.ToString(), out var currentInspector);

                        var changeStatus = currentStatus != newStatus;
                        var changeInspector = currentInspector != newInspector;

                        if (changeStatus || changeInspector)
                        {
                            listUserUpdate.Add(new RoleUser
                            {
                                Id = Convert.ToInt32(existingItem.Id),
                                UserId = Convert.ToInt32(existingItem.UserId),
                                RoleId = Convert.ToInt32(existingItem.RoleId),
                                RecordStatus = newStatus ? 1 : 0,
                                IsInspector = newInspector ? 1: 0,
                                CreatedBy = "AUTHORIZATION",
                                CreatedAt = now
                            });
                        }
                    }
                }

                ResponsePostDetail commitIns = new ResponsePostDetail();
                ResponsePostDetail commitUpd = new ResponsePostDetail();

                if (listUserInsert.Any())
                {
                    commitIns = _unitOfWork.RoleUserRepository.Insert(listUserInsert);
                }

                if (listUserUpdate.Any())
                {
                    commitUpd = _unitOfWork.RoleUserRepository.Update(listUserUpdate);
                }

                var rows =
                    (commitIns?.RowsAffected ?? 0) +
                    (commitUpd?.RowsAffected ?? 0);

                return new ResponsePost
                {
                    Respuesta = new ResponsePostDetail
                    {
                        Process =
                            $"USER-ROLES => INSERT: {commitIns?.RowsAffected ?? 0}, UPDATE: {commitUpd?.RowsAffected ?? 0}",
                        RowsAffected = rows
                    },
                    StatusCode = HttpStatusCode.OK,
                    Mensajes = new[]
                    {
                        new Message
                        {
                            Tipo = TypeMessage.information.ToString(),
                            Descripcion = "Role processing completed."
                        }
                    }
                };
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<ResponseGetObject> GetRolesForMultipleUsers(GetUserRoles queryFilter)
        {
            try
            {
                // If you go back to decryption:
                // queryFilter.Users = queryFilter.Users.Select(a => _encryptionHelper.DecryptParameterId(a));

                queryFilter.Users = queryFilter.Users.Select(a => a);
                var usersArray = string.Join(",", queryFilter.Users);

                var userRoles = await _authorizationRepository.GetRolesForMultipleUsers(usersArray);

                userRoles = userRoles.Where(a => a.RecordStatus == 1);

                if (!userRoles.Any())
                {
                    return new ResponseGetObject
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "No data found that matches the selected filters."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                var users = queryFilter.Users
                    .GroupJoin(
                        userRoles,
                        user => user,
                        role => role.UserId.ToString(),
                        (user, roles) => new
                        {
                            User = user,
                            Roles = roles
                                .GroupBy(r => r.RoleId)
                                .Select(g => new
                                {
                                    RoleId = g.Key,
                                    Inspector = g.Any(x =>
                                    {
                                        if (string.IsNullOrWhiteSpace(x.Inspector))
                                            return false;

                                        return bool.TryParse(x.Inspector.Trim(), out var parsed) && parsed;
                                    })
                                })
                                .ToList()
                        })
                    .ToList();

                return new ResponseGetObject
                {
                    Datos = users,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<ResponseGetObject> SearchUsersByDocOrName(SearchUsersQueryFilter query)
        {
            try
            {
                var searchCriteria = string.IsNullOrWhiteSpace(query.SearchCriteria)
                    ? null
                    : query.SearchCriteria;

                var rows = await _authorizationRepository.SearchUsersByCriteria(
                    searchCriteria,
                    onlyActive: query.OnlyActive,
                    top: query.Top);

                if (rows != null && rows.Any())
                {
                    var data = rows.Select(r => new
                    {
                        r.UserId,
                        r.DocumentNumber,
                        r.FullName,
                        r.SystemUserId
                    });

                    return new ResponseGetObject
                    {
                        Datos = data,
                        StatusCode = HttpStatusCode.OK
                    };
                }
                else
                {
                    return new ResponseGetObject
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = "No users were found that meet the search criteria."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        #endregion
    }
}
