using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class IDUsuarioModel
    {
        public int idUsuario { get; set; }
    }

    public class UsuarioClaimModel
    {
        public int Id { get; set; }
        public int idAtivo { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Roles { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Idioma { get; set; }
        public DateTime Expiracao { get; set; }
        public string Avatar { get; set; }
        public bool DoisFatoresHabilitado { get; set; }
        public string AutenticadorGoogleChaveSecreta { get; set; }
        public int idFinanceiroConta { get; set; }
        public int idPais { get; set; }
    }

    public class UsuarioCadastroModel
    {
        public string nome { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int idUsuarioDados { get; set; }
        public int idImagem { get; set; }
        public int idEmpresa { get; set; }
        public int idFilial { get; set; }
        public int idUserSituacao { get; set; }
        public int idPlano { get; set; }
        public int idPais { get; set; }
        public int idSexo { get; set; }
        public DateTime dataVencimentoPlano { get; set; }
        public DateTime dataExpiracaoSenha { get; set; }
        public int idIdioma { get; set; }
        public string avatar { get; set; }
        public bool DoisFatoresHabilitado { get; set; }
        public string AutenticadorGoogleChaveSecreta { get; set; }
        public string perfil { get; set; }
    }

    public class UsuarioDadosModel
    {
        public int id { get; set; }
        [Required]
        public int Atualizacao { get; set; }
        [Required]
        public int DataUltimoAcesso { get; set; }
        [Required]
        public bool ExibeBoasVindas { get; set; }
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

    public class UsuarioModel
    {
        public int id { get; set; }
        public int idUsuarioDados { get; set; }
        public int idImagem { get; set; }
        public int idEmpresa { get; set; }
        public int idFilial { get; set; }
        public int idUserSituacao { get; set; }
        public int idPlano { get; set; }
        public int idPais { get; set; }
        public int idSexo { get; set; }
        public int atualizacao { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string login { get; set; }
        public string senha { get; set; }
        public bool doisFatoresHabilitados { get; set; }
        public DateTime dataVencimentoPlano { get; set; }
        public DateTime dataExpiracaoSenha { get; set; }
        public int idIdioma { get; set; }
        public string avatar { get; set; }
        public string celular { get; set; }
        public bool celularConfirmado { get; set; }
    }

    public class NovaSenhaModel
    {
        public string Password { get; set; }
        public string ConfirmarPassword { get; set; }
        public string Token { get; set; }
        public string Token2 { get; set; }
    }

    public class DoisFatoresModel
    {
        public int Id { get; set; }
        public bool DoisFatoresHabilitado { get; set; }
        public string AutenticadorGoogleChaveSecreta { get; set; }
    }

    public class UserNameModel
    {
        [Required(ErrorMessage = "UserName é necessário")]
        public string UserName { get; set; }
    }

    public class UserNameTokenModel
    {
        [Required(ErrorMessage = "UserName é necessário")]
        public string UserNameToken { get; set; }
    }

    public class UserEmailModel
    {
        [Required(ErrorMessage = "email é necessário")]
        public string Email { get; set; }
    }

    public class UsuarioListaCrudModel
    {
        public int id { get; set; }  
        public string nome { get; set; }    
        public string login { get; set; }      
    }

    #region Fixo

    public class UsuarioSexoModel
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string nomeIng { get; set; }
        public string nomeEsp { get; set; }

    }

    public class UsuarioPaisModel
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string nomeIng { get; set; }
        public string nomeEsp { get; set; }
        public string nomeInternacional { get; set; }
        public string cultura { get; set; }
    }

    public class UsuarioSituacaoModel
    {
        public int id { get; set; }
        public int criacao { get; set; }
        public string nome { get; set; }
        public string nomeIng { get; set; }
        public string nomeEsp { get; set; }
    }

    #endregion
}