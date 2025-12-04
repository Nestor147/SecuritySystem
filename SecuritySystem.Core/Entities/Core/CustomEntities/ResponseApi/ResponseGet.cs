using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;
namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi
{
    public class ResponseGet
    {
        public IEnumerable<object> Datos { get; set; }
        public Message[] Mensajes { get; set; }
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
    }
}

