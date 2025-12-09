using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Application.Exceptions
{
    public class AppNotFoundException : Exception
    {
        public AppNotFoundException(string message) : base(message)
        {
        }
    }
}
