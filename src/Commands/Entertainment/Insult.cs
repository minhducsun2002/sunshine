using Color = System.Drawing.Color;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Commands.Builders;
using sunshine.Services;
using Pastel;

namespace sunshine.Commands
{
    public class Insult : ModuleBase<SocketCommandContext>
    {
        public LogService logger { get; set; }

        private string moduleName = "Insult";
        [Command("insult")]
        public async Task speak(SocketUser _ = null) {
            if (_ == null) _ = Context.Message.Author;
            await ReplyAsync($"{await InsultService.insult()}, {_.Mention}.");
        }

        protected override void OnModuleBuilding(CommandService serv, ModuleBuilder b)
        {
            logger.success($"Loaded module {moduleName.Pastel(Color.Yellow)}.");
        }
    }    
}
