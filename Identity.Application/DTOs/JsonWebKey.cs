using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.DTOs
{
    public class JsonWebKey
    {
        public string kty { get; set; } = "RSA";  // Key Type
        public string use { get; set; } = "sig";  // Usage (signature)
        public string kid { get; set; } = string.Empty; // Key ID
        public string alg { get; set; } = "RS256"; // Algorithm
        public string n { get; set; } = string.Empty;   // Modulus (base64url)
        public string e { get; set; } = string.Empty;   // Exponent (base64url)
    }
}
