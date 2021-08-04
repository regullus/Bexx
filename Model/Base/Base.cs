using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{    
    public class PaisModel
    {
        public int id { get; set; }
        public int atualizacao { get; set; }
        public string isoCode { get; set; }
        public string isoCode3 { get; set; }
        public string nome { get; set; }
        public string nomeInternacional { get; set; }
        public string idioma { get; set; }
        public string culture { get; set; }
        public string cultureIdioma { get; set; }
        public string cultureIdioma3 { get; set; }
        public string nomeIng { get; set; }
        public string nomeEsp { get; set; }
    }

    public class Response
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }
}