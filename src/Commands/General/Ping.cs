using System.Drawing;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Commands.Builders;
using sunshine.Services;
using Pastel;

namespace sunshine.Commands
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        public LogService logger { get; set; }

        private string moduleName = "ping";

        [Command("ping")]
        public async Task ping() {
            await Context.Message.Channel.SendMessageAsync(
                $"Pong!\nRoundtrip latency : {Context.Client.Latency}ms."
            );
        }

        protected override void OnModuleBuilding(CommandService serv, ModuleBuilder b)
        {
            logger.success($"Loaded command {moduleName.Pastel(Color.Yellow)}.");
        }
    }    
}
