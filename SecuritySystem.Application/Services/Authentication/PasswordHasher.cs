using SecuritySystem.Application.Interfaces.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Application.Services.Authentication
{
    public class PasswordHasher : IPasswordHasher
    {
        // Implementación simple de ejemplo: CAMBIA ESTO por BCrypt/Argon2
        public string Hash(string password)
        {
            // NO usar en producción, es solo placeholder
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public bool Verify(string password, string passwordHash)
        {
            var hashed = Hash(password);
            return hashed == passwordHash;
        }
    }
}
