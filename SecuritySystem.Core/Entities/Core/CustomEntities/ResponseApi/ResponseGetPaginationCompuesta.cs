using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;
using System.Net;
using System.Text.Json.Serialization;

namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi
{
    public class ResponseGetPaginationCompuesta
    {
        public object Data { get; set; }

        public Pagination Pagination { get; set; }

        public Message[] Messages { get; set; }
        [JsonIgnore]

        public HttpStatusCode StatusCode { get; set; }
    }
}
