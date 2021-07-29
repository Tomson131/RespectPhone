using Newtonsoft.Json.Linq;
using RespectPhone.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RespectPhone.Connections
{
    public class WebAPIRequest
    {

        public  async static Task<WebLogin> RespLogin()
        {
            WebLogin w = new WebLogin();
            try
            {
                string pass = MD5H(RespSIPAccount.INS.rpass);
                MultiPostSendAPI send = new MultiPostSendAPI();
                send.AddParam("login", RespSIPAccount.INS.rlogin, MultiParamTypeAPI.Field);
                send.AddParam("password",pass , MultiParamTypeAPI.Field);
                send.AddParam("token", "875f374c9d00014fb3a05a85905f58cf", MultiParamTypeAPI.Field);
                var res = await send.Post(RespSIPAccount.INS.base_url);
                var j = JObject.Parse(res);
                w.host = JSONHelper.GetString(j["host"]);
                w.secret= JSONHelper.GetString(j["password"]);
                w.extension = JSONHelper.GetString(j["extension"]);
                w.name = JSONHelper.GetString(j["user_name"]);
                w.port = JSONHelper.GetString(j["port"]);

                if (w.extension.Length > 2)
                    w.isLogin = true;

            }
            catch (Exception ex)
            {

            }

            return w;
        }

        private static string MD5H(string rpass)
        {
            if (String.IsNullOrEmpty(rpass)) rpass = "1";
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(rpass));
            var str = Encoding.UTF8.GetString(hash);
            return str;
        }
    }
}
