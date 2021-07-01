using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class MensagemModel
    {
        public int id { get; set; }
        public int idDestino { get; set; }
        public int idUsuarioOrigem { get; set; }
        public int idUsuarioDestino { get; set; }
        public int idAtivo { get; set; }
        public int idEmpresa { get; set; }
        public int idMensagemTipo { get; set; }
        public string texto { get; set; }
        public string textoIng { get; set; }
        public string textoEsp { get; set; }
        public DateTime data { get; set; }
        public int validade { get; set; }
        public bool urgente { get; set; }
    }

    #region Fixo
    public class MensagemTipoModel
    {
        public int id { get; set; }
        public int atualizacao { get; set; }
        public string nome { get; set; }
        public string nomeIng { get; set; }
        public string nomeEsp { get; set; }
    }

    #endregion
}