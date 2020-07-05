using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using sunshine.Services;
using sunshine.Classes;

namespace sunshine.Commands
{
    namespace MAL
    {
        public class MAL : CommandModuleBase
        {
            MAL() { name = "MAL"; }

            public MyAnimeList mal { get; set; }

            [Command("anime")]
            [Category("Weebs")]
            public async Task anime([Remainder] string query = "")
            {
                var m = Context.Message;
                var err = new EmbedBuilder() { }.WithColor(Color.Red);
                if (query == null || query.Length == 0)
                {
                    await ReplyAsync(
                        null, false,
                        err.WithDescription($"{m.Author.Mention}, I see nothing to search about. :frowning:").Build()
                    );
                    return;
                };
                var result = await mal.anime(query);
                if (result.Count < 1) {
                    await ReplyAsync($"Apologies, {m.Author.Mention}, couldn't find anything that matched.");
                    return;
                };
                var id = result[0].mal_id;
                var _ = await mal.anime(id);
                await ReplyAsync(
                    null, false,
                    new EmbedBuilder()
                    {
                        Title = _.title,
                        Url = _.url,
                        ImageUrl = _.image_url,
                        Description = (
                            _.synopsis.Length > 2000 ? _.synopsis.Substring(0, 2000) + "..." : _.synopsis
                        )
                    } 
                        .AddField(
                            "Titles",
                            $"**Japanese** : {_.title_japanese}"
                            + (_.title_english.Length > 0 ? $"\n**English** : {_.title_english}" : "")
                            + (_.title_synonyms.Length > 0 ? $"\n**Alternatives** : {String.Join(", ", _.title_synonyms)}" : "")
                        )
                        .AddField(
                            "General information",
                            $"**Type** : {_.type}"
                            + $"\n**Source** : {_.source}"
                            + (_.episodes == null ? "" : $"\n**Episode** : {_.episodes}")
                            + $"\n**Premiere** : {_.premiered}"
                            + $"\n**Duration** : {_.duration}"
                            + $"\n**Rating** : {_.rating}"
                            + $"\n{(_.airing ? "**Currently airing**" : $"**Aired** : {_.aired.@string}")}"
                            + $"\n**Genre** : {String.Join(", ", _.genres.Select(g => g.name).ToArray())}"
                            + $"\n**Studio** : {String.Join(", ", _.studios.Select(g => g.name).ToArray())}"
                        )
                        .Build()
                );

            }

            [Command("manga")]
            [Category("Weebs")]            
            public async Task manga([Remainder] string query = "")
            {
                var m = Context.Message;
                var err = new EmbedBuilder() { }.WithColor(Color.Red);
                if (query == null || query.Length == 0)
                {
                    await ReplyAsync(
                        null, false,
                        err.WithDescription($"{m.Author.Mention}, I see nothing to search about. :frowning:").Build()
                    );
                    return;
                };
                var result = await mal.manga(query);
                if (result.Count < 1) {
                    await ReplyAsync($"Apologies, {m.Author.Mention}, couldn't find anything that matched.");
                    return;
                };
                var id = result[0].mal_id;
                var _ = await mal.manga(id);
                await ReplyAsync(
                    null, false,
                    new EmbedBuilder()
                    {
                        Title = _.title,
                        Url = _.url,
                        ImageUrl = _.image_url,
                        Description = (
                            _.synopsis.Length > 2000 ? _.synopsis.Substring(0, 2000) + "..." : _.synopsis
                        )
                    } 
                        .AddField(
                            "Titles",
                            $"**Japanese** : {_.title_japanese}"
                            + (_.title_english.Length > 0 ? $"\n**English** : {_.title_english}" : "")
                            + (_.title_synonyms.Length > 0 ? $"\n**Alternatives** : {string.Join(", ", _.title_synonyms)}" : "")
                        )
                        .AddField(
                            "General information",
                            $"**Type** : {_.type}"
                            + (_.publishing
                                ? "\nCurrently publishing"
                                : $"**\nPublished** : {_.published.@string}")
                            + (_.volumes == null ? "" : $"\n**Volumes** : {_.volumes}")
                            + (_.chapters == null ? "" : $"\n**Chapters** : {_.chapters}")
                            + (_.score == null ? "" : $"\n**Score** : {_.score}")
                            + $"\n**Genre** : {string.Join(", ", _.genres.Select(g => $"[{g.name}]({g.url})").ToArray())}"
                            + $"\n**Author** : {string.Join(", ", _.authors.Select(g => $"[{g.name}]({g.url})").ToArray())}"
                        )
                        .Build()
                );

            }
        }
    }
}
