#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Disqord;
using Disqord.Bot;
using Disqord.Extensions.Interactivity.Menus;
using Disqord.Extensions.Interactivity.Menus.Paged;
using Disqord.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qmmands;

namespace sunshine
{
    internal class AnimePagedView : PagedView
    {
        private static readonly HttpClient Client = new();
        private readonly long[] animeIds;
        public AnimePagedView(PageProvider pageProvider, long[] animeIds) : base(pageProvider, new LocalMessage())
        {
            this.animeIds = animeIds;
            
            FirstPageButton.Label = "First result"; FirstPageButton.Emoji = null;
            PreviousPageButton.Label = "Previous"; PreviousPageButton.Emoji = null;
            NextPageButton.Label = "Next"; NextPageButton.Emoji = null;
            if (NextPageButton.Position != null) NextPageButton.Position++;
            if (LastPageButton.Position != null) LastPageButton.Position++;
            LastPageButton.Label = "Last result"; LastPageButton.Emoji = null;
            RemoveComponent(StopButton);
        }
        
        [Button(Label = "Detailed", Style = LocalButtonComponentStyle.Primary)]
        public async ValueTask Confirm(ButtonEventArgs e)
        {
            var anime = JsonConvert.DeserializeObject<AnimeDetailed>(
                await Client.GetStringAsync($"{MyAnimeList.BaseURL}/anime/{animeIds[CurrentPageIndex]}")
            )!;
            var t = (string.IsNullOrWhiteSpace(anime.TitleJapanese) ? "" : $"**Japanese** : {anime.TitleJapanese}")
                    + (string.IsNullOrWhiteSpace(anime.TitleEnglish) ? "" : $"\n**English** : {anime.TitleEnglish}")
                    + (anime.TitleSynonyms == null || anime.TitleSynonyms.Length < 1
                        ? ""
                        : $"\n**Alternatives** : {string.Join(", ", anime.TitleSynonyms)}");

            var embed = new LocalEmbed
            {
                Title = anime.Title,
                Url = anime.Url,
                ImageUrl = anime.ImageUrl,
                Description = anime.Synopsis.Length > 2000 ? anime.Synopsis[..2000] + "..." : anime.Synopsis,
                Fields = new List<LocalEmbedField>
                {
                    new()
                    {
                        Name = "General information",
                        Value = $"**Type** : {anime.Type}"
                                + $"\n**Source** : {anime.Source}"
                                + (anime.Episodes == null ? "" : $"\n**Episode** : {anime.Episodes}")
                                + (anime.Premiered == null ? "" : $"\n**Premiere** : {anime.Premiered}")
                                + $"\n**Duration** : {anime.Duration}"
                                + $"\n**Rating** : {anime.Rating}"
                                + "\n"
                                + $@"{(anime.Airing
                                    ? $"**Currently airing** {MyAnimeList.GetDate(anime.Aired.From, anime.Aired.To, true)}"
                                    : $"**Aired** : {MyAnimeList.GetDate(anime.Aired.From, anime.Aired.To, true, false)}"
                                    )}"
                                + $"\n**Genre** : {string.Join(", ", anime.Genres.Select(g => g.Name).ToArray())}"
                                + $"\n**Studio** : {string.Join(", ", anime.Studios.Select(g => g.Name).ToArray())}"
                    }
                }
            };

            if (!string.IsNullOrWhiteSpace(t)) embed.Fields.Insert(0, new LocalEmbedField { Name = "Title", Value = t });
            await e.Interaction.Response().ModifyMessageAsync(new LocalInteractionResponse
            {
                Embeds = new[] {embed}
            });
            Menu.Stop();
        }
    }
    
    public class MyAnimeList : DiscordModuleBase
    {
        private static readonly HttpClient Client = new();
        internal const string BaseURL = "https://api.jikan.moe/v3";
        
        internal static string GetDate(string startDate, string? endDate, bool toBeInterpolated = false, bool bold = true) =>
            (endDate == null ? $"{(toBeInterpolated ? "s" : "S")}ince " : "")
            + $@"{(bold ? "**" : "")}{
                DateTime.Parse(startDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToLongDateString()
            }{(bold ? "**" : "")}"
            + (endDate != null
                ? $@" to {(bold ? "**" : "")}{
                    DateTime.Parse(endDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToLongDateString()
                }{(bold ? "**" : "")}"
                : "");

        private async Task<string> BaseSearch(string obj, string q)
            => await Client.GetStringAsync($"{BaseURL}/search/{obj}?q={HttpUtility.UrlEncode(q)}");

        [Command("anime")]
        [Description("Search for an anime")]
        public async Task<DiscordCommandResult> Anime([Remainder] string query = "")
        {
            if (string.IsNullOrWhiteSpace(query)) return Reply("I saw nothing to search about!");
            var response = await BaseSearch("anime", query);
            var animes = JObject.Parse(response)["results"].ToObject<Anime[]>();

            if (animes.Length == 0)
                return Reply("I found no results. Are you sure you aren't searching for illegal stuff?");

            var animeIds = animes.Select(anime => anime.MyAnimeListId).ToArray();
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
                            Value = GetDate(anime.StartDate, anime.EndDate)
                        }
                    }
                };

                return new Page {Embeds = new List<LocalEmbed> {embed}};
            });

            return View(new AnimePagedView(new ListPageProvider(embeds), animeIds));
        }
    }
}