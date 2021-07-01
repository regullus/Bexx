using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    
    public class ConfiguracaoModel
    {
        public int id { get; set; }
        public int idEmpresa { get; set; }
        public string chave { get; set; }
        public string valor { get; set; }
        public string descricao { get; set; }
        public string valorIng { get; set; }
        public string descricaoIng { get; set; }
        public string valorEsp { get; set; }
        public string descricaoEsp { get; set; }
        public int atualizacao { get; set; }
    }

    #region Fixo
    public class ConfiguracaoTipoModel
    {
        public int id { get; set; }
        public int atualizacao { get; set; }
        public string nome { get; set; }
        public string nomeIng { get; set; }
        public string nomeEsp { get; set; }
    }
    #endregion
}