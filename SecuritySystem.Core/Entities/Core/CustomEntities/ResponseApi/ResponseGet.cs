using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;
namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi
{
    public class ResponseGet
    {
        public IEnumerable<object> Data { get; set; }
        public Message[] Messages { get; set; }
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
    }
}

