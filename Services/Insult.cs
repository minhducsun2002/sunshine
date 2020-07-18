using System.Threading.Tasks;
using System.Net.Http;

namespace sunshine.Services
{
    public class InsultService
    {
        private static string _ = "https://insult.mattbas.org/api/insult";
        private static HttpClient c = new HttpClient();
        public static async Task<string> insult()
        {
            return await c.GetStringAsync(_);
        }
    }
}