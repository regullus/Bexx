#region using


#endregion

namespace WebApp.Model
{
    public class DadosPessoais
    {
        public int id { get; set; }    
        public int idPais { get; set; }
        public int idSexo { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public int idIdioma { get; set; }
        public string avatar { get; set; }
        public string celular { get; set; }
        //
        public int idUsuarioDados { get; set; }
        public string SocialTwitch { get; set; }
        public string SocialFacebook { get; set; }
        public string SocialTwitter { get; set; }
        public string SocialYouTube { get; set; }
        public string SocialInstagram { get; set; }
        public string cpf { get; set; }
        public int idInstituicaoFinanceira { get; set; }
        public string agencia { get; set; }
        public string agenciaDigito { get; set; }
        public string conta { get; set; }
        public string contaDigito { get; set; }
        public string chavePix { get; set; }
    }

}
