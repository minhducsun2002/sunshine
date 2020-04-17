using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace sunshine.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient client;
        private readonly CommandService commands;
        private readonly IServiceProvider services;

        public CommandHandler (IServiceProvider serv) {
            this.client = serv.GetRequiredService<DiscordSocketClient>();
            this.commands = serv.GetRequiredService<CommandService>();
            this.services = serv;
        }

        public async Task PrepareCommandsAsync()
        {
            // _client
            await commands.AddModulesAsync(
                Assembly.GetEntryAssembly(),
                services: this.services
            );
            client.MessageReceived += HandleCommandAsync;
        }

        public async Task HandleCommandAsync(SocketMessage msg)
        {
            // check if system msg
            if (!(msg is SocketUserMessage m)) return;
            if (msg.Author.Id == this.client.CurrentUser.Id) return;

            string prefix = Environment.GetEnvironmentVariable("PREFIX");
            
            int argPos = 0;
            if (!m.HasStringPrefix(prefix, ref argPos, StringComparison.OrdinalIgnoreCase)) return;

            var context = new SocketCommandContext(this.client, m);
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (result.Error != null) {
                // who tf cares?
                if (result.Error.Value == CommandError.UnknownCommand) return;
                // if (result.Error.Value == CommandError.UnmetPrecondidtion) return;
                // if (result.Error.Value == CommandError.BadArgCount) return;
                // if (result.Error.Value == CommandError.ParseFailed) return;

                // get logger
                services.GetRequiredService<LogService>().error(result.ErrorReason);
                await m.Channel.SendMessageAsync(
                    $"Apologize, {m.Author.Mention}, executing your command failed due to the following reason :",
                    false,
                    new Discord.EmbedBuilder(){}
                        .WithDescription($"```{result.ErrorReason}```")
                        .Build()
                );
            }
        }
    }
}