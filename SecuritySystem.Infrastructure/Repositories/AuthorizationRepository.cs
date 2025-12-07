using SecuritySystem.Core.GetQueryFilter.Autorization;
using SecuritySystem.Core.Interfaces;
using SecuritySystem.Core.Interfaces.Core.SQLServer.ADO;
using SecuritySystem.Core.QueryFilters.Autorization;
using SecuritySystem.Core.QueryFilters.Autorization.Request;
using System.Data;
using System.Linq;

namespace SecuritySystem.Infrastructure.Repositories
{
    public class AuthorizationRepository : IAuthorizationRepository
    {
        private readonly IAdo _ado;

        public AuthorizationRepository(IAdo ado)
        {
            _ado = ado;
        }

        #region Applications

        public async Task<IEnumerable<ApplicationQueryFilter>> GetApplications()
        {
            var rows = await _ado.ExecuteEntitiesAsync(
                "AUTORIZACION.GetApplications",
                CommandType.StoredProcedure,
                parameters: null
            );

            return rows.Select(r => r.To<ApplicationQueryFilter>());
        }

        public async Task<IEnumerable<ApplicationsByUserRequest>> GetApplicationsByUserId(string userId)
        {
            var rows = await _ado.ExecuteEntitiesAsync(
                "AUTORIZACION.GetApplicationsByUserId",
                CommandType.StoredProcedure,
                new { id_usuario = Convert.ToInt32(userId) }
            );

            return rows.Select(r => r.To<ApplicationsByUserRequest>());
        }

        public async Task<IEnumerable<ApplicationQueryFilter>> GetDuplicateApplications(ApplicationQueryFilter applicationQueryFilter)
        {
            int? idAplicacion = null;
            if (!string.IsNullOrWhiteSpace(applicationQueryFilter.Id) &&
                int.TryParse(applicationQueryFilter.Id, out var val) &&
                val > 0)
            {
                idAplicacion = val;
            }

            var rows = await _ado.ExecuteEntitiesAsync(
                "AUTORIZACION.GetDuplicatedApplications",
                CommandType.StoredProcedure,
                new
                {
                    IdAplicacion = idAplicacion,
                    Sigla = applicationQueryFilter.Code,
                    Descripcion = applicationQueryFilter.Description
                }
            );

            return rows.Select(r => r.To<ApplicationQueryFilter>());
        }

        #endregion

        #region Resources

        public async Task<IEnumerable<ResourceQueryFilter>> GetResources()
        {
            var rows = await _ado.ExecuteEntitiesAsync(
                "AUTORIZACION.GetResources",
                CommandType.StoredProcedure,
                parameters: null
            );

            return rows.Select(r => r.To<ResourceQueryFilter>());
        }

        public async Task<IEnumerable<ResourceQueryFilter>> GetDuplicateResources(ResourceQueryFilter resourceQueryFilter)
        {
            int? idObjeto = null;
            if (!string.IsNullOrWhiteSpace(resourceQueryFilter.Id) &&
                int.TryParse(resourceQueryFilter.Id, out var val) &&
                val > 0)
            {
                idObjeto = val;
            }

            string? page = string.IsNullOrWhiteSpace(resourceQueryFilter.Page)
                ? null
                : resourceQueryFilter.Page.Trim();

            string? description = string.IsNullOrWhiteSpace(resourceQueryFilter.Description)
                ? null
                : resourceQueryFilter.Description.Trim();

            var rows = await _ado.ExecuteEntitiesAsync(
                "AUTORIZACION.GetDuplicatedResources",
                CommandType.StoredProcedure,
                new
                {
                    Pagina = page,
                    Descripcion = description,
                    IdObjeto = idObjeto
                }
            );

            return rows.Select(r => r.To<ResourceQueryFilter>());
        }

        public async Task<IEnumerable<RoleResourceQueryFilter>> GetRolesByResourceId(string resourceIdsCsv)
        {
            if (string.IsNullOrWhiteSpace(resourceIdsCsv))
                return Enumerable.Empty<RoleResourceQueryFilter>();

            var rows = await _ado.ExecuteEntitiesAsync(
                "AUTORIZACION.GetRolesByResourceIds",
                CommandType.StoredProcedure,
                new
                {
                    // El SP espera un NVARCHAR(MAX) con CSV: '1,2,3'
                    IdObjeto = resourceIdsCsv
                }
            );

            return rows.Select(r => r.To<RoleResourceQueryFilter>());
        }

