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
                string pass = GetMd5Hash(RespSIPAccount.INS.GetPass());
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
                w.port = "5160";
                if (w.extension.Length > 2 && w.extension.Length < 10)
                    w.isLogin = true;

            }
            catch (Exception ex)
            {

            }

            return w;
        }
        public static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        private static string MD5H(string rpass)
        {
            if (String.IsNullOrEmpty(rpass)) rpass = "1";
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(rpass));
            var str = Convert.ToBase64String(hash); // Encoding.UTF8.GetString(hash);
            return str;
        }
    }
}
