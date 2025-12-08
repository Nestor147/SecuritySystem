using Newtonsoft.Json;
using SecuritySystem.Core.QueryFilters.Custom;
using System;
using System.Collections.Generic;

namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class RoleContentQueryFilter : AuditFieldsQueryFilter
    {
        #region Properties

        // Coincide con: rm.Id AS Id
        public int Id { get; set; }

        // Coincide con: rm.ResourceId AS ResourceId
        public int ResourceId { get; set; }

        // Coincide con: r.Name AS Name
        public string Name { get; set; }

        // Coincide con: r.Description AS Description
        public string Description { get; set; }

        // No viene de GetFullMenu, pero lo dejamos por si lo usas en otros flujos
        public string Detail { get; set; }

        // Coincide con: r.Page AS Page
        public string Page { get; set; }

        // Coincide con: r.IconName AS IconName, pero exponemos "Icon" en JSON para compatibilidad
        [JsonProperty("Icon")]
        public string IconName { get; set; }

        // Coincide con: r.IsNew AS IsNew
        public bool IsNew { get; set; }

        // Estas dos banderas no vienen del SP actual; las dejamos por si las manejas en lógica de negocio
        public bool IsMenuActive { get; set; }
        public bool IsPageActive { get; set; }

        // Coincide con: r.ResourceType AS ResourceType
        public int ResourceType { get; set; }

        // Mantiene el JSON como "IdRol" para compatibilidad con el front
        [JsonProperty("IdRol")]
        public string RoleId { get; set; }

        // Coincide con: rm.Level AS Level
        public int Level { get; set; }

        // Coincide con: rm.IndentLevel AS IndentLevel
        // Si tu front envía/recibe "Indentation", se mantiene con JsonProperty
        [JsonProperty("Indentation")]
        public int IndentLevel { get; set; }

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


