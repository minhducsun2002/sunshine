using System;
using System.Threading.Tasks;
using Discord.Commands;
using sunshine.Classes;

namespace sunshine.Commands
{
    public class Tableflip : CommandModuleBase
    {
        Tableflip() { this.name = "tableflip"; }
        private string[] frames = {
            "(-°□°)-  ┬─┬",
            "(╯°□°)╯    ]",
            "(╯°□°)╯  ︵  ┻━┻",
            "(╯°□°)╯       [",
            "(╯°□°)╯           ┬─┬"
        };

        [Command("tableflip")]
        [Alias("tf")]
        [Category("Entertainment")]
        public async Task flip()
        {
            var _ = await Context.Channel.SendMessageAsync(frames[0]);
            foreach (var frame in new ArraySegment<string>(frames, 1, 4))
            {
                await Task.Delay(300);
                await _.ModifyAsync(x => x.Content = frame);
            }
        }
    }
}
