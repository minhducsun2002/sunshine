using System;
using System.Diagnostics;
using EmbedBuilder = Discord.EmbedBuilder;
using System.Drawing;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Commands.Builders;
using sunshine.Services;
using Pastel;

namespace sunshine.Commands
{
    public class Uptime : ModuleBase<SocketCommandContext>
    {
        public LogService logger { get; set; }

        private string moduleName = "uptime";

        [Command("uptime")]
        public async Task uptime() {
            var _ = (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"d\:hh\:mm\:ss\.fff");
            await Context.Message.Channel.SendMessageAsync(
                null, false,
                new EmbedBuilder(){}
                    .WithDescription($":clock: {Context.Message.Author.Mention}, I've been running for **{_}**.")
                    .Build()
            );
        }

        protected override void OnModuleBuilding(CommandService serv, ModuleBuilder b)
        {
            logger.success($"Loaded module {moduleName.Pastel(Color.Yellow)}.");
        }
    }    
}
