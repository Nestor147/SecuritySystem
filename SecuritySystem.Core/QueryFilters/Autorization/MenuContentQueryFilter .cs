using Newtonsoft.Json;
using SecuritySystem.Core.QueryFilters.Custom;

namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class MenuContentQueryFilter : AuditFieldsQueryFilter
    {
        #region Properties

        public string Id { get; set; }
        public string Description { get; set; }
        public string Page { get; set; }
        public string Icon { get; set; }
        public bool IsNew { get; set; }
        public bool IsMenuActive { get; set; }
        public bool IsPageActive { get; set; }

        public string ResourceId { get; set; }
        public int ResourceType { get; set; }

        // Compatibilidad con JSON anterior: sigue llegando "IdAplicacion"
        [JsonIgnore]
        [JsonProperty("IdAplicacion")]
        public string ApplicationId { get; set; }

        public string Level { get; set; }
        public int Indentation { get; set; }

        /// <summary>
        /// 1: New record
        /// 2: Edit record
        /// 3: Delete record
        /// </summary>
        public int Status { get; set; }

        public List<MenuContentQueryFilter> SubLinks { get; set; }

        #endregion

        #region Constructors

        public MenuContentQueryFilter()
        {
            SubLinks = new List<MenuContentQueryFilter>();
        }

        #endregion
    }
}
