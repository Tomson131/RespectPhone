using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RespectPhone
{
    public class WebLogin
    {
        public string extension { get; set; } = "";
        public string secret { get; set; } = "";
        public string host{ get; set; } = "";
        public string port { get; set; } = "";
        public string name { get; set; } = "";
        public bool isLogin { get; set; } = false;
        public WebLogin()
        {
        }
    }
    public class RespSIPAccount
    {
        public static RespSIPAccount _ins;
        public static string conf_file = "aster_ari.config";

        [JsonIgnore]
        public bool isLogin = false;
        

        public static RespSIPAccount INS
        {
            get
            {
                if (_ins == null)
                    _ins = new RespSIPAccount();
                return _ins;
            }
        }

        internal string base_url { get; set; }= "https://e.respectrb.ru/api/aster_sip_detail/";
        public bool registrationRequired { get; set; } = true;

        public void SetExt(WebLogin w)
        {
            if (!w.isLogin) return;

            isLogin = w.isLogin;
            userName = w.extension;
            authenticationId = w.extension;
            displayName = w.name;
            domainHost = w.host + ":" + w.port;
            registerPassword = w.secret;

        }

        public string displayName { get; set; } = "test";
        public string userName { get; set; } = "4777";
        public string authenticationId { get; set; } = "4777";
        public string registerPassword { get; set; } = "1q2w3e4r";
        public string domainHost { get; set; } = "aster.institutrb.ru:5160";
        public string domainPort { get; set; } = "";
        public string rlogin { get; set; } = "";
        public string rpass { get; set; } = "";


        public static void ReadConf()
        {
            try
            {

                if (File.Exists(conf_file))
                {
                    var c = File.ReadAllText(conf_file);
                    RespSIPAccount.INS.ParseConf(c);
                }
                else
                {
                    File.WriteAllText(conf_file, JsonConvert.SerializeObject(INS));
                }

                Console.WriteLine(JsonConvert.SerializeObject(INS));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }      
        private void ParseConf(string c)
        {
            try
            {
                _ins = JsonConvert.DeserializeObject<RespSIPAccount>(c);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
