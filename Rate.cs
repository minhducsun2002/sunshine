using System.Net.Http;
using System.Threading.Tasks;
using Disqord.Bot;
using Qmmands;

namespace sunshine
{
    public class Rate : DiscordModuleBase
    {
        private HttpClient httpClient = new();

        [Command("rate")]
        public async Task<DiscordCommandResult> Exec([Remainder] string query = "")
        {
            var pick = await httpClient.GetStringAsync(
                $"https://www.random.org/integers/?num=1&min=1&max={101}&col=1&base=10&format=plain&rnd=new"
            );
            if (string.IsNullOrWhiteSpace(query)) return Reply("I rate the void 100%.");
            var rating = int.Parse(pick) - 1;
            return Reply($"I would give \"**{query}**\" a rating of {rating}%.");
        }
    }
}