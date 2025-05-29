namespace Shared.Options
{
    public class JwtValidationOptions
    {
        // {Authority}/.well-known/openid-configuration
        //  ⮕ contains → "jwks_uri": {Authority}/.well-known/jwks.json
        //      ⮕ contains → public keys to verify JWT signature
        public string Authority { get; set; } = string.Empty;
        public bool RequireHttpsMetadata { get; set; }
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
