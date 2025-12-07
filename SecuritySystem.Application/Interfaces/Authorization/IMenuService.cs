using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi;
using SecuritySystem.Core.GetQueryFilter.Autorization;
using SecuritySystem.Core.QueryFilters.Autorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Application.Interfaces.Authorization
{
    public interface IMenuService
    {
        Task<ResponsePost> CreateResourceMenu(MenuV2QueryFilter menuQueryFilter);
        Task<ResponseGetObject> GetMenuByApplication(string applicationId);
        Task<ResponseGetObject> GetMenuByRole(GetMenuQueryFilter queryFilter);
        Task<ResponseGetObject> GetRoleResources(GetRoleMenuQueryFilter queryFilter);
        Task<ResponsePost> InsertRoleResource(InsertRoleResourceQueryFilter queryFilter);
        Task<ResponseGetObject> GetMenuResourcesByUser(GetMenuQueryFilter queryFilter);
        Task<ResponseGet> GetRolesByResourceId(string resourceId);
        Task<ResponseGet> ValidateResourceMenuChanges(ValidateMenuQueryFilter menuQueryFilter);
        Task<ResponseGetObject> GetMenuByUserRoles(GetMenuQueryFilter queryFilter);
    }
}
