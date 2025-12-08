namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.DisplayFormat
{
    public class ColumnCellFormat
    {
        public string ColumnName { get; set; }
        public RowModel[] CellContent { get; set; }

        public static ColumnCellFormat Create(string columnName, RowModel[] cellContent)
        {
            return new ColumnCellFormat()
            {
                ColumnName = columnName,
                CellContent = cellContent
            };
        }
    }

}
