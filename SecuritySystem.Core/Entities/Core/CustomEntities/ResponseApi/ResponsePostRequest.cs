using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;
using System.Net;
using System.Text.Json.Serialization;
namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi
{
    public class ResponsePostRequest
    {
        public Message[] Messages { get; set; }
        public string Id { get; set; }
        //public ResponsePostDetail Respuesta { get; set; }
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }

        [JsonIgnore]
        public object ParamRequest { get; set; }
    }
}
