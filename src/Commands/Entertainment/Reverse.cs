using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using sunshine.Classes;

namespace sunshine.Commands
{
    public class Reverse : CommandModuleBase
    {
        Reverse() { this.name = "reverse"; }
        [Command("reverse")]
        [Category("Entertainment")]
        public async Task flip([Remainder] string _ = null)
        {
            var c = Context.Channel;
            if (_ == null || _.Length < 1)
                await c.SendMessageAsync(
                    null, false,
                    new EmbedBuilder() { }
                        .WithDescription($"{Context.Message.Author.Mention}, please give me something to reverse.")
                        .Build()
                );
            await c.SendMessageAsync(String.Join("", _.ToCharArray().Reverse().ToArray()));
        }
    }
}
