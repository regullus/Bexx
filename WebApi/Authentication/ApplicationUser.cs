using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Authentication
{
    public class ApplicationUser : IdentityUser<int>
    {
        public int? idUsuarioDados { get; set; }
        public int idEmpresa { get; set; }
        public int idGrupo { get; set; }
        public int idUsuarioSituacao { get; set; }
        public int idArea { get; set; }
        public int idPais { get; set; }
        public int idSexo { get; set; }
        public int idIdioma { get; set; }
        public int criacao { get; set; }
        public int atualizacao { get; set; }
        [StringLength(60)]
        public string nome { get; set; }
        [StringLength(50)]
        public string senha { get; set; }
        public DateTime? dataExpiracaoSenha { get; set; }
        [StringLength(100)]
        public string avatar { get; set; }
        public DateTime? termoUsoEm { get; set; }
        public string googleAuthenticatorSecretKey { get; set; }
    }
}
