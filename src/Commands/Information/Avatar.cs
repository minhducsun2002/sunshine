using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using sunshine.Classes;

namespace sunshine.Commands
{
    public class Avatar : CommandModuleBase
    {
        Avatar() { name = "avatar"; }

        [Command("avatar")]
        [Category("Moderation")]
        public async Task avatar(SocketUser user = null)
        {
            if (user == null) user = Context.User;
            await Context.Message.Channel.SendMessageAsync(
                "", false,
                new EmbedBuilder()
                {
                    Description = $"Profile picture of {user.Mention}",
                    ImageUrl = user.GetAvatarUrl(ImageFormat.Auto, 1024)
                }.Build()
            );
        }


    }
}
