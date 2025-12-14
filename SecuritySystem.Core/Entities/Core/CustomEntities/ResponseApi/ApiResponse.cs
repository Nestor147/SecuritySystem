using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;
namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public Message[] Messages { get; set; }
        public Pagination Pagination { get; set; }

        public ApiResponse(T dato)
        {
            Data = dato;
        }
    }
}
