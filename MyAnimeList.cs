using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Disqord;
using Disqord.Bot;
using Disqord.Extensions.Interactivity.Menus.Paged;
using Newtonsoft.Json.Linq;
using Qmmands;

namespace sunshine
{
    internal class AnimePagedView : PagedView
    {
        public AnimePagedView(PageProvider pageProvider) : base(pageProvider)
        {
            FirstPageButton.Label = "First result"; FirstPageButton.Emoji = null;
            LastPageButton.Label = "Last result"; LastPageButton.Emoji = null;
            PreviousPageButton.Label = "Previous"; PreviousPageButton.Emoji = null;
            NextPageButton.Label = "Next"; NextPageButton.Emoji = null;
            RemoveComponent(StopButton);
        }
    }
    
    public class MyAnimeList : DiscordModuleBase
    {
        private readonly HttpClient client = new();
        private const string BaseURL = "https://api.jikan.moe/v3";

        private async Task<string> BaseSearch(string obj, string q)
            => await client.GetStringAsync($"{BaseURL}/search/{obj}?q={HttpUtility.UrlEncode(q)}");

        [Command("anime")]
        [Description("Search for an anime")]
        public async Task<DiscordCommandResult> Anime([Remainder] string query = "")
        {
            if (string.IsNullOrWhiteSpace(query)) return Reply("I saw nothing to search about!");
            var response = await BaseSearch("anime", query);
            var animes = JObject.Parse(response)["results"].ToObject<Anime[]>();

            if (animes.Length == 0)
                return Reply("I found no results. Are you sure you aren't searching for illegal stuff?");

            var embeds = animes.Select(anime =>
            {
                var embed = new LocalEmbed
                {
                    Title = anime.Title,
                    Url = anime.Url,
                    ImageUrl = anime.ImageUrl,
                    Description = anime.Synopsis,
                    Fields = new List<LocalEmbedField>
                    {
                        new()
                        {
                            Name = "Statistics",
                            Value = $"**Type** : {anime.Type}\n**Episodes** : {anime.Episodes}\n**Score** : {anime.Score}"
                        },
                        new()
                        {
                            Name = anime.Airing ? "Airing" : "Aired",
                            Value = (anime.EndDate == null ? "Since " : "") 
                                    + $"**{DateTime.Parse(anime.StartDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToLongDateString()}**"
                                    + (anime.EndDate != null
                                        ? $" to **{DateTime.Parse(anime.EndDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToLongDateString()}**"
                                        : "")
                        }
                    }
                };

                return new Page {Embeds = new List<LocalEmbed> {embed}};
            });

            return View(new AnimePagedView(new ListPageProvider(embeds)));
        }
    }
}