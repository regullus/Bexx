#region using

using System;

#endregion

namespace WebApp.Model
{
    public class CadastroUsuario
    {
        // == Usuario
        public int id { get; set; }
        public int idUserSituacao { get; set; }
        public int idPais { get; set; }
        public int idSexo { get; set; }
        public DateTime dataVencimentoPlano { get; set; }
        public DateTime dataExpiracaoSenha { get; set; }
        public int idIdioma { get; set; }
        public string avatar { get; set; }
        public string avatarUrl { get; set; }
        public string nome { get; set; }
        public string login { get; set; }
        public string senha { get; set; }
        public string perfil { get; set; }
        public string email { get; set; }
        public string celular { get; set; }
        public bool doisFatoresHabilitados { get; set; }
        
        // == UsuarioDados
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
