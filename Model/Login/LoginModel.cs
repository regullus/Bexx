using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class LoginInputModel
    {
        [Required]
        public string login { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string token { get; set; }
        public bool lembrar { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string confirmarPassword { get; set; }
        public string aceite { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Usuário é necessario")]
        public string usuario { get; set; }

        [Required(ErrorMessage = "Senha é necessária")]
        public string password { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "nome é necessário")]
        public string nome { get; set; }
        [Required(ErrorMessage = "Usuário é necessário")]
        public string usuario { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email é necessário")]
        public string email { get; set; }
        [Required(ErrorMessage = "Senha é necessária")]
        public string password { get; set; }
        public int? idUsuarioDados { get; set; }
        public int idEmpresa { get; set; }
        public int idGrupo { get; set; }
        public int idArea { get; set; }
        public int idUsuarioSituacao { get; set; }
        public int idPais { get; set; }
        public int idSexo { get; set; }
        public DateTime? dataExpiracaoSenha { get; set; }
        public int idIdioma { get; set; }
        public string avatar { get; set; }
        public string perfil { get; set; }
    }
}