        #endregion

        #region Roles

        public async Task<IEnumerable<RoleQueryFilter>> GetRoles(GetRoleQueryFilter queryFilterRoles)
        {
            var rows = await _ado.ExecuteEntitiesAsync(
                "AUTORIZACION.GetRoles",
                CommandType.StoredProcedure,
                new { IdAplicacion = Convert.ToInt32(queryFilterRoles.ApplicationId) }
            );

            return rows.Select(r => r.To<RoleQueryFilter>());
        }

        public async Task<IEnumerable<RoleQueryFilter>> GetRolesByApplication(string applicationId)
        {
            // Igual que GetRoles: el SP recibe IdAplicacion
            var rows = await _ado.ExecuteEntitiesAsync(
                "AUTORIZACION.GetRoles",
                CommandType.StoredProcedure,
                new { IdAplicacion = Convert.ToInt32(applicationId) }
            );

            return rows.Select(r => r.To<RoleQueryFilter>());
        }

        public async Task<IEnumerable<RoleQueryFilter>> GetDuplicateRoles(RoleQueryFilter roleQueryFilter)
        {
            int? idRol = null;
            if (!string.IsNullOrWhiteSpace(roleQueryFilter.Id) &&
                int.TryParse(roleQueryFilter.Id, out var val) &&
                val > 0)
            {
                idRol = val;
            }

            string? nombre = string.IsNullOrWhiteSpace(roleQueryFilter.Name)
                ? null
                : roleQueryFilter.Name.Trim();

            string? descripcion = string.IsNullOrWhiteSpace(roleQueryFilter.Description)
                ? null
                : roleQueryFilter.Description.Trim();

            var rows = await _ado.ExecuteEntitiesAsync(
                "AUTORIZACION.GetDuplicatedRoles",
                CommandType.StoredProcedure,
                new
                {
                    IdAplicacion = roleQueryFilter.ApplicationId,
                    Nombre = nombre,
                    Descripcion = descripcion,
                    IdRol = idRol
                }
            );

            return rows.Select(r => r.To<RoleQueryFilter>());
        }

        public async Task<IEnumerable<UserRoleQueryFilter>> GetRolesForMultipleUsers(string userId)
        {
            var rows = await _ado.ExecuteEntitiesAsync(
                "AUTORIZACION.GetRolesByMultipleUsers",
                CommandType.StoredProcedure,
                new { IdUsuario = userId }
            );

            return rows.Select(r => r.To<UserRoleQueryFilter>());
        }

        #endregion

        #region Menus / Role-Resource Menus

        public async Task<IEnumerable<RoleContentQueryFilter>> GetMenuByApplication(string applicationId, int onlyActive)
        {
            // Antes: Autorizacion.ObtenerMenuCompleto
            var rows = await _ado.ExecuteEntitiesAsync(
                "AUTORIZACION.GetFullMenu",
                CommandType.StoredProcedure,
                new
                {
                    IdAplicacion = Convert.ToInt32(applicationId),
                    SoloActivos = onlyActive // igual que antes, solo activos
                }
            );

            return rows.Select(r => r.To<RoleContentQueryFilter>());
        }

        public async Task<IEnumerable<RoleResourceMenuQueryFilter>> GetMenuByRole(GetMenuQueryFilter getMenuQueryFilter)
        {
            // Antes: Autorizacion.ObtenerMenuPorIdRol
            var rows = await _ado.ExecuteEntitiesAsync(
                "AUTORIZACION.GetMenuByRoleId",
                CommandType.StoredProcedure,
                new
                {
                    IdRol = Convert.ToInt32(getMenuQueryFilter.RoleId)
                }
            );

            return rows.Select(r => r.To<RoleResourceMenuQueryFilter>());
        }

