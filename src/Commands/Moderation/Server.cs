using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using EmbedBuilder = Discord.EmbedBuilder;
using sunshine.Classes;

namespace sunshine.Commands
{
    public class ServerInfo : CommandModuleBase
    {
        ServerInfo() { this.name = "server-info"; }

        private string[] Verification = { "None", "Low", "Medium", "High", "Extreme" };

        [Command("server")]
        [Alias("serverinfo")]
        public async Task server()
        {
            var _ = Context.Guild;
            long voice = 0, category = 0, text = 0;

            // listing channels
            var channels = _.Channels.GetEnumerator();
            do
            {
                var __ = channels.Current;
                if (__ is SocketCategoryChannel) category++;
                if (__ is SocketTextChannel) text++;
                if (__ is SocketVoiceChannel) voice++;
            }
            while (channels.MoveNext());

            // listing users
            long bot = 0, user = 0;
            await _.DownloadUsersAsync();
            var users = _.Users.GetEnumerator();
            while (users.MoveNext())
                if (users.Current.IsBot) bot++; else user++;

            await Context.Channel.SendMessageAsync(
                null, false,
                new EmbedBuilder() { }
                    .WithAuthor(_.Name, _.IconUrl)
                    .WithTitle("Server information")
                    .WithDescription($"ID : **`{_.Id}`**")
                    .AddField(
                        "Details",
                        $"Created : **{_.CreatedAt.UtcDateTime.ToString()}**\n"
                            + $"Owner : {_.Owner.Mention}\n"
                            + $"Voice region : **{_.VoiceRegionId.toSentencedCase()}**\n"
                            + $"Verification status : **{Verification[(int)_.VerificationLevel]}**"
                    )
                    .AddField(
                        "Statistics",
                        $"Users : **{bot + user}** (**{bot}** bot | **{user}** non-bot)\n"
                            + $"Roles : **{_.Roles.Count}**\n"
                            + $"Channels : **{category}** category | **{voice}** voice | **{text}** text"
                    )
                    .Build()
            );
        }

        [Command("icon")]
        [Alias("servericon", "server-icon")]
        public async Task icon() => await Context.Channel.SendMessageAsync(
            null, false,
            new EmbedBuilder() { }
                .WithAuthor(Context.Guild.Name, Context.Guild.IconUrl)
                .WithImageUrl(Context.Guild.IconUrl)
                .Build()
        );


    }
}
