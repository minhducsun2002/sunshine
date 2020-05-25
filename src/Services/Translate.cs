using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace sunshine.Services
{
    public class TranslateService
    {
        public struct languages {
            public Dictionary<string, string> sl;
            public Dictionary<string, string> tl;
        }
        private HttpClient client = new HttpClient();

        public languages getLanguages()
        {
            languages _;
            var a = (JObject)
                JsonConvert.DeserializeObject(client.GetStringAsync("https://translate.googleapis.com/translate_a/l?client=gtx").Result);
            _.sl = a.GetValue("sl").Value<JObject>().ToObject<Dictionary<string, string>>();
            _.tl = a.GetValue("tl").Value<JObject>().ToObject<Dictionary<string, string>>();
            return _;
        }

        public Tuple<string, string> translate(string src, string dst, string content)
        {
            var a = (JArray)
                JsonConvert.DeserializeObject(
                    client.GetStringAsync($@"
                        https://translate.googleapis.com/translate_a/single?client=gtx&sl={
                            src
                        }&tl={dst}&dt=t&q={HttpUtility.UrlEncode(content)}
                    ").Result
                );
            var _ = (JArray)((JArray) a[0])[0];
            return new Tuple<string, string>(
                _[0].Value<string>(),
                a[2].Value<string>()
            );
        }
    }
}
