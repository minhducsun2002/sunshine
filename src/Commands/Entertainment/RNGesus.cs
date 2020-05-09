using System;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Discord.Commands;
using DiscordColor = Discord.Color;
using sunshine.Classes;

namespace sunshine.Commands
{
    public class RNGesus : CommandModuleBase
    {
        private SHA512Managed sha = new SHA512Managed();
        RNGesus() { this.name = "rng"; }

        [Command("rate")]
        [Category("Entertainment")]
        public async Task ping([Remainder] string __ = null)
        {
            const uint scale = 10;

            var m = Context.Message;
            var err = new Discord.EmbedBuilder() { }.WithColor(DiscordColor.Red);
            if (__ == null || __.Length == 0)
            {
                await Context.Channel.SendMessageAsync(
                    null, false,
                    err.WithDescription($"{m.Author.Mention}, I don't want to rate the void. :rage:").Build()
                );
                return;
            };
            var _ = (BitConverter.ToUInt64(sha.ComputeHash(Encoding.UTF8.GetBytes(__))) % (scale + 1));
            string[] strings = {
                "a big fat", "quite a poor", "quite a poor",
                "an improvable", "an improvable", "a somewhat moderate",
                "a pretty moderate", "a prominent", "a high",
                "a high", "a solid"
            };
            await Context.Channel.SendMessageAsync(
                $"I'd give \"**{__}**\" {strings[_]} {_}/{scale}."
            );
        }


    }
}
