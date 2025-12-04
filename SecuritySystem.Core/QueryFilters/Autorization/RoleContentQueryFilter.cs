using Newtonsoft.Json;
using SecuritySystem.Core.QueryFilters.Custom;

namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class RoleContentQueryFilter : AuditFieldsQueryFilter
    {
        #region Properties
        public string Id { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }
        public string Page { get; set; }
        public string Icon { get; set; }
        public bool IsNew { get; set; }
        public bool IsMenuActive { get; set; }
        public bool IsPageActive { get; set; }
        public string ResourceId { get; set; }
        public int ResourceType { get; set; }

        // Mantengo el JSON como "IdRol" para compatibilidad
        [JsonIgnore]
        [JsonProperty("IdRol")]
        public string RoleId { get; set; }

        public string Level { get; set; }
        public int Indentation { get; set; }

        /// <summary>
        /// 1: New Record
        /// 2: Edit Record
        /// 3: Delete Record
        /// </summary>
        [JsonIgnore]
        public int Status { get; set; }

        public List<RoleContentQueryFilter> SubLinks { get; set; }
        #endregion

        #region Constructors
        public RoleContentQueryFilter()
        {
            SubLinks = new List<RoleContentQueryFilter>();
        }
        #endregion
    }
}
