using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class IdModel
    {
        public int id { get; set; }
    }
    public class stringModel
    {
        public string valor { get; set; }
    }

    public class IdAnuncioModel
    {
        public int idAnuncio { get; set; }
    }

    public class WeatherForecast
    {
        public DateTime date { get; set; }
        public int temperatureC { get; set; }
        public int temperatureF => 32 + (int)(temperatureC / 0.5556);
        public string summary { get; set; }
    }

    public class AtivoModel
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string nomeIng { get; set; }
        public string nomeEsp { get; set; }
    }


    public class IdiomaModel
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string nomeIng { get; set; }
        public string nomeEsp { get; set; }
        public string sigla { get; set; }
    }
    public class valorStrModel
    {
        public int id { get; set; }
        public string valor { get; set; }
    }

    public class valorIntModel
    {
        public int id { get; set; }
        public int? valor { get; set; }
    }

}
