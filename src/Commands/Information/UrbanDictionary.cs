using System;
using System.Collections.Generic;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using DiscordColor = Discord.Color;
using sunshine.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace sunshine.Commands
{
    public class UrbanResponse
    {
        public string definition { get; set; }
        public string example { get; set; }
        public string permalink { get; set; }
    }

    public class Urban : CommandModuleBase
    {
        Urban() { this.name = "urban"; }
        private int MAX_LEN = 1000;
        private readonly HttpClient httpClient = new HttpClient();

        [Command("urban")]
        [Category("Information")]
        public async Task urban([Remainder] string query = null)
        {
            var m = Context.Message;
            var err = new EmbedBuilder() { }.WithColor(DiscordColor.Red);
            if (query == null || query.Length == 0)
            {
                await Context.Channel.SendMessageAsync(
                    null, false,
                    err.WithDescription($"{m.Author.Mention}, I see nothing to search about. :frowning:").Build()
                );
                return;
            };

            try
            {
                var response = await httpClient.GetStringAsync(
                    $"http://api.urbandictionary.com/v0/define?term=${HttpUtility.UrlEncode(query)}"
                );
                var _ = ((JObject)JsonConvert.DeserializeObject(response))["list"]
                    .ToObject<List<UrbanResponse>>();
                string def = _[0].definition, eg = _[0].example;
                await Context.Channel.SendMessageAsync(
                    null, false,
                    new EmbedBuilder() { }
                        .WithAuthor("Urban Dictionary", "https://vgy.me/ScvJzi.jpg")
                        .WithTitle($"Urban Dictionary definition for **{query}**")
                        .WithUrl(_[0].permalink)
                        .AddField(
                            "Definition",
                            def.Substring(0, Math.Min(MAX_LEN, def.Length))
                            + (def.Length > 1000 ? "..." : "")
                        )
                        .AddField(
                            "Example",
                            eg.Substring(0, Math.Min(MAX_LEN, eg.Length))
                            + (eg.Length > 1000 ? "..." : "")
                        )
                        .WithFooter($"Definition 1 of {_.Capacity}")
                        .WithTimestamp(DateTime.Now)
                        .WithColor(0, 255, 255)
                        .Build()
                );
            }
            catch (HttpRequestException e)
            {
                await Context.Channel.SendMessageAsync(
                    null, false,
                    err
                        .WithDescription(
                            $"Apologize, {m.Author.Mention}, but an error occurred :frowning:\n"
                            + $"```{e.ToString()}```"
                        )
                        .Build()
                );
            }
        }


    }
}
