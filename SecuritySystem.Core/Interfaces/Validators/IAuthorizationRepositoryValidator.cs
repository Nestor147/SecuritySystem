using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi;
using SecuritySystem.Core.GetQueryFilter.Autorization;
using SecuritySystem.Core.QueryFilters.Autorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Core.Interfaces.Validators
{
    public interface IAuthorizationRepositoryValidator
    {
        // Applications
        ResponseModel ValidateCreateApplication(ApplicationQueryFilter applicationQueryFilter);
        ResponseModel ValidateUpdateApplication(ApplicationQueryFilter applicationQueryFilter);

        // Resources (Nodes / Pages)
        ResponseModel ValidateCreatePage(ResourceQueryFilter resourceQueryFilter);
        ResponseModel ValidateCreateNode(ResourceQueryFilter resourceQueryFilter);
        ResponseModel ValidateUpdateNode(ResourceQueryFilter resourceQueryFilter);
        ResponseModel ValidateUpdatePage(ResourceQueryFilter resourceQueryFilter);
        ResponseModel ValidateGetResources(GetResourcesQueryFilter getResourcesQueryFilter);

        // Menu
        ResponseModel ValidateInsertMenuV2(MenuV2QueryFilter menuQueryFilter);
        ResponseModel ValidateInsertRoleResource(InsertRoleResourceQueryFilter queryFilter);

        // Roles
        ResponseModel ValidateCreateRole(RoleQueryFilter roleQueryFilter);
        ResponseModel ValidateUpdateRole(RoleQueryFilter roleQueryFilter);
        ResponseModel ValidateGetRoles(GetRoleQueryFilter getRoleQueryFilter);
        ResponseModel ValidateGetRolesByUser(GetUsersByRoleQueryFilter getUsersByRoleQueryFilter);
        ResponseModel ValidateGetUsersByRole(GetUsersByRoleQueryFilter getUsersByRoleQueryFilter);
        ResponseModel ValidateInsertUserRoles(InsertUserRoleQueryFilter queryFilter);
        ResponseModel ValidateGetMenuByUserRoles(GetMenuQueryFilter getMenuQueryFilter);
        ResponseModel ValidateInsertRoleMenu(InsertRoleResourceQueryFilter menuQueryFilter);
        ResponseModel ValidateGetRoleMenuByApplication(GetRoleMenuQueryFilter menuQueryFilter);
        ResponseModel ValidateGetMenuByRole(GetMenuQueryFilter getMenuQueryFilter);
    }
}
