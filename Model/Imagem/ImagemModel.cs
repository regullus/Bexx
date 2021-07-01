using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ImagemModel
    {
        public int id { get; set; }
        public int atualizacao { get; set; }
        public int idEmpresa { get; set; }
        public int idImagemTipo { get; set; }
        public string nome { get; set; }
        public int tamanho { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string nomeIng { get; set; }
        public string nomeEsp { get; set; }
        public string extensao { get; set; }
    }

    #region Fixo
    public class ImagemTipoModel
    {
        public int id { get; set; }
        public int atualizacao { get; set; }
        public string nome { get; set; }
        public string nomeIng { get; set; }
        public string nomeEsp { get; set; }
    }

    #endregion
}