using System.Drawing;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Commands.Builders;
using sunshine.Services;
using Pastel;

namespace sunshine.Commands
{
    public class Tableflip : ModuleBase<SocketCommandContext>
    {
        public LogService logger { get; set; }

        private string moduleName = "tableflip";
        private string[] frames = {
            "(-°□°)-  ┬─┬",
            "(╯°□°)╯    ]",
            "(╯°□°)╯  ︵  ┻━┻",
            "(╯°□°)╯       [",
            "(╯°□°)╯           ┬─┬"
        };

        [Command("tableflip")]
        [Alias("tf")]
        public async Task flip() {
            var _ = await Context.Channel.SendMessageAsync(frames[0]);
            foreach (var frame in frames)
            {
                Task.Run(() => { while (true) {}; }).Wait(300);
                await _.ModifyAsync(x => x.Content = frame);
            }
        }

        protected override void OnModuleBuilding(CommandService serv, ModuleBuilder b)
        {
            logger.success($"Loaded module {moduleName.Pastel(Color.Yellow)}.");
        }
    }    
}
