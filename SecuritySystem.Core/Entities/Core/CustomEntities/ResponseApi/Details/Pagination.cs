namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details
{
    public class Pagination
    {
        public int TotalRegistros { get; set; }
        public int TamanoDePagina { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public bool TienePaginaAnterior { get; set; }
        public bool TienePaginaSiguiente { get; set; }

        public Pagination()
        {

        }

        public Pagination(ListaPaginada<object> lista)
        {
            TotalRegistros = lista.TotalRegistros;
            TamanoDePagina = lista.TamanoDePagina;
            PaginaActual = lista.PaginaActual;
            TotalPaginas = lista.TotalPaginas;
            TienePaginaAnterior = lista.TienePaginaAnterior;
            TienePaginaSiguiente = lista.TienePaginaSiguiente;
        }
    }
}