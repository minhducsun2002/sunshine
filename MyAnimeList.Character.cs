#nullable enable
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
    internal class CharacterPagedView : PagedView
    {
        private static readonly HttpClient Client = new();
        private readonly long[] characterIds;
        public CharacterPagedView(PageProvider pageProvider, long[] characterIds) : base(pageProvider, new LocalMessage())
        {
            this.characterIds = characterIds;
            
            FirstPageButton.Label = "First result"; FirstPageButton.Emoji = null;
            PreviousPageButton.Label = "Previous"; PreviousPageButton.Emoji = null;
            NextPageButton.Label = "Next"; NextPageButton.Emoji = null;
            LastPageButton.Label = "Last result"; LastPageButton.Emoji = null;
            RemoveComponent(StopButton);
        }
        
        [Button(Label = "Detailed", Style = LocalButtonComponentStyle.Primary)]
        public async ValueTask Confirm(ButtonEventArgs e)
        {
            var character = JsonConvert.DeserializeObject<CharacterDetailed>(
                await Client.GetStringAsync($"{MyAnimeList.BaseURL}/character/{characterIds[CurrentPageIndex]}")
            )!;

            character.About = character.About.Replace("\n\n", "\n");
            
            var embed = new LocalEmbed
            {
                Title = character.Name + (string.IsNullOrWhiteSpace(character.NameKanji) ? "" : $" ({character.NameKanji})"),
                Url = character.Url,
                ImageUrl = character.ImageUrl,
                Description = character.About.Length > 2000 ? character.About[..2000] + "..." : character.About,
                Fields = new List<LocalEmbedField>()
            };
            
            if (character.Nicknames.Length != 0)
                embed.Fields.Add(new LocalEmbedField
                {
                    Name = "Nicknames",
                    Value = string.Join(", ", character.Nicknames)
                });
            
            if (character.Mangaography.Length != 0)
                embed.Fields.Add(new LocalEmbedField
                {
                    Name = "Mangaography",
                    Value = string.Join(", ", character.Mangaography.Select(manga => $"[{manga.Name}]({manga.Url}) ({manga.Role})"))
                });
            
            if (character.Animeography.Length != 0)
                embed.Fields.Add(new LocalEmbedField
                {
                    Name = "Animeography",
                    Value = string.Join(", ", character.Animeography.Select(anime => $"[{anime.Name}]({anime.Url}) ({anime.Role})"))
                });
            
            if (character.VoiceActors.Length != 0)
                embed.Fields.Add(new LocalEmbedField
                {
                    Name = "Voice actors",
                    Value = string.Join(", ", character.VoiceActors.Select(va => $"[{va.Name}]({va.Url}) ({va.Language})"))
                });
            
            await e.Interaction.Response().ModifyMessageAsync(new LocalInteractionResponse
            {
                Embeds = new[] {embed}
            });
            Menu.Stop();
        }
    }
    
    public partial class MyAnimeList
    {
        [Command("character")]
        [Description("Search for a character")]
        public async Task<DiscordCommandResult> Character([Remainder] string query = "")
        {
            if (string.IsNullOrWhiteSpace(query)) return Reply("I saw nothing to search about!");

            var response = await BaseSearch("character", query);
            var characters = JObject.Parse(response)["results"]!.ToObject<CharacterSearch[]>()!;
            
            if (characters.Length == 0) return Reply("I found no results.");
            var characterIds = characters.Select(character => character.MyAnimeListId).ToArray();
            var embeds = characters.Select(character => new Page
            {
                Embeds = new List<LocalEmbed>
                {
                    new()
                    {
                        Title = character.Name,
                        Url = character.Url,
                        ImageUrl = character.ImageUrl,
                        Fields = new List<LocalEmbedField?>
                            {
                                (character.Anime.Length == 0
                                    ? null
                                    : new LocalEmbedField
                                    {
                                        Name = "Anime appearances",
                                        Value = string.Join(", ",
                                            character.Anime.Select(anime => $"[{anime.Name}]({anime.Url})")),
                                        IsInline = false
                                    }),
                                (character.Manga.Length == 0
                                    ? null
                                    : new LocalEmbedField
                                    {
                                        Name = "Manga appearance",
                                        Value = string.Join(", ",
                                            character.Manga.Select(anime => $"[{anime.Name}]({anime.Url})")),
                                        IsInline = false
                                    }),
                            }
                            .Where(field => field != null)
                            .ToList()
                    }
                }
            });

            return View(new CharacterPagedView(new ListPageProvider(embeds), characterIds));
        }
    }
}