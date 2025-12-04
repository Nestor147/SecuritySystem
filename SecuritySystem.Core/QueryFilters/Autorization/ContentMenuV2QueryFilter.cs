using Newtonsoft.Json;
using SecuritySystem.Core.QueryFilters.Custom;

namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class ContentMenuV2QueryFilter : AuditFieldsQueryFilter
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

        [JsonIgnore]
        [JsonProperty("IdAplicacion")]
        public string ApplicationId { get; set; }

        public string Level { get; set; }
        public int Indentation { get; set; }

        /// <summary>
        /// 1: New record
        /// 2: Update record
        /// 3: Delete record
        /// </summary>
        public int Status { get; set; }

        public List<ContentMenuV2QueryFilter> SubLinks { get; set; }

        #endregion

        #region Constructors

        public ContentMenuV2QueryFilter()
        {
            SubLinks = new List<ContentMenuV2QueryFilter>();
        }

        #endregion
    }
}
