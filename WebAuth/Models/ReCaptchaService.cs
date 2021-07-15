#region using

using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

#endregion

namespace WebApp.Model
{
    public class ReCaptchaService
    {
        private ReCaptchaSettings _settings;

        public ReCaptchaService(IOptions<ReCaptchaSettings> settings)
        {
            _settings = settings.Value;
        }

        public virtual async Task<reCaptchaResponse> VerifyReCaptcha(string _Token)
        {
            reCaptchaData _Data = new reCaptchaData
            {
                response = _Token,
                secret = _settings.ReCAPTCHA_Secret_Key
            };

            HttpClient client = new HttpClient();

            var repponse = await client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={_Data.secret}&response={_Data.response}");

            var capResp = JsonConvert.DeserializeObject<reCaptchaResponse>(repponse);

            return capResp;

        }
    }

    public class reCaptchaData 
    {
        public string response { get; set; }
        public string secret { get; set; }
    }

    public class reCaptchaResponse
    {
        public bool success { get; set; }
        public double score { get; set; }
        public string action { get; set; }
        public DateTime challenge_ts { get; set; }
        public string hostname { get; set; }
    }

}
