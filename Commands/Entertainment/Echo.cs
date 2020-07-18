using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using sunshine.Classes;

namespace sunshine.Commands
{
    public class Echo : CommandModuleBase
    {
        Echo() { this.name = "echo"; }

        [Command("say")]
        [Alias("echo", "repeat")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [Summary("Too shy to say something? I can help you with that.")]
        [Category("Entertainment")]
        public async Task flip([Remainder] [Name("Text to say")] string _ = null)
        {
            var c = Context.Channel;
            if (_ == null || _.Length < 1)
                await c.SendMessageAsync(
                    null, false,
                    new EmbedBuilder() { }
                        .WithDescription($"{Context.Message.Author.Mention}, please give me something to repeat.")
                        .Build()
                );
            try
            {
                await c.DeleteMessageAsync(Context.Message.Id);
            }
            catch { }
            await c.SendMessageAsync(_);
        }
    }
}
