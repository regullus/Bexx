#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace WebApp.Model
{
    public class ReCaptchaSettings
    {
        public string ReCAPTCHA_Site_Key { get; set; }
        public string ReCAPTCHA_Secret_Key { get; set; }
    }
}
