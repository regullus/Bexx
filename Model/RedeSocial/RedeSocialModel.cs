using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class RedeSocialModel
    {
        public int id { get; set; }
        public int idAtivo { get; set; }
        public int idRedeSocialTipo { get; set; }
        public string nome { get; set; }
        public int criacao { get; set; }
    }

    #region Fixo

    public class RedeSocialTipoModel
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string nomeIng { get; set; }
        public string nomeEsp { get; set; }
    }

    #endregion

    #region Custom

    public class RedeSocialSelecionadoModel
    {
        public int id { get; set; }
        public int idAtivo { get; set; }
        public int idRedeSocialTipo { get; set; }
        public string nome { get; set; }
        public int criacao { get; set; }
        public bool selecionado { get; set; }
    }

    #endregion
}