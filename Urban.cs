using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Disqord;
using Disqord.Bot;
using Disqord.Extensions.Interactivity.Menus.Paged;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qmmands;

namespace sunshine
{
    internal class UrbanPagedView : PagedView
    {
        public UrbanPagedView(PageProvider pageProvider) : base(pageProvider)
        {
            FirstPageButton.Label = "First definition"; FirstPageButton.Emoji = null;
            LastPageButton.Label = "Last definition"; LastPageButton.Emoji = null;
            PreviousPageButton.Label = "Previous"; PreviousPageButton.Emoji = null;
            NextPageButton.Label = "Next"; NextPageButton.Emoji = null;
            RemoveComponent(StopButton);
        }
    }

    internal class UrbanResponse
    {
        [JsonProperty("definition")] public string Definition { get; set; }
        [JsonProperty("example")] public string Example { get; set; }
        [JsonProperty("permalink")] public string Permalink { get; set; }
        [JsonProperty("thumbs_up")] public int Likes { get; set; }
        [JsonProperty("thumbs_down")] public int Dislikes { get; set; }
    }


    public class Urban : DiscordModuleBase
    {
        private readonly HttpClient httpClient = new();
        private const int MaxLen = 1000;

        [Command("urban")]
        public async Task<DiscordCommandResult> Exec([Remainder] string query = "")
        {
            if (string.IsNullOrEmpty(query)) return Reply("I saw nothing to search about!");

            var response = await httpClient.GetStringAsync(
                $"http://api.urbandictionary.com/v0/define?term=${HttpUtility.UrlEncode(query)}"
            );

            var _ = ((JObject)JsonConvert.DeserializeObject(response))["list"]
                .ToObject<List<UrbanResponse>>();
            if (_ == null || _.Count < 1) return Reply("I found no results for your query.");

            _.Sort(
                (record1, record2) =>
                    record1.Likes - record1.Dislikes - (record2.Likes - record2.Dislikes)
            );

            var embeds = _.Select((chosen, index) =>
            {
                string def = chosen.Definition, eg = chosen.Example;
                if (string.IsNullOrEmpty(eg.Trim())) eg = "[no example]";
                var embed = new LocalEmbed()
                    .WithAuthor("Urban Dictionary", "https://vgy.me/ScvJzi.jpg")
                    .WithTitle($"Urban Dictionary definition for **{query}**")
                    .WithUrl(chosen.Permalink)
                    .AddField(
                        "Definition",
                        def[..Math.Min(MaxLen, def.Length)]
                        + (def.Length > 1000 ? "..." : "")
                    )
                    .AddField(
                        "Example",
                        eg[..Math.Min(MaxLen, eg.Length)]
                        + (eg.Length > 1000 ? "..." : "")
                    )
                    .WithFooter($"Definition {index + 1} of {_.Count} | {chosen.Likes} 👍 | {chosen.Dislikes} 👎")
                    .WithTimestamp(DateTime.Now)
                    .WithColor(Color.Green);
                return new Page
                {
                    Embeds = new[] { embed }
                };
            }).ToArray();

            return View(new UrbanPagedView(new ListPageProvider(embeds)));
        }
    }
}