using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using sunshine.Classes;

namespace sunshine.Commands
{
    public class CustomUserReader<T> : UserTypeReader<T>
        where T : class, IUser
    {
        public CustomUserReader() : base() {}
        public override Task<TypeReaderResult> ReadAsync(ICommandContext c, string i, IServiceProvider s)
        {
            var _ = MentionUtils.TryParseUser(i, out var userId);
            c.Client.GetUserAsync(userId, CacheMode.AllowDownload).Result.GetOrCreateDMChannelAsync();
            return base.ReadAsync(c, i, s);
        }
    }

    public class Avatar : CommandModuleBase
    {
        Avatar() { name = "avatar"; }

        [Command("avatar")]
        [Category("Moderation")]
        public async Task avatar(
            [OverrideTypeReader(typeof (CustomUserReader<IUser>))] IUser user = null
        )
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
