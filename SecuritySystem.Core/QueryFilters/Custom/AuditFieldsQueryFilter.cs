using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Core.QueryFilters.Custom
{
    public class AuditFieldsQueryFilter
    {
        public int EstadoRegistro { get; set; } = 1;
        public string UsuarioRegistro { get; set; } = "AUTORIZACION";
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
