using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi;
using SecuritySystem.Core.GetQueryFilter.Autorization;
using SecuritySystem.Core.QueryFilters.Autorization;
using SecuritySystem.Core.QueryFilters.Helper;

namespace SecuritySystem.Application.Interfaces.Authorization
{
    public interface IRoleService
    {
        Task<ResponsePost> InsertRole(RoleQueryFilter queryFilter, string token);
        Task<ResponsePost> UpdateRole(RoleQueryFilter roleQueryFilter, string token);
        Task<ResponseGetObject> GetRoleById(string roleId);
        Task<ResponsePost> DeleteRoleById(IdStringFilter roleId);
        Task<object> GetRoleList(GetRoleQueryFilter getRoleQueryFilter);
        Task<ResponseGet> GetRolesCombo(GetRoleQueryFilter getRoleQueryFilter);
        Task<object> GetRolesByUser(GetUsersByRoleQueryFilter getUsersByRoleQueryFilter);
        Task<object> GetUsersByRole(GetUsersByRoleQueryFilter getUsersByRoleQueryFilter);
        Task<ResponsePost> DeleteUserRoleById(IdStringFilter userRoleId);
        Task<ResponseGetObject> GetRolesUsers(GetUserRoles queryFilter);
        Task<ResponsePost> InsertUserRoles(List<InsertUserRoleQueryFilter> queryFilter);
        Task<ResponseGetObject> GetRolesForMultipleUsers(GetUserRoles queryFilter);
        Task<ResponseGetObject> SearchUsersByDocOrName(SearchUsersQueryFilter query);
    }

}
