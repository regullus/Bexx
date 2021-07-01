using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class EmpresaModel
    {
        public int id { get; set; }
        public int atualizacao { get; set; }
        public int idCidade { get; set; }
        public int idAdministrador { get; set; }
        public int idIdioma { get; set; }
        public int idAtivo { get; set; }
        public string nome { get; set; }
        public string nomeFantasia { get; set; }
        public string telefone { get; set; }
        public string celular { get; set; }
        public string logradouro { get; set; }
        public string identificacao { get; set; }
        public string responsavel { get; set; }
        public string email { get; set; }
        public int dataValidade { get; set; }
        public string multiLanguage { get; set; }
        public string observacao { get; set; }
    }
}