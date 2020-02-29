using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using dotenv.net;

namespace sunshine
{
    class Program
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        private DiscordSocketClient main = new DiscordSocketClient();

        public async Task MainAsync() {
            DotEnv.Config();
            string t = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

            main.Ready += onReady;

            await main.LoginAsync(TokenType.Bot, t);
            await main.StartAsync();
            await Task.Delay(-1);
        }

        private Task onReady() {
            this.Log($"Logged in as {main.CurrentUser.ToString()}, ready to serve {main.Guilds.Count} guilds.");
            return Task.CompletedTask;
        }

        private Task Log(string m) {
            Console.WriteLine(m);
            return Task.CompletedTask;
        }
    }
}
