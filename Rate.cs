using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Disqord.Bot;
using Qmmands;

namespace sunshine
{
    public class Rate : DiscordModuleBase
    {
        private readonly SHA512Managed sha = new SHA512Managed();
        
        [Command("rate")]
        public async Task<DiscordCommandResult> Exec([Remainder] string query = "")
        {
            if (string.IsNullOrWhiteSpace(query)) return Reply("I rate the void 10 out of 10.");
            string[] strings = {
                "a big fat", "quite a poor", "quite a poor",
                "an improvable", "an improvable", "a somewhat moderate",
                "a pretty moderate", "a prominent", "a high",
                "a high", "a solid"
            };
            var scale = (uint) strings.Length;
            var rating = BitConverter.ToUInt64(sha.ComputeHash(Encoding.UTF8.GetBytes(query))) % scale;
            return Reply($"I would give \"**{query}**\" {strings[rating]} {rating}/{scale - 1}.");
        }
    }
}
