using System.Drawing;
using Discord.Commands;
using Discord.Commands.Builders;
using sunshine.Services;
using Pastel;

namespace sunshine.Classes
{
    public abstract class CommandModuleBase : ModuleBase<SocketCommandContext>
    {
        public LogService logger { get; set; }
        // public ICommandContext Context;
        protected string name;
        protected override void OnModuleBuilding(CommandService serv, ModuleBuilder b)
        {
            logger.success($"Loaded module {name.Pastel(Color.Yellow)}.");
        }
    }
}