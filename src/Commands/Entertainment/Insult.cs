using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using sunshine.Services;
using sunshine.Classes;

namespace sunshine.Commands
{
    public class Insult : CommandModuleBase
    {
        Insult() { this.name = "insult"; }
        [Command("insult")]
        [Category("Entertainment")]
        public async Task speak(SocketUser _ = null)
        {
            if (_ == null) _ = Context.Message.Author;
            await ReplyAsync($"{await InsultService.insult()}, {_.Mention}.");
        }
    }
}
