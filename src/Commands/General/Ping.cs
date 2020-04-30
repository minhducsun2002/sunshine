using System.Threading.Tasks;
using Discord.Commands;
using sunshine.Classes;

namespace sunshine.Commands
{
    public class Ping : CommandModuleBase
    {
        Ping() { this.name = "ping"; }

        [Command("ping")]
        public async Task ping()
        {
            await Context.Message.Channel.SendMessageAsync(
                $"Pong!\nRoundtrip latency : {Context.Client.Latency}ms."
            );
        }


    }
}
