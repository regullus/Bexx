using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class TicketsModel
    {
        public int id { get; set; }
        public int idUsuario { get; set; }
        public string origem { get; set; }
        public string descricao { get; set; }
        public string valor { get; set; }
        public string ticket { get; set; }
        public DateTime criacao { get; set; }
        public DateTime atualizacao { get; set; }
        public int status { get; set; }
    }
}