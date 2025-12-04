using SecuritySystem.Core.QueryFilters.Custom;

namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class ResourceQueryFilter : AuditFieldsQueryFilter
    {
        public string Id { get; set; }
        public string ResourceId { get; set; }
        public string PageId { get; set; }
        public string NodeId { get; set; }

        public string? ModuleId { get; set; }
        // public string Module { get; set; }

        public string ApplicationId { get; set; }
        public string ApplicationName { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }

        public int ResourceType { get; set; }

        public string IconName { get; set; }
        public bool IsGhost { get; set; }

        public bool IsNew { get; set; }
        public object SubLinks { get; set; }

        // NombreIcono > Icono (Authorization V2)
        public string Icon { get; set; }

        // Nombre > Pagina (Authorization V2)
        public string? Page { get; set; }
    }
}
