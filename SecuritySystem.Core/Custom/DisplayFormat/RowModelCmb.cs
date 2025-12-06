namespace SecuritySystem.Core.Custom.DisplayFormat
{
    public class RowModelCmb
    {
        public object Valor { get; set; }
        public object Descripcion { get; set; }

        //private bool estaSeleccionado = false;

        //public bool EstaSeleccionado
        //{
        //    get => estaSeleccionado;
        //    set => estaSeleccionado = false;
        //}

        public bool EstaSeleccionado { get; set; }
        public object? Dominio;
    }
}
