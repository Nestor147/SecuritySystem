using Newtonsoft.Json;
using System.ComponentModel;

namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.DisplayFormat
{
    public class RowModel
    {
        [DefaultValue("")]
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string TipoContenido { get; set; }

        [DefaultValue("")]
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public dynamic Contenido { get; set; }

        [DefaultValue("")]
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string Accion { get; set; }

        [DefaultValue("")]
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string NombrePagina { get; set; }

        [DefaultValue(new object[] { })]
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public ParameterModel[] Parametros { get; set; }
    }
}
