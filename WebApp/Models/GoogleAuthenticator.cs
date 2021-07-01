namespace WebApp.Model
{
    public class GoogleAuthenticatorResponse
    {
        public bool ok { get; set; }
        public string qrCodeImage { get; set; }
        public string mensagem { get; set; }
        public string secretkey { get; set; }
        public bool twoFactorEnabled { get; set; }
    }

    public class GoogleAuthenticatorRequest
    {
        public string nome { get; set; }

        public string token { get; set; }

        public string secretkey { get; set; }

    }
}