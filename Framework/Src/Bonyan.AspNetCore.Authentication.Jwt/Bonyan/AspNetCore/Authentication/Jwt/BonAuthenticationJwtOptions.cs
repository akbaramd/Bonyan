namespace Bonyan.AspNetCore.Authentication.Jwt
{
    public class BonAuthenticationJwtOptions
    {
        public bool Enabled { get; set; } = true;
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public bool RequireHttpsMetadata { get; set; } = true;
        public bool SaveToken { get; set; } = true;
        public TimeSpan ClockSkew { get; set; } = TimeSpan.Zero;
    }
} 