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

        private string pickGenerator(string[] _, Int32 seed = 1)
        {
            var random = new Random(BitConverter.ToInt32(
                sha.ComputeHash(Encoding.UTF8.GetBytes(
                    String.Join("", _)
                ))
            ) * seed);
            return _[((UInt32) random.Next()) % _.Length];
        }

        [Command("pick")]
        [Category("Entertainment")]
        public async Task pick([Remainder] string _ = "")
        {
            var m = Context.Message;
            if (_.Length == 0) {
                await ReplyAsync($"{m.Author.Mention}, I choose you.");
                return;
            }

            // split
            var __ = _.Split("\n");
            await ReplyAsync($"{m.Author.Mention}, I would choose **{pickGenerator(__, (Int32) DateTime.Now.ToBinary())}**");

        }

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
