using Disqord.Bot.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

dotenv.net.DotEnv.Load();

await new HostBuilder()
    .ConfigureAppConfiguration(x =>
    {
        x.AddEnvironmentVariables("DISCORD_");
    })
    .ConfigureLogging(x =>
    {
        x.AddSimpleConsole();
    })
    .ConfigureDiscordBot((context, bot) =>
    {
        bot.Token = context.Configuration["TOKEN"];
        bot.Prefixes = new[] { "a!" };
    })
    .RunConsoleAsync();