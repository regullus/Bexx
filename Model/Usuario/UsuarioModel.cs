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
        public int id { get; set; }
        public int idAtivo { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string senha { get; set; }
        public string regras { get; set; }
        public string token { get; set; }
        public string refreshToken { get; set; }
        public string idioma { get; set; }
        public DateTime expiracao { get; set; }
        public string avatar { get; set; }
        public bool doisFatoresHabilitado { get; set; }
        public string autenticadorGoogleChaveSecreta { get; set; }
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
        public int idGrupo { get; set; }
        public int idUsuarioSituacao { get; set; }
        public int idArea { get; set; }
        public int idPais { get; set; }
        public int idSexo { get; set; }
        public DateTime dataExpiracaoSenha { get; set; }
        public int idIdioma { get; set; }
        public string avatar { get; set; }
        public bool doisFatoresHabilitado { get; set; }
        public string autenticadorGoogleChaveSecreta { get; set; }
        public string perfil { get; set; }
    }

    public class UsuarioDadosModel
    {
        public int id { get; set; }
        [Required]
        public int atualizacao { get; set; }
        [Required]
        public int dataUltimoAcesso { get; set; }
        [Required]
        public bool exibeBoasVindas { get; set; }
        public string twitch { get; set; }
        public string facebook { get; set; }
        public string twitter { get; set; }
        public string youtube { get; set; }
        public string instagram { get; set; }
    }

    public class UsuarioModel
    {
        public int id { get; set; }
        public int idUsuarioDados { get; set; }
        public int idImagem { get; set; }
        public int idEmpresa { get; set; }
        public int idGrupo { get; set; }
        public int idArea { get; set; }
        public int idUsuarioSituacao { get; set; }
        public int idPais { get; set; }
        public int idSexo { get; set; }
        public int idIdioma { get; set; }
        public int atualizacao { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string login { get; set; }
        public string senha { get; set; }
        public bool doisFatoresHabilitados { get; set; }
        public DateTime dataExpiracaoSenha { get; set; }
        public string avatar { get; set; }
        public string celular { get; set; }
        public bool celularConfirmado { get; set; }
    }

    public class NovaSenhaModel
    {
        public string password { get; set; }
        public string confirmarPassword { get; set; }
        public string token { get; set; }
        public string token2 { get; set; }
    }

    public class DoisFatoresModel
    {
        public int id { get; set; }
        public bool doisFatoresHabilitado { get; set; }
        public string autenticadorGoogleChaveSecreta { get; set; }
    }

    public class UsuarioNomeModel
    {
        [Required(ErrorMessage = "UserName é necessário")]
        public string usuarioNome { get; set; }
    }

    public class UsuarioNomeTokenModel
    {
        [Required(ErrorMessage = "Usuário é necessário")]
        public string usuarioNomeToken { get; set; }
    }

    public class UsuarioEmailModel
    {
        [Required(ErrorMessage = "email é necessário")]
        public string email { get; set; }
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