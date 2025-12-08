using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi;
using SecuritySystem.Core.GetQueryFilter.Autorization;
using SecuritySystem.Core.QueryFilters.Autorization;
using SecuritySystem.Core.QueryFilters.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Application.Interfaces.Authorization
{
    public interface IResourceService
    {
        Task<ResponsePost> InsertResource(ResourceQueryFilter resourceFilter);
        Task<ResponsePost> UpdateResource(ResourceQueryFilter resourceFilter);
        Task<ResponseGetObject> GetResourceById(string resourceId);
        Task<ResponsePost> DeleteResourceById(IdStringFilter resourceId, int resourceType);
        Task<object> GetResourceList(GetResourcesQueryFilter queryFilter);
        Task<ResponseGet> GetResources(GetResourcesQueryFilter queryFilter);
    }
}
