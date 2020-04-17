using System;
using Color = System.Drawing.Color;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using dotenv.net;
using sunshine.Services;
using Microsoft.Extensions.DependencyInjection;
using Pastel;

namespace sunshine
{
    class Program
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        private DiscordSocketClient _client = new DiscordSocketClient();

        public async Task MainAsync()
        {
            DotEnv.Config(false);

            var s = ConfigServices();
            var client = s.GetRequiredService<DiscordSocketClient>();
            
            // registering handler
            client.Ready += () => {
                s.GetRequiredService<LogService>()
                    .success($@"Successfully logged in as {
                        _client.CurrentUser.ToString().PastelBg(Color.DarkBlue).Pastel(Color.White)
                    }.");
                return Task.CompletedTask;
            };

            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_TOKEN"));
            await client.StartAsync();

            await s.GetRequiredService<CommandHandler>().PrepareCommandsAsync();
            await Task.Delay(-1);
        }

        private Task onReady()
        {
            this.Log($"Logged in as {_client.CurrentUser.ToString()}, ready to serve {_client.Guilds.Count} guilds.");
            return Task.CompletedTask;
        }

        private IServiceProvider ConfigServices() 
        {
            var commandService = new CommandService(new CommandServiceConfig(){
               IgnoreExtraArgs = true 
            });
            return new ServiceCollection()
                .AddSingleton<LogService>()
                .AddSingleton(_client)
                .AddSingleton(commandService)
                .AddSingleton<CommandHandler>()
                .AddSingleton<LogService>()
                // bruh
                .AddSingleton<MyAnimeList>()
                .BuildServiceProvider();
        }

        private Task Log(string m)
        {
            Console.WriteLine(m);
            return Task.CompletedTask;
        }
    }
}
