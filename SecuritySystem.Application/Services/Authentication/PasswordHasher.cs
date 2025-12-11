using SecuritySystem.Application.Interfaces.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Application.Services.Authentication
{
    public sealed class BcryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            // workFactor = cost; 12 is a good default for production
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public bool Verify(string password, string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash))
                return false;

            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
