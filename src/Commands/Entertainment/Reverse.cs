using System;
using System.Linq;
using Color = System.Drawing.Color;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using sunshine.Services;
using Pastel;

namespace sunshine.Commands
{
    public class Reverse : ModuleBase<SocketCommandContext>
    {
        public LogService logger { get; set; }

        private string moduleName = "reverse";
        [Command("reverse")]
        public async Task flip([Remainder] string _ = null) {
            var c = Context.Channel;
            if (_ == null || _.Length < 1)
                await c.SendMessageAsync(
                    null, false,
                    new EmbedBuilder(){}
                        .WithDescription($"{Context.Message.Author.Mention}, please give me something to reverse.")
                        .Build()
                );
            await c.SendMessageAsync(String.Join("", _.ToCharArray().Reverse().ToArray()));
        }

        protected override void OnModuleBuilding(CommandService serv, ModuleBuilder b)
        {
            logger.success($"Loaded module {moduleName.Pastel(Color.Yellow)}.");
        }
    }    
}
