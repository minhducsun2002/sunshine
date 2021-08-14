using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Bot.Hosting;
using Disqord.Gateway;
using Humanizer;
using Qmmands;

namespace sunshine
{
    public class Chunk : DiscordBotService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Bot.WaitUntilReadyAsync(stoppingToken);
            foreach (var guild in Bot.GetGuilds().Values)
                if (guild.MemberCount <= 100)
                    await Bot.Chunker.ChunkAsync(guild, stoppingToken);
            
            await base.ExecuteAsync(stoppingToken);
        }
    }
    
    public class ServerInfo : DiscordGuildModuleBase
    {
        [Command("serverinfo", "server")]
        [Description("View information of this server")]
        public async Task<DiscordCommandResult> Exec()
        {
            var guild = Context.Guild;
            var createdAt = guild.CreatedAt();
            var members = guild.GetMembers().Values.ToList();
            var channels = guild.GetChannels().Values.ToList();
            
            var embed = new LocalEmbed
            {
                Title = guild.Name,
                Description = $"**Server ID** : {guild.Id}",
                Fields = new List<LocalEmbedField>
                {
                    new()
                    {
                        Name = "Details",
                        Value = string.Join("\n",
                            $"• Created : **{createdAt.ToUniversalTime().ToString($"HH:mm:ss, dd/MM/yyyy UTC", CultureInfo.InvariantCulture)}**"
                            + $" ({createdAt.Humanize()})",
                            $"• Owner : <@{guild.OwnerId}>",
                            $"• Voice region : **`{guild.VoiceRegion}`**",
                            $"• Verification : **{guild.VerificationLevel.Humanize().Transform(To.SentenceCase)}**"
                        )
                    },
                    new()
                    {
                        Name = "Users",
                        Value = string.Join("\n",
                            $"• Users : **{members.Count(member => !member.IsBot)}**",
                            $"• Bots : **{members.Count(member => member.IsBot)}**"
                        ),
                        IsInline = true
                    },
                    new()
                    {
                        Name = "Roles",
                        Value = string.Join("\n",
                            $"• Count : **{guild.Roles.Count}**",
                            $"• Highest : {guild.Roles.Values.OrderByDescending(role => role.Position).ThenBy(role => role.Id).First().Mention}"
                        ),
                        IsInline = true
                    },
                    new()
                    {
                        Name = "Channels",
                        Value = string.Join("\n",
                            $"• Text : **{channels.Count(channel => channel is CachedTextChannel)}**",
                            $"• Voice : **{channels.Count(channel => channel is CachedVoiceChannel)}**",
                            $"• Category : **{channels.Count(channel => channel is CachedCategoryChannel)}**"
                                + (guild.SystemChannelId != null ? $"\n• Default : <#{guild.SystemChannelId}>" : "")
                        ),
                        IsInline = true
                    }
                },
                Color = Color.Cyan,
                ThumbnailUrl = guild.GetIconUrl(CdnAssetFormat.Png, 1024)
            };

            return Reply(embed);
        }
    }
}