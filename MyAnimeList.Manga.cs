using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
    internal class MangaPagedView : PagedView
    {
        private static readonly HttpClient Client = new();
        private readonly long[] mangaIds;
        public MangaPagedView(PageProvider pageProvider, long[] mangaIds) : base(pageProvider, new LocalMessage())
        {
            this.mangaIds = mangaIds;
            
            FirstPageButton.Label = "First result"; FirstPageButton.Emoji = null;
            PreviousPageButton.Label = "Previous"; PreviousPageButton.Emoji = null;
            NextPageButton.Label = "Next"; NextPageButton.Emoji = null;
            LastPageButton.Label = "Last result"; LastPageButton.Emoji = null;
            RemoveComponent(StopButton);
        }
        
        [Button(Label = "Detailed", Style = LocalButtonComponentStyle.Primary)]
        public async ValueTask Confirm(ButtonEventArgs e)
        {
            var manga = JsonConvert.DeserializeObject<MangaDetailed>(
                await Client.GetStringAsync($"{MyAnimeList.BaseURL}/manga/{mangaIds[CurrentPageIndex]}")
            )!;
            var t = (string.IsNullOrWhiteSpace(manga.TitleJapanese) ? "" : $"**Japanese** : {manga.TitleJapanese}")
                    + (string.IsNullOrWhiteSpace(manga.TitleEnglish) ? "" : $"\n**English** : {manga.TitleEnglish}")
                    + (manga.TitleSynonyms == null || manga.TitleSynonyms.Length < 1
                        ? ""
                        : $"\n**Alternatives** : {string.Join(", ", manga.TitleSynonyms)}");

            var embed = new LocalEmbed
            {
                Title = manga.Title,
                Url = manga.Url,
                ImageUrl = manga.ImageUrl,
                Description = manga.Synopsis.Length > 2000 ? manga.Synopsis[..2000] + "..." : manga.Synopsis,
                Fields = new List<LocalEmbedField>
                {
                    new()
                    {
                        Name = "General information",
                        Value = $"**Type** : {manga.Type}"
                                + (manga.Chapters == null ? "" : $"\n**Chapters** : {manga.Chapters}")
                                + $"\n**Volumes** : {manga.Volumes}"
                                + $"\n**Score** : {manga.Score}"
                                + "\n"
                                + $@"{(manga.Publishing
                                    ? $"**Currently published** {MyAnimeList.GetDate(manga.Published.From, manga.Published.To, true)}"
                                    : $"**Published** : {MyAnimeList.GetDate(manga.Published.From, manga.Published.To, true, false)}"
                                    )}"
                                + $"\n**Genre** : {string.Join(", ", manga.Genres.Select(g => g.Name).ToArray())}"
                                + $"\n**Author** : {string.Join(", ", manga.Authors.Select(g => $"[{g.Name}]({g.Url})").ToArray())}"
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
    
    public partial class MyAnimeList
    {
        [Command("manga")]
        [Description("Search for a manga")]
        public async Task<DiscordCommandResult> Manga([Remainder] string query = "")
        {
            if (string.IsNullOrWhiteSpace(query)) return Reply("I saw nothing to search about!");
            var response = await BaseSearch("manga", query);
            var mangas = JObject.Parse(response)["results"]!.ToObject<Manga[]>()!;
            
            if (mangas.Length == 0)
                return Reply("I found no results. Are you sure you aren't searching for illegal stuff?");
            
            var mangaIds = mangas.Select(manga => manga.MyAnimeListId);

            var embeds = mangas.Select(manga =>
            {
                var embed = new LocalEmbed
                {
                    Title = manga.Title,
                    Url = manga.Url,
                    ImageUrl = manga.ImageUrl,
                    Description = manga.Synopsis,
                    Fields = new List<LocalEmbedField>
                    {
                        new()
                        {
                            Name = "Statistics",
                            Value =
                                $"**Type** : {manga.Type}\n**Chapters** : {manga.Chapters}\n**Volumes** : {manga.Volumes}\n**Score** : {manga.Score}"
                        },
                        new()
                        {
                            Name = manga.Publishing ? "Publishing" : "Published",
                            Value = GetDate(manga.StartDate, manga.EndDate)
                        }
                    }
                };

                return new Page {Embeds = new List<LocalEmbed> {embed}};
            });

            return View(new MangaPagedView(new ListPageProvider(embeds), mangaIds.ToArray()));
        }
    }
}