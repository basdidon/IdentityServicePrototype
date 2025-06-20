﻿namespace Shared.Options
{
    public class JwtIssuerOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string PrivateKeyPath { get; set; } = string.Empty;
        public string PublicKeyPath { get; set; } = string.Empty;
        public string KeyId { get; set; } = string.Empty;
    }
}
