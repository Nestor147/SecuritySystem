using Microsoft.Extensions.Options;
using SecuritySystem.Application.Interfaces.Authorization;
using SecuritySystem.Core.Entities;
using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi;
using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;
using SecuritySystem.Core.Enums;
using SecuritySystem.Core.GetQueryFilter.Autorization;
using SecuritySystem.Core.Interfaces;
using SecuritySystem.Core.Interfaces.Core;
using SecuritySystem.Core.Interfaces.Validators;
using SecuritySystem.Core.QueryFilters.Autorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Application.Services.Authorization
{
    public class MenuService : IMenuService
    {
        #region Fields
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly SecureSettings _secureSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationRepositoryValidator _authorizationRepositoryValidator;
        #endregion

        #region Constructor
        public MenuService(
            IAuthorizationRepository authorizationRepository,
            IAuthorizationRepositoryValidator authorizationRepositoryValidator,
            IOptions<SecureSettings> secureSettings,
            IUnitOfWork unitOfWork)
        {
            _authorizationRepository = authorizationRepository;
            _authorizationRepositoryValidator = authorizationRepositoryValidator;
            _secureSettings = secureSettings.Value;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Private helpers

        private List<RoleContentQueryFilter> InsertMenuItemByUserRole(
            List<RoleContentQueryFilter> menu,
            RoleContentQueryFilter item)
        {
            foreach (var subMenu in menu.ToList())
            {
                if (item.Level == subMenu.Level + 1 &&
                    item.RoleId == subMenu.RoleId)
                {
                    // Example: Level = "101020"
                    var prefix = item.Level.ToString().Substring(0,  item.IndentLevel * 2 - 2);
                    if (prefix == subMenu.Level.ToString())
                    {
                        subMenu.SubLinks.Add(item);
                        return menu;
                    }
                }
                else
                {
                    subMenu.SubLinks = InsertMenuItemByUserRole(subMenu.SubLinks, item);
                }
            }

            return menu;
        }

        private List<MenuContentQueryFilter> BuildMenuItemsWithFormatV2(
            MenuContentQueryFilter rootItem,
            string initialLevel,
            int initialIndentLevel)
        {
            var menu = new List<MenuContentQueryFilter>();

            rootItem.Level = initialLevel;
            rootItem.Indentation = initialIndentLevel;
            menu.Add(rootItem);

            initialIndentLevel++;
            var levelSeed = 10;
            var subLevel = 0;

            foreach (var child in rootItem.SubLinks)
            {
                child.ApplicationId = rootItem.ApplicationId;
                child.CreatedBy = rootItem.CreatedBy;

                menu.AddRange(
                    BuildMenuItemsWithFormatV2(
                        child,
                        initialLevel + (levelSeed + subLevel),
                        initialIndentLevel
                    )
                );

                subLevel++;
            }

            return menu;
        }

        private List<RoleContentQueryFilter> InsertMenuItemByApplication(
            List<RoleContentQueryFilter> menu,
            RoleContentQueryFilter item)
        {
            foreach (var subMenu in menu.ToList())
            {
                if (item.Level == subMenu.Level + 1)
                {
                    var prefix = item.Level.ToString().Substring(0, item.IndentLevel * 2 - 2);
                    if (prefix == subMenu.Level.ToString())
                    {
                        subMenu.SubLinks.Add(item);
                        return menu;
                    }
                }
                else
                {
                    subMenu.SubLinks = InsertMenuItemByApplication(subMenu.SubLinks, item);
                }
            }

            return menu;
        }

        /// <summary>
        /// If you re-enable encryption, this method should decrypt ResourceId.
        /// For now it just keeps the value as is.
        /// </summary>
        public void NormalizeResourceIds(List<MenuContentQueryFilter> menus)
        {
            foreach (var menu in menus)
            {
                menu.ResourceId = menu.ResourceId;

                if (menu.SubLinks != null && menu.SubLinks.Any())
                {
                    NormalizeResourceIds(menu.SubLinks);
                }
            }
        }

        public List<RoleContentQueryFilter> GetParentNodesByPage(
            IEnumerable<RoleContentQueryFilter> fullMenu,
            string resourceId)
        {
            var child = fullMenu.FirstOrDefault(a => a.ResourceId.ToString() == resourceId);
            var parentNodes = new List<RoleContentQueryFilter>();

            while (child != null && child.IndentLevel > 0)
            {
                var possibleParents = fullMenu
                    .Where(a =>
                        a.IndentLevel == (child.IndentLevel - 1) &&
                        child.Level.ToString().StartsWith(a.Level.ToString()))
                    .OrderByDescending(a => a.Level.ToString().Length)
                    .ToList();

                if (!possibleParents.Any())
                {
                    break;
                }

                var parentNode = possibleParents.FirstOrDefault();

                if (parentNode != null)
                {
                    parentNodes.Add(parentNode);
                    child = parentNode;
                }
                else
                {
                    break;
                }
            }

            return parentNodes;
        }

        public List<RoleContentQueryFilter> GetAllPageNodesByParent(
            IEnumerable<RoleContentQueryFilter> fullMenu,
            string resourceId)
        {
            var parent = fullMenu.FirstOrDefault(a => a.ResourceId.ToString() == resourceId);
            if (parent == null)
                return new List<RoleContentQueryFilter>();

            fullMenu = fullMenu.Where(m => m.Id != parent.Id);

            var directChildren = fullMenu
                .Where(m =>
                    m.Level.ToString().Length == parent.Level.ToString().Length + 2 &&
                    m.Level.ToString().StartsWith(parent.Level.ToString()) &&
                    m.IndentLevel == parent.IndentLevel + 1)
                .ToList();

            var allChildren = new List<RoleContentQueryFilter>(directChildren);

            foreach (var child in directChildren)
            {
                var childDescendants = GetAllPageNodesByParent(fullMenu, child.ResourceId.ToString());
                allChildren.AddRange(childDescendants);
            }

            return allChildren;
        }

        public List<RoleContentQueryFilter> GetPageNodesByParent(
            IEnumerable<RoleContentQueryFilter> fullMenu,
            string resourceId)
        {
            var parent = fullMenu.FirstOrDefault(a => a.ResourceId.ToString() == resourceId);
            if (parent == null)
                return new List<RoleContentQueryFilter>();

            fullMenu = fullMenu.Where(m => m.Id != parent.Id);

            var directChildren = fullMenu
                .Where(m =>
                    m.Level.ToString().Length == parent.Level.ToString().Length + 2 &&
                    m.Level.ToString().StartsWith(parent.Level.ToString()) &&
                    m.IndentLevel == parent.IndentLevel + 1)
                .ToList();

            return directChildren;
        }

        private List<RolesMenuByApplicationQueryFilter> CreateSubLinksByApplication(
            List<RolesMenuByApplicationQueryFilter> menu,
            RolesMenuByApplicationQueryFilter item)
        {
            foreach (var subMenu in menu.ToList())
            {
                if (item.Indentation == subMenu.Indentation + 1)
                {
                    var prefix = item.Level.Substring(0, item.Indentation * 2 - 2);
                    if (prefix == subMenu.Level)
                    {
                        subMenu.SubLinks.Add(item);
                        return menu;
                    }
                }
                else
                {
                    subMenu.SubLinks = CreateSubLinksByApplication(subMenu.SubLinks, item);
                }
            }

            return menu;
        }

        #endregion

        #region IMenuService implementation

        public async Task<ResponsePost> CreateResourceMenu(MenuV2QueryFilter menuQueryFilter)
        {
            try
            {
                #region Validator
                var validation = _authorizationRepositoryValidator.ValidateInsertMenuV2(menuQueryFilter);
                if (!validation.IsValid)
                {
                    return new ResponsePost
                    {
                        Mensajes = validation.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
                #endregion

                var initialLevel = 10;
                var initialIndentLevel = 1;

                var insertCommit = new ResponsePostDetail();
                var updateCommit = new ResponsePostDetail();

                var applicationId = menuQueryFilter.ApplicationId;

                #region Flatten menu
                var flatMenu = new List<MenuContentQueryFilter>();

                foreach (var element in menuQueryFilter.Menu)
                {
                    element.CreatedBy = "AUTHORIZATION";
                    element.ApplicationId = applicationId;

                    flatMenu.AddRange(
                        BuildMenuItemsWithFormatV2(
                            element,
                            initialLevel.ToString(),
                            initialIndentLevel
                        )
                    );

                    initialLevel++;
                }
                #endregion

                NormalizeResourceIds(flatMenu);

                #region Validate depth
                var ordered = flatMenu.OrderByDescending(x => x.Indentation).ToList();
                var maxDepth = ordered.FirstOrDefault()?.Indentation ?? 0;

                if (maxDepth > 6)
                {
                    return new ResponsePost
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Type        = TypeMessage.warning.ToString(),
                                Description = "A menu with more than 6 levels is not allowed."
                            }
                        },
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
                #endregion

                #region Get existing menu by application
                var existingMenu = await _authorizationRepository.GetMenuByApplication(applicationId, 1);

                var newMenuItems = new List<ResourceMenu>();
                var menuItemsToUpdate = new List<ResourceMenu>();
                var roleResourcesToUpdate = new List<RoleResourceMenu>();

                if (!existingMenu.Any())
                {
                    foreach (var element in flatMenu)
                    {
                        newMenuItems.Add(new ResourceMenu
                        {
                            ResourceId = Convert.ToInt32(element.ResourceId),
                            Level = Convert.ToInt32(element.Level),
                            IndentLevel = element.Indentation,
                            CreatedBy = "AUTHORIZATION"
                        });
                    }
                }
                else
                {
                    var newItems = flatMenu.Where(a => !existingMenu.Any(b => b.ResourceId.ToString() == a.ResourceId));
                    if (newItems.Any())
                    {
                        foreach (var element in newItems)
                        {
                            newMenuItems.Add(new ResourceMenu
                            {
                                ResourceId = Convert.ToInt32(element.ResourceId),
                                Level = Convert.ToInt32(element.Level),
                                IndentLevel = element.Indentation,
                                CreatedBy = "AUTHORIZATION"
                                
                            });
                        }
                    }

                    var reactivable = existingMenu
                        .Where(a => a.RecordStatus == 0 && flatMenu.Any(b => b.ResourceId == a.ResourceId.ToString()));

                    var removed = existingMenu
                        .Where(a => a.RecordStatus == 1 && !flatMenu.Any(b => b.ResourceId == a.ResourceId.ToString()));

                    var existingActive = existingMenu
                        .Where(a => a.RecordStatus == 1 && flatMenu.Any(b => b.ResourceId == a.ResourceId.ToString()));

                    if (existingActive.Any())
                    {
                        foreach (var element in existingActive)
                        {
                            menuItemsToUpdate.Add(new ResourceMenu
                            {
                                Id = Convert.ToInt32(element.Id),
                                ResourceId =Convert.ToInt32(element.ResourceId),
                                Level = Convert.ToInt32(
                                    flatMenu.Where(a => a.ResourceId == element.ResourceId.ToString())
                                            .Select(a => a.Level)
                                            .FirstOrDefault()
                                ),
                                IndentLevel = flatMenu.Where(a => a.ResourceId == element.ResourceId.ToString())
                                                      .Select(a => a.Indentation)
                                                      .FirstOrDefault(),
                                CreatedAt = DateTime.Now,
                                RecordStatus = 1,
                                CreatedBy = "AUTHORIZATION"
                            });
                        }
                    }

                    if (removed.Any())
                    {
                        var resourceIdsCsv = string.Join(",", removed.Select(a => a.ResourceId));
                        var rolesResources = await _authorizationRepository.GetRolesByResourceId(resourceIdsCsv);

                        foreach (var element in removed)
                        {
                            menuItemsToUpdate.Add(new ResourceMenu
                            {
                                Id = Convert.ToInt32(element.Id),
                                ResourceId =Convert.ToInt32(element.ResourceId),
                                Level =Convert.ToInt32(element.Level),
                                IndentLevel =Convert.ToInt32(element.IndentLevel),
                                CreatedAt = DateTime.Now,
                                RecordStatus = 0,
                                CreatedBy = "AUTHORIZATION"
                            });
                        }

                        if (rolesResources.Any())
                        {
                            foreach (var rr in rolesResources)
                            {
                                roleResourcesToUpdate.Add(new RoleResourceMenu
                                {
                                    Id = Convert.ToInt32(rr.Id),
                                    RoleId = Convert.ToInt32(rr.RoleId),
                                    ResourceId = Convert.ToInt32(rr.ResourceId),
                                    RecordStatus = 0,
                                    CreatedAt = DateTime.Now,
                                    CreatedBy = "AUTHORIZATION"
                                });
                            }
                        }
                    }

                    if (reactivable.Any())
                    {
                        foreach (var element in reactivable)
                        {
                            menuItemsToUpdate.Add(new ResourceMenu
                            {
                                Id =Convert.ToInt32(element.Id),
                                ResourceId =Convert.ToInt32(element.ResourceId),
                                Level = Convert.ToInt32(
                                    flatMenu.Where(a => a.ResourceId == element.ResourceId.ToString())
                                            .Select(a => a.Level)
                                            .FirstOrDefault()
                                ),
                                IndentLevel = flatMenu.Where(a => a.ResourceId == element.ResourceId.ToString())
                                                      .Select(a => a.Indentation)
                                                      .FirstOrDefault(),
                                CreatedAt = DateTime.Now,
                                RecordStatus = 1,
                                CreatedBy = "AUTHORIZATION"
                            });
                        }
                    }
                }
                #endregion

                if (newMenuItems.Any())
                {
                    insertCommit = _unitOfWork.ResourceMenuRepository.Insert(newMenuItems);
                }

                if (menuItemsToUpdate.Any())
                {
                    updateCommit = _unitOfWork.ResourceMenuRepository.Update(menuItemsToUpdate);
                }

                if (roleResourcesToUpdate.Any())
                {
                    _unitOfWork.RoleResourceMenuRepository.Update(roleResourcesToUpdate);
                }

                var affectedRows = insertCommit.RowsAffected + updateCommit.RowsAffected;

                return new ResponsePost
                {
                    Respuesta = new ResponsePostDetail
                    {
                        Process = "CREATE MENU",
                        RowsAffected = affectedRows
                    },
                    StatusCode = HttpStatusCode.OK,
                    Mensajes = new[]
                    {
                        new Message
                        {
                            Type        = TypeMessage.information.ToString(),
                            Description = "The menu was saved successfully."
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseGetObject> GetMenuByApplication(string applicationId)
        {
            try
            {
                var menu = await _authorizationRepository.GetMenuByApplication(applicationId, onlyActive: 1);
                menu = menu.Where(x => x.IndentLevel > 0);

                if (menu.Any())
                {
                    var formattedMenu = new List<RoleContentQueryFilter>();
                    var ordered = menu.OrderBy(a => a.Level);

                    foreach (var item in ordered)
                    {
                        item.Id = item.Id;
                        item.ResourceId = item.ResourceId;

                        if (item.IndentLevel == 1)
                        {
                            formattedMenu.Add(item);
                        }
                        else
                        {
                            formattedMenu = InsertMenuItemByApplication(formattedMenu, item);
                        }
                    }

                    return new ResponseGetObject
                    {
                        Datos = formattedMenu,
                        StatusCode = HttpStatusCode.OK
                    };
                }

                return new ResponseGetObject
                {
                    Mensajes = new[]
                    {
                        new Message
                        {
                            Type        = TypeMessage.information.ToString(),
                            Description = "No menu found."
                        }
                    },
                    StatusCode = HttpStatusCode.NotFound
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseGetObject> GetMenuByRole(GetMenuQueryFilter queryFilter)
        {
            try
            {
                var validation = _authorizationRepositoryValidator.ValidateGetMenuByRole(queryFilter);
                if (!validation.IsValid)
                {
                    return new ResponseGetObject
                    {
                        Mensajes = validation.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                var roleMenu = await _authorizationRepository.GetMenuByRole(queryFilter);
                if (!roleMenu.Any())
                {
                    return new ResponseGetObject
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Type        = TypeMessage.information.ToString(),
                                Description = "Menu not found."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                roleMenu = roleMenu.Where(a => a.RecordStatus == "1");

                var applicationId = roleMenu
                    .Select(x => x.ApplicationId)
                    .Distinct()
                    .SingleOrDefault();

                var fullMenu = await _authorizationRepository.GetMenuByApplication(applicationId, onlyActive: 1);

                var preUserMenu = new List<RoleContentQueryFilter>();

                foreach (var item in roleMenu)
                {
                    var menuItem = fullMenu.FirstOrDefault(b => b.ResourceId.ToString() == item.ResourceId);
                    if (menuItem != null)
                    {
                        preUserMenu.Add(menuItem);

                        if (item.Indentation > 1)
                        {
                            var parents = GetParentNodesByPage(fullMenu, item.ResourceId);
                            foreach (var p in parents)
                            {
                                if (!preUserMenu.Any(a => a.ResourceId == p.ResourceId))
                                {
                                    preUserMenu.Add(p);
                                }
                            }
                        }
                    }
                }

                var orderedUserMenu = preUserMenu.OrderBy(a => a.Level);
                var finalMenu = new List<RoleContentQueryFilter>();

                foreach (var element in orderedUserMenu.ToList())
                {
                    element.Id = element.Id;
                    element.ResourceId = element.ResourceId;
                    element.RoleId = null;

                    if (element.IndentLevel == 1)
                    {
                        finalMenu.Add(element);
                    }
                    else
                    {
                        finalMenu = InsertMenuItemByUserRole(finalMenu, element);
                    }
                }

                return new ResponseGetObject
                {
                    Datos = finalMenu,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseGetObject> GetRoleResources(GetRoleMenuQueryFilter queryFilter)
        {
            try
            {
                var validation =
                    _authorizationRepositoryValidator.ValidateGetRoleMenuByApplication(queryFilter);

                if (!validation.IsValid)
                {
                    return new ResponseGetObject
                    {
                        Mensajes = validation.ValidationMessages.ToArray(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                var menu = await _authorizationRepository.GetRolesMenuByApplication(queryFilter.ApplicationId);
                var menuList = menu.Where(x => x.Indentation > 0).ToList();

                if (!menuList.Any())
                {
                    return new ResponseGetObject
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Type        = TypeMessage.information.ToString(),
                                Description = "No menu found."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                var resourceIds = menuList.Select(c => c.ResourceId.ToString()).ToList();

                var roles = await _unitOfWork.RoleResourceMenuRepository.GetByCustomQuery(a =>
                    a.Where(b => b.RecordStatus == 1 && resourceIds.Contains(b.ResourceId.ToString())).ToList()
                );

                if (queryFilter.Roles != null)
                {
                    roles = roles
                        .Where(a => queryFilter.Roles.Contains(a.RoleId.ToString()))
                        .ToList();
                }

                var formattedMenu = new List<RolesMenuByApplicationQueryFilter>();
                var ordered = menuList.OrderBy(a => a.Level);

                foreach (var element in ordered)
                {
                    element.Roles = roles
                        .Where(a => a.ResourceId.ToString() == element.ResourceId)
                        .Select(r => r.RoleId.ToString())
                        .ToList();

                    element.Roles = element.Roles.Select(r => r).ToList();
                    element.Id = element.Id;
                    element.ResourceId = element.ResourceId;

                    if (element.Indentation == 1)
                    {
                        formattedMenu.Add(element);
                    }
                    else
                    {
                        formattedMenu = CreateSubLinksByApplication(formattedMenu, element);
                    }
                }

                return new ResponseGetObject
                {
                    Datos = formattedMenu,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponsePost> InsertRoleResource(InsertRoleResourceQueryFilter queryFilter)
        {
            try
            {
                var existing = await _unitOfWork.RoleResourceMenuRepository.GetByCustomQuery(a =>
                    a.Where(b =>
                        b.RoleId.ToString() == queryFilter.RoleId &&
                        b.ResourceId.ToString() == queryFilter.ResourceId
                    )
                    .ToList()
                );

                if (!existing.Any())
                {
                    _unitOfWork.RoleResourceMenuRepository.Insert(new RoleResourceMenu
                    {
                        RoleId = Convert.ToInt32(queryFilter.RoleId),
                        ResourceId = Convert.ToInt32(queryFilter.ResourceId),
                        CreatedBy = "AUTHORIZATION"
                    });
                }
                else
                {
                    var current = existing.First();

                    _unitOfWork.RoleResourceMenuRepository.Update(new RoleResourceMenu
                    {
                        Id = current.Id,
                        RoleId = current.RoleId,
                        ResourceId = current.ResourceId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = "AUTHORIZATION",
                        RecordStatus = queryFilter.IsSelected
                    });
                }

                if (queryFilter.IsSelected == 1)
                {
                    return new ResponsePost
                    {
                        StatusCode = HttpStatusCode.OK,
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Type        = TypeMessage.success.ToString(),
                                Description = "The page was successfully assigned to the role."
                            }
                        }
                    };
                }

                return new ResponsePost
                {
                    StatusCode = HttpStatusCode.OK,
                    Mensajes = new[]
                    {
                        new Message
                        {
                            Type        = TypeMessage.success.ToString(),
                            Description = "The page was removed from the role."
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseGetObject> GetMenuResourcesByUser(GetMenuQueryFilter queryFilter)
        {
            try
            {
                var userRoles =
                    (await _authorizationRepository.GetUserRoles(new GetUsersByRoleQueryFilter { UserId = queryFilter.UserId }))
                    .Select(a => a.RoleId);

                queryFilter.RoleId = string.Join(',', userRoles);

                var allRoleResources =
                    await _authorizationRepository.GetRoleResourcesForUser(queryFilter);

                var roleResources = allRoleResources?
                    .Where(a => a.RecordStatus == "1")
                    .ToList();

                if (roleResources == null || !roleResources.Any())
                {
                    return new ResponseGetObject
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Type        = TypeMessage.information.ToString(),
                                Description = "No menu found."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                roleResources = roleResources
                    .Where(a =>
                        a.ApplicationId == queryFilter.ApplicationId &&
                        a.RecordStatus == "1")
                    .GroupBy(a => a.ResourceId)
                    .Select(grp => grp.First())
                    .ToList();

                var fullMenu =
                    await _authorizationRepository.GetMenuByApplication(queryFilter.ApplicationId, onlyActive: 1);

                if (!fullMenu.Any())
                {
                    return new ResponseGetObject
                    {
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Type        = TypeMessage.information.ToString(),
                                Description = "No menu found."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                var preUserMenu = new List<RoleContentQueryFilter>();

                foreach (var rr in roleResources)
                {
                    var menuItem = fullMenu.FirstOrDefault(b => b.ResourceId.ToString() == rr.ResourceId);
                    if (menuItem == null) continue;

                    preUserMenu.Add(menuItem);

                    if (rr.Indentation > 1)
                    {
                        var parents = GetParentNodesByPage(fullMenu, rr.ResourceId);
                        foreach (var p in parents)
                        {
                            if (!preUserMenu.Any(a => a.ResourceId == p.ResourceId))
                            {
                                preUserMenu.Add(p);
                            }
                        }
                    }
                }

                var orderedUserMenu = preUserMenu.OrderBy(a => a.Level);
                var finalMenu = new List<RoleContentQueryFilter>();

                foreach (var element in orderedUserMenu.ToList())
                {
                    element.Id = element.Id;
                    element.ResourceId = element.ResourceId;
                    element.RoleId = null;

                    if (element.IndentLevel == 1)
                    {
                        finalMenu.Add(element);
                    }
                    else
                    {
                        finalMenu = InsertMenuItemByUserRole(finalMenu, element);
                    }
                }

                return new ResponseGetObject
                {
                    Datos = finalMenu,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseGet> GetRolesByResourceId(string resourceId)
        {
            try
            {
                var rolesResources = await _authorizationRepository.GetRolesByResourceId(resourceId);

                if (!rolesResources.Any())
                {
                    return new ResponseGet
                    {
                        Messages = new[]
                        {
                            new Message
                            {
                                Type        = TypeMessage.information.ToString(),
                                Description = "There are no roles related to this resource."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                foreach (var item in rolesResources)
                {
                    item.Id = item.Id;
                    item.RoleId = item.RoleId;
                    item.ApplicationId = item.ApplicationId;
                    item.ResourceId = item.ResourceId;
                }

                return new ResponseGet
                {
                    Data = rolesResources,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseGet> ValidateResourceMenuChanges(ValidateMenuQueryFilter menuQueryFilter)
        {
            try
            {
                var resourcesCsv = string.Join(",", menuQueryFilter.ResourceIds.Select(a => a));
                var rolesResources = await _authorizationRepository.GetRolesByResourceId(resourcesCsv);

                if (!rolesResources.Any())
                {
                    return new ResponseGet
                    {
                        Messages = new[]
                        {
                            new Message
                            {
                                Type        = TypeMessage.success.ToString(),
                                Description = "No roles are associated; the menu can be modified."
                            }
                        },
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                var validatedMenu = rolesResources
                    .GroupBy(ro => ro.ResourceId)
                    .Select(group => new ValidatedMenuQueryFilter
                    {
                        Page = group.First().Page,
                        ResourceId = group.Key,
                        Roles = group.Select(ro => ro.Name).Distinct().ToList()
                    })
                    .ToList();

                return new ResponseGet
                {
                    Data = validatedMenu,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseGetObject> GetMenuByUserRoles(GetMenuQueryFilter queryFilter)
        {
            // If you want a separate behavior from GetMenuResourcesByUser, you can implement it here.
            // For now, we reuse the same logic.
            var result = await GetMenuResourcesByUser(queryFilter);
            return new ResponseGetObject
            {
                Datos = result.Datos,
                Mensajes = result.Mensajes,
                StatusCode = result.StatusCode
            };
        }

        #endregion
    }
}
