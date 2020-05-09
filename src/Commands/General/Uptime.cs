using System;
using System.Diagnostics;
using Discord;
using System.Threading.Tasks;
using Discord.Commands;
using sunshine.Classes;

namespace sunshine.Commands
{
    public class Uptime : CommandModuleBase
    {
        Uptime() { this.name = "uptime"; }

        [Command("uptime")]
        [Category("General")]
        public async Task uptime()
        {
            var _ = (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"d\:hh\:mm\:ss\.fff");
            await Context.Message.Channel.SendMessageAsync(
                null, false,
                new EmbedBuilder() { }
                    .WithDescription($":clock: {Context.Message.Author.Mention}, I've been running for **{_}**.")
                    .Build()
            );
        }


    }
}
