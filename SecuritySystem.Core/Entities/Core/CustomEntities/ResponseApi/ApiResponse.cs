using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;
namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi
{
    public class ApiResponse<T>
    {
        public T Datos { get; set; }
        public Message[] Mensajes { get; set; }
        public Pagination Paginacion { get; set; }

        public ApiResponse(T dato)
        {
            Datos = dato;
        }
    }
}
