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
    public interface IApplicationsService
    {
        Task<ResponsePost> CreateApplication(ApplicationQueryFilter applicationQueryFilter, string token);
        Task<ResponsePost> UpdateApplication(ApplicationQueryFilter applicationQueryFilter, string token);
        Task<ResponsePost> DeleteApplicationById(IdStringFilter idFilter, string token);
        Task<ResponseGetObject> GetApplicationById(string applicationId);
        Task<object> GetApplicationList(GetApplicationsQueryFilter getApplicationsQueryFilter);
        Task<ResponseGet> GetApplications(GetApplicationsQueryFilter getApplicationsQueryFilter);
        Task<ResponseGet> GetApplicationsByUser(string token);
    }
}
