//namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;

//    public class ListaPaginada<T> : List<T>
//    {
//        public int PaginaActual { get; set; }
//        public int TotalPaginas { get; set; }
//        public int TamanoDePagina { get; set; }
//        public int TotalRegistros { get; set; }

//        public bool TienePaginaAnterior => PaginaActual > 1;
//        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;
//        public int? NumeroPaginaSiguiente => TienePaginaSiguiente ? PaginaActual + 1 : null;
//        public int? NumeroPaginaAnterior => TienePaginaAnterior ? PaginaActual - 1 : null;

//        public ListaPaginada(List<T> datos, int count, int numeroDePagina, int tamanoDePagina)
//        {
//            TotalRegistros = count;
//            TamanoDePagina = tamanoDePagina;
//            PaginaActual = numeroDePagina;
//            TotalPaginas = (int)Math.Ceiling(TotalRegistros / (double)TamanoDePagina);

//            AddRange(datos);
//        }

//        public ListaPaginada(IEnumerable<T> datos, Pagination paginacion)
//        {
//            TotalRegistros = paginacion.TotalRegistros;
//            TamanoDePagina = paginacion.TamanoDePagina;
//            PaginaActual = paginacion.PaginaActual;
//            TotalPaginas = paginacion.TotalPaginas;
//            AddRange(datos);
//        }

//        public static ListaPaginada<T> Crear(IEnumerable<T> registros, int numeroDePagina, int tamanoDePagina)
//        {
//            var count = registros.Count();
//            var datos = registros.Skip((numeroDePagina - 1) * tamanoDePagina).Take(tamanoDePagina).ToList();
//            return new ListaPaginada<T>(datos, count, numeroDePagina, tamanoDePagina);
//        }

//        public static IEnumerable<T> Paginar(IEnumerable<T> registros, int numeroDePagina, int tamanoDePagina, out Pagination paginacion)
//        {
//            paginacion = new Pagination();
//            paginacion.TotalRegistros = registros.Count();
//            paginacion.TamanoDePagina = tamanoDePagina;
//            paginacion.PaginaActual = numeroDePagina;
//            paginacion.TotalPaginas = (int)Math.Ceiling(paginacion.TotalRegistros / (double)paginacion.TamanoDePagina);
//            paginacion.TienePaginaAnterior = paginacion.PaginaActual > 1;
//            paginacion.TienePaginaSiguiente = paginacion.PaginaActual < paginacion.TotalPaginas;
//            var datos = registros.Skip((numeroDePagina - 1) * tamanoDePagina).Take(tamanoDePagina);
//            return datos;
//        }
//    }
//}
namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PaginatedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int? NextPageNumber => HasNextPage ? CurrentPage + 1 : (int?)null;
        public int? PreviousPageNumber => HasPreviousPage ? CurrentPage - 1 : (int?)null;

        public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalRecords = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(TotalRecords / (double)PageSize);

            AddRange(items);
        }

        public PaginatedList(IEnumerable<T> items, Pagination pagination)
        {
            TotalRecords = pagination.TotalRecords;
            PageSize = pagination.PageSize;
            CurrentPage = pagination.CurrentPage;
            TotalPages = pagination.TotalPages;

            AddRange(items);
        }

        public static PaginatedList<T> Create(IEnumerable<T> records, int pageNumber, int pageSize)
        {
            var count = records.Count();
            var items = records
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }

        public static IEnumerable<T> Paginate(
            IEnumerable<T> records,
            int pageNumber,
            int pageSize,
            out Pagination pagination)
        {
            pagination = new Pagination
            {
                TotalRecords = records.Count(),
                PageSize = pageSize,
                CurrentPage = pageNumber
            };

            pagination.TotalPages = (int)Math.Ceiling(
                pagination.TotalRecords / (double)pagination.PageSize);

            pagination.HasPreviousPage = pagination.CurrentPage > 1;
            pagination.HasNextPage = pagination.CurrentPage < pagination.TotalPages;

            var items = records
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return items;
        }
    }
}
