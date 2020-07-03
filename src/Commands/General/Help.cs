using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Linq;
using sunshine.Classes;

namespace sunshine.Commands
{
    public class Help : CommandModuleBase
    {
        public CommandService commands { get; set; }
        public string prefix = Environment.GetEnvironmentVariable("PREFIX");

        Help() { this.name = "help"; }

        public async Task SendCategory(Dictionary<string, CommandInfo> _, string categoryName)
        {
            await ReplyAsync(
                "", false,
                new EmbedBuilder()
                {
                    Description = $"Commands in the **{categoryName}** category :\n",
                    Timestamp = DateTime.UtcNow,
                    Fields = _.Select(
                        a => new EmbedFieldBuilder()
                            .WithName($"{prefix}{a.Value.Name}")
                            .WithValue($"{(a.Value.Summary == null ? "(No description.)" : a.Value.Summary)}")
                    ).ToList()
                }
                .Build()
            );
        }

        public async Task SendCommand(CommandInfo _)
        {
            var o = new EmbedBuilder()
                {
                    Title = $"`{prefix}{_.Name}`"
                        + (_.Aliases.Count > 0
                            ? $"({String.Join(", ", _.Aliases.Select(a => prefix + a))})"
                            : ""),
                    Description = _.Summary == null ? "(No description.)" : ""
                };

            if (_.Parameters.Count > 0)
                o.AddField(
                    "Arguments",
                    $"`{String.Join("", _.Parameters.Select(a => $"[{a.Type.Name}]"))}`"
                );
            await ReplyAsync("", false, o.Build());
        }

        [Command("help")]
        [Category("General")]
        [Summary("Where everything begins.")]
        public async Task help([Remainder] string query = "")
        {
            // categorizes
            var cat = new Dictionary<string, Dictionary<string, CommandInfo>>();
            var cmd = new Dictionary<string, CommandInfo>();

            commands.Commands.ToList().ForEach(a => {
                // skip nulls
                if (!(a.Attributes.FirstOrDefault(a => (a is CategoryAttribute)) is CategoryAttribute c)) return;

                if (!cat.ContainsKey(c.Category)) cat.Add(c.Category, new Dictionary<string, CommandInfo>());
                cat[c.Category].Add(a.Name, a);

                // individual commands
                a.Aliases.ToList().ForEach(alias => cmd.TryAdd(alias, a));
            });

            if (query.Length > 0) {
                bool pass = false;
                // search for categories
                var a = cat.Keys.FirstOrDefault(a => a.ToLowerInvariant() == query.ToLowerInvariant());
                if (a != null) { await SendCategory(cat[a], a); pass = true; }

                // search for commands
                a = cmd.Keys.FirstOrDefault(a => a.ToLowerInvariant() == query.ToLowerInvariant());
                if (a != null) { await SendCommand(cmd[a]); pass = true; }

                if (pass) return;

                await ReplyAsync(
                    "", false,
                    new EmbedBuilder()
                        .WithDescription("Sorry, couldn't find any matching category/command.")
                        .Build()
                );
            }

            var __ = Context.Client.CurrentUser;

            await ReplyAsync(
                "", false,
                new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder()
                        .WithName(__.ToString())
                        .WithIconUrl(__.GetAvatarUrl(ImageFormat.Png)),
                    Description = "Available command categories :\n" + $@"{
                        String.Join("\n", cat.Select(a => $"* **{a.Key}**"))
                    }",
                    Footer = new EmbedFooterBuilder()
                        .WithText(
                            $"Use \"{prefix}help <category>\" to show corresponding commands."
                        ),
                    Timestamp = DateTime.UtcNow
                }
                .Build()
            );
        }
    }
}