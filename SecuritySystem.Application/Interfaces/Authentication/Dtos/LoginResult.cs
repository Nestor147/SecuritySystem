using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Application.Interfaces.Authentication.Dtos
{
    public sealed class LoginResult
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public int ApplicationId { get; set; }
        public string ApplicationCode { get; set; }

        public IReadOnlyList<string> Roles { get; set; }
    }
}
