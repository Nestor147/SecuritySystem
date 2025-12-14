using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;
using System.Net;
using System.Text.Json.Serialization;
namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi
{
    public class ResponsePost
    {
        public Message[] Messages { get; set; }
        public string Id { get; set; }
        public ResponsePostDetail Response { get; set; }
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
    }
}
