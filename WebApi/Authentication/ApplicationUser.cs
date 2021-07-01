using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Authentication
{
    public class ApplicationUser : IdentityUser<int>
    {
        public int? id_usuario_dados { get; set; }
        public int id_imagem { get; set; }
        public int id_empresa { get; set; }
        public int id_filial { get; set; }
        public int id_user_situacao { get; set; }
        public int id_plano { get; set; }
        public int id_pais { get; set; }
        public int id_sexo { get; set; }
        public int criacao { get; set; }
        public int atualizacao { get; set; }
        public DateTime? data_vencimento_plano { get; set; }
        public DateTime? data_expiracao_senha { get; set; }
        [StringLength(10)]
        public int id_idioma { get; set; }
        [StringLength(100)]
        public string avatar { get; set; }
        public DateTime? termo_uso_em { get; set; }
        // [Column(TypeName = "NVARCHAR")]
        [StringLength(60)]
        public string nome { get; set; }
        [StringLength(50)]
        public string senha { get; set; }
        public string google_authenticator_secretkey { get; set; }

    }
}
