namespace BankMore.Transfer.Api.Configuration
{
    public class JwtConfiguration
    {
        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int Expires { get; set; }
    }
}
