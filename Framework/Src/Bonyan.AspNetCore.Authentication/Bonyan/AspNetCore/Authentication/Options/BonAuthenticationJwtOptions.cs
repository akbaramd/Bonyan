namespace Bonyan.AspNetCore.Authentication.Options
{
    public class BonAuthenticationJwtOptions
    {
        public bool Enabled { get; set; } = false;
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public bool RequireHttpsMetadata { get; set; } = false;
        public bool SaveToken { get; set; } = true;
        public double ExpirationInMinutes { get; set; } = 60;
    }
}