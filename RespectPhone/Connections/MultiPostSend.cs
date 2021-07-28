using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RespectPhone.Connections
{
    public class MultiPostSendAPI
    {
        public HttpContent content = new MultipartFormDataContent();

        //public FormUrlEncodedContent form_content = new FormUrlEncodedContent();
        public List<KeyValuePair<string, string>> paramList = new List<KeyValuePair<string, string>>();
        public string resp;
        public List<string> _cookies = new List<string>();
        public string tempFname = "temp";
        public List<KeyValuePair<string, string>> cstm_headers = new List<KeyValuePair<string, string>>();
        public HttpResponseHeaders headers = null;

        private string GetCookie(List<string> li)
        {
            var s = "";
            foreach (var l in li)
            {
                s += l + ";";
            }
            return s;
        }
        public void AddParam(string name, string value, MultiParamTypeAPI type, byte[] cont = null)
        {
            try
            {
                switch (type)
                {
                    case MultiParamTypeAPI.Field:
                        var valtxt = new StringContent(value);
                        valtxt.Headers.Remove("Content-Type");
                        valtxt.Headers.Add("Content-Type", "application/www-form-urlencoded");
                        paramList.Add(new KeyValuePair<string, string>(name, value));
                        break;
                    case MultiParamTypeAPI.Raw:
                        content = new StringContent(value, Encoding.UTF8, "application/json");

                        break;
                }
            }
            catch (Exception ex)
            {


            }
        }


        public async Task<string> Post(string _url)
        {
            var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false });

            //client.DefaultRequestHeaders.Add("Host", "www.cccone.com");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:82.0) Gecko/20100101 Firefox/82.0");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json, text/plain, */* ");
            // client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
            //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");           

            // client.DefaultRequestHeaders.Add("Origin", "https://www.cccone.com");
            // client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            // client.DefaultRequestHeaders.Add("Referer", "https://www.cccone.com/login");*/
            //client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            //client.DefaultRequestHeaders.Add("", "");

            // client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json, text/plain, */* ");
            if(_cookies.Count>0)
                client.DefaultRequestHeaders.Add("Cookie", GetCookie(_cookies));



            client.BaseAddress = new Uri(_url);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var mt = HttpMethod.Post;

            HttpRequestMessage request = new HttpRequestMessage(mt, _url);


            //request.Content = form_content;            
            var result = new HttpResponseMessage();

            result = await client.PostAsync(_url, new FormUrlEncodedContent(paramList));
            var res = await result.Content.ReadAsStringAsync();

            return res;
        }


        public async Task<HttpResponseMessage> Get(string _url)
        {


            var client = new HttpClient(new HttpClientHandler { UseCookies = false });


            //client.DefaultRequestHeaders.Add("Cookie", cookie);
            //client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:76.0) Gecko/20100101 Firefox/76.0");
            foreach (var c in _cookies)
            {
                client.DefaultRequestHeaders.Add("Cookie", c);
            }

            client.BaseAddress = new Uri(_url);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var mt = HttpMethod.Get;

            HttpRequestMessage request = new HttpRequestMessage(mt, _url);
            //request.Content = new StringContent("");


            client.Timeout = new TimeSpan(0, 0, 30000);
            var result = new HttpResponseMessage();

            result = await client.SendAsync(request);

            return result;
        }
      
        public void SetCookie(HttpResponseHeaders headers)
        {
            foreach (var c in headers)
            {
                if (c.Key == "Set-Cookie")
                {
                    _cookies.AddRange(c.Value.ToList());
                }

            }
        }



    }

    public enum MultiParamTypeAPI
    {
        File,
        Field,
        Raw
    }
}
