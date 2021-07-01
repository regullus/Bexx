#region using


using System;
using System.Collections.Generic;


#endregion

namespace WebApp.Model
{
    public class ExtratoItem
    {
        public int? idAnuncio { get; set; }
        public string nomeAnuncio { get; set; }
        public DateTime data { get; set; }
        public string valor { get; set; }
        public string descricao { get; set; }
        public string tipo { get; set; }
    }
}
