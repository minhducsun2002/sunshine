using Color = System.Drawing.Color;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using sunshine.Services;
using Pastel;

namespace sunshine.Commands
{
    public class Echo : ModuleBase<SocketCommandContext>
    {
        public LogService logger { get; set; }

        private string moduleName = "echo";
        [Command("say")]
        [Alias("echo", "repeat")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task flip([Remainder] string _ = null) {
            var c = Context.Channel;
            if (_ == null || _.Length < 1)
                await c.SendMessageAsync(
                    null, false,
                    new EmbedBuilder(){}
                        .WithDescription($"{Context.Message.Author.Mention}, please give me something to repeat.")
                        .Build()
                );
            try {
                await c.DeleteMessageAsync(Context.Message.Id);
            } catch {}
            await c.SendMessageAsync(_);
        }

        protected override void OnModuleBuilding(CommandService serv, ModuleBuilder b)
        {
            logger.success($"Loaded module {moduleName.Pastel(Color.Yellow)}.");
        }
    }    
}
