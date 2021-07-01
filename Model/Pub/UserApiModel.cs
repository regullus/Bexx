using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class UserApiModel
    {
        public string token { get; set; }
        public int userID { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public bool confirmed { get; set; }
        public bool confirmedSocialMidia { get; set; }
        public string expiryDate { get; set; }

    }
}
