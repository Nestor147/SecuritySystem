using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;
using System.Net;
using System.Text.Json.Serialization;
namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi
{
    public class ResponseGetPagination
    {
        public PaginatedList<object> Paginacion { get; set; }
        public Message[] Mensajes { get; set; }
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }       
    }
}

