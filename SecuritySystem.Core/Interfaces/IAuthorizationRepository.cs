using SecuritySystem.Core.GetQueryFilter.Autorization;
using SecuritySystem.Core.QueryFilters.Autorization;
using SecuritySystem.Core.QueryFilters.Autorization.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Core.Interfaces
{
    public interface IAuthorizationRepository
    {
        // IMPLEMENTED (with logic in AutorizacionRepository)

        Task<IEnumerable<RoleQueryFilter>> GetRoles(GetRoleQueryFilter queryFilterRoles);
        Task<IEnumerable<UserRoleQueryFilter>> GetUserRoles(GetUsersByRoleQueryFilter queryFilter);

        // New implemented methods

        Task<IEnumerable<ApplicationQueryFilter>> GetDuplicateApplications(ApplicationQueryFilter applicationQueryFilter);
        Task<IEnumerable<ApplicationQueryFilter>> GetApplications();
        Task<IEnumerable<ApplicationsByUserRequest>> GetApplicationsByUserId(string userId);

        Task<IEnumerable<ResourceQueryFilter>> GetResources();
        Task<IEnumerable<ResourceQueryFilter>> GetDuplicateResources(ResourceQueryFilter resourceQueryFilter);

        Task<IEnumerable<RolesMenuByApplicationQueryFilter>> GetRolesMenuByApplication(string applicationId);
        Task<IEnumerable<RoleQueryFilter>> GetRolesByApplication(string applicationId);
        Task<IEnumerable<RoleResourceQueryFilter>> GetRolesByResourceId(string resourceId);

        Task<IEnumerable<UserRoleQueryFilter>> GetRolesForMultipleUsers(string userId);
        Task<IEnumerable<UserDataQueryFilter>> GetUserDataByRoleId(string roleId);

        Task<IEnumerable<RoleResourceMenuQueryFilter>> GetRoleResourcesForUser(GetMenuQueryFilter getMenuQueryFilter);
        Task<IEnumerable<RoleQueryFilter>> GetDuplicateRoles(RoleQueryFilter roleQueryFilter);
        Task<IEnumerable<RoleResourceMenuQueryFilter>> GetMenuByRole(GetMenuQueryFilter getMenuQueryFilter);
        Task<IEnumerable<RoleContentQueryFilter>> GetMenuByApplication(string applicationId, int onlyActive);

        Task<IEnumerable<UserByDocOrNameResultQueryFilter>> SearchUsersByCriteria(
            string? searchCriteria,
            bool onlyActive = true,
            int top = 50);
    }
}
