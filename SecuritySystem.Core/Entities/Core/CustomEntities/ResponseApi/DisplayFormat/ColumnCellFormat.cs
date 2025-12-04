namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.DisplayFormat
{
    public class ColumnCellFormat
    {
        public string NombreColumna { get; set; }
        public RowModel[] ContenidoCelda { get; set; }

        public static ColumnCellFormat Crear(string nombreColumna, RowModel[] contenidoCelda)
        {
            return new ColumnCellFormat() { NombreColumna = nombreColumna, ContenidoCelda = contenidoCelda };
        }
    }
}
