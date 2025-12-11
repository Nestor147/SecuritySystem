using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Core.Entities.SealedAuthentication
{
    public sealed class RefreshRequest
    {
        public string RefreshToken { get; set; } = default!;
    }
}
