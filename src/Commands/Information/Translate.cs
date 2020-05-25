using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using sunshine.Classes;
using sunshine.Services;

namespace sunshine.Commands
{
    public class Translate : CommandModuleBase
    {
        public TranslateService translate { get; set; }

        Translate() { this.name = "echo"; }

        [Command("translate")]
        [Summary("Let's speak in another language.")]
        [Category("Information")]
        public async Task exec(
            [Name("Source language")] string src = "auto",
            [Name("Target language")] string dst = "en",
            [Remainder] [Name("Text to translate")] string _ = ""
        )
        {
            var lang = translate.getLanguages();
            if (!lang.sl.ContainsKey(src.ToLowerInvariant())) {
                await ReplyAsync("Sorry, your source language code is incorrect.");
                return;
            }

            if (!lang.tl.ContainsKey(dst.ToLowerInvariant())) {
                await ReplyAsync("Sorry, your target language code is incorrect.");
                return;
            }

            var response = translate.translate(src, dst, _);
            await ReplyAsync(
                response.Item1, false, 
                new EmbedBuilder(){
                    Description = $"Source : **{lang.sl[src]}**\nTarget : **{lang.tl[dst]}**"
                }.Build()
            );
        }
    }
}
