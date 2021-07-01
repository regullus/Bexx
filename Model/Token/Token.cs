using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TokenRefreshModel
    {
        public int id { get; set; }
        public int idAtivo { get; set; }
        public int idUsuario { get; set; }
        public string idJWT { get; set; }
        public string token { get; set; }
        public bool revogado { get; set; }
        public DateTime dataAdicao { get; set; }
        public DateTime dataExpiracao { get; set; }
    }

    public class TokenRequest
    {
        public int idUsuario { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }

}
