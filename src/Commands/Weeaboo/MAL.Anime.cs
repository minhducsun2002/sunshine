using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using sunshine.Services;
using Pastel;
using DiscordColor = Discord.Color;
using Color = System.Drawing.Color;

namespace sunshine.Commands
{
    namespace MAL
    {
        public class Anime : ModuleBase<SocketCommandContext>
        {
            public LogService logger { get; set; }
            public MyAnimeList MAL { get; set; }

            private string moduleName = "MAL.anime";

            [Command("anime")]
            public async Task anime([Remainder] string query = "") {
                var m = Context.Message;
                var err = new EmbedBuilder(){}.WithColor(DiscordColor.Red);
                if (query == null || query.Length == 0) {
                    await Context.Channel.SendMessageAsync(
                        null, false,
                        err.WithDescription($"{m.Author.Mention}, I see nothing to search about. :frowning:").Build()
                    );
                    return;
                };
                try {
                    var id = (await MAL.anime(query))[0].mal_id;
                    var _ = await MAL.anime(id);
                    await Context.Channel.SendMessageAsync(
                        null, false,
                        new EmbedBuilder(){}
                            .WithTitle(_.title)
                            .WithUrl(_.url)
                            .WithImageUrl(_.image_url)
                            .WithDescription(_.synopsis)
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
                                + $"\n**Episodes** : {_.episodes}"
                                + $"\n**Premiere** : {_.premiered}"
                                + $"\n**Duration** : {_.duration}"
                                + $"\n**Rating** : {_.rating}"
                                + $"\n{(_.airing ? "**Currently airing**" : $"**Aired** : {_.aired.@string}")}"
                                + $"\n**Genres** : {String.Join(", ", _.genres.Select(g => g.name).ToArray())}"
                            )
                            .Build()
                    );
                } catch (Exception e) {
                    await Context.Channel.SendMessageAsync(
                        null, false,
                        err
                            .WithDescription(
                                $"Apologize, {m.Author.Mention}, but an error occurred :frowning:\n"
                                + $"```{e.ToString()}```"
                            )
                            .Build()
                    );
                    return;
                }
            }

            protected override void OnModuleBuilding(CommandService serv, ModuleBuilder b)
            {
                logger.success($"Loaded module {moduleName.Pastel(Color.Yellow)}.");
            }
        }    
    }
}