        public async Task<IEnumerable<RolesMenuByApplicationQueryFilter>> GetRolesMenuByApplication(string applicationId)
        {
            // Antes: Autorizacion.ObtenerMenuPorAplicacion
            var rows = await _ado.ExecuteEntitiesAsync(
                "AUTORIZACION.GetMenuByApplication",
                CommandType.StoredProcedure,
                new
                {
                    IdAplicacion = Convert.ToInt32(applicationId)
                }
            );

            return rows.Select(r => r.To<RolesMenuByApplicationQueryFilter>());
        }

        public async Task<IEnumerable<RoleResourceMenuQueryFilter>> GetRoleResourcesForUser(GetMenuQueryFilter getMenuQueryFilter)
        {
            // Antes: Autorizacion.ObtenerMenuPorRolUsuario
            int? idUsuarioSistema = null;
            if (!string.IsNullOrWhiteSpace(getMenuQueryFilter.UserId) &&
                int.TryParse(getMenuQueryFilter.UserId, out var valUsuario) &&
                valUsuario > 0)
            {
                idUsuarioSistema = valUsuario;
            }

            int? idRol = null;
            if (!string.IsNullOrWhiteSpace(getMenuQueryFilter.RoleId) &&
                int.TryParse(getMenuQueryFilter.RoleId, out var valRol) &&
                valRol > 0)
            {
                idRol = valRol;
            }

            var rows = await _ado.ExecuteEntitiesAsync(
                "AUTORIZACION.GetMenuByUserRoles",
                CommandType.StoredProcedure,
                new
                {
                    DocumentoIdentidad = getMenuQueryFilter.DocumentNumber,
                    IdUsuarioSistema = idUsuarioSistema,
                    IdRol = idRol
                }
            );

            return rows.Select(r => r.To<RoleResourceMenuQueryFilter>());
        }

        #endregion

        #region User ↔ Role

        public async Task<IEnumerable<UserRoleQueryFilter>> GetUserRoles(GetUsersByRoleQueryFilter queryFilter)
        {
            int? idUsuario = null;
            if (!string.IsNullOrWhiteSpace(queryFilter.UserId) &&
                int.TryParse(queryFilter.UserId, out var valUsuario) &&
                valUsuario > 0)
            {
                idUsuario = valUsuario;
            }

            int? idRol = null;
            if (!string.IsNullOrWhiteSpace(queryFilter.RoleId) &&
                int.TryParse(queryFilter.RoleId, out var valRol) &&
                valRol > 0)
            {
                idRol = valRol;
            }

            string? docId = string.IsNullOrWhiteSpace(queryFilter.DocumentNumber)
                ? null
                : queryFilter.DocumentNumber.Trim();

            var rows = await _ado.ExecuteEntitiesAsync(
                "AUTORIZACION.GetUsersByRole",
                CommandType.StoredProcedure,
                new
                {
                    IdUsuarioSistema = idUsuario,
                    IdRol = idRol,
                    DocumentoIdentidad = docId
                }
            );

            return rows.Select(r => r.To<UserRoleQueryFilter>());
        }

        public async Task<IEnumerable<UserDataQueryFilter>> GetUserDataByRoleId(string roleId)
        {
            var rows = await _ado.ExecuteEntitiesAsync(
                "AUTORIZACION.GetUsersByRoleId",
                CommandType.StoredProcedure,
                new { IdRol = Convert.ToInt32(roleId) }
            );

            return rows.Select(r => r.To<UserDataQueryFilter>());
        }

        #endregion

        #region User search (General)

        public async Task<IEnumerable<UserByDocOrNameResultQueryFilter>> SearchUsersByCriteria(
            string? searchCriteria,
            bool onlyActive = true,
            int top = 50)
        {
            var parameters = new
            {
                CriterioBusqueda = string.IsNullOrWhiteSpace(searchCriteria) ? null : searchCriteria,
                SoloActivos = onlyActive,
                Top = top
            };

            var rows = await _ado.ExecuteEntitiesAsync(
                nameOrSql: "General.BuscarUsuariosPorDocONombre",
                type: CommandType.StoredProcedure,
                parameters: parameters
            );

            return rows.Select(r => r.To<UserByDocOrNameResultQueryFilter>());
        }

        #endregion
    }
}
