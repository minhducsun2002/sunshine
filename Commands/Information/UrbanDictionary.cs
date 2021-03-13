using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Interactivity;
using Interactivity.Pagination;
using DiscordColor = Discord.Color;
using sunshine.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace sunshine.Commands
{
    public class UrbanResponse
    {
        [JsonProperty("definition")] public string Definition { get; set; }
        [JsonProperty("example")] public string Example { get; set; }
        [JsonProperty("permalink")] public string Permalink { get; set; }
        [JsonProperty("thumbs_up")] public int Likes { get; set; }
        [JsonProperty("thumbs_down")] public int Dislikes { get; set; }
    }

    public class Urban : CommandModuleBase
    {
        public InteractivityService InteractivityService { get; set; }

        Urban() { this.name = "urban"; }
        private int MAX_LEN = 1000;
        private readonly HttpClient httpClient = new HttpClient();

        [Command("urban")]
        [Category("Information")]
        public async Task urban([Remainder] string query = null)
        {
            var m = Context.Message;
            var err = new EmbedBuilder().WithColor(DiscordColor.Red);
            if (string.IsNullOrEmpty(query))
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
                if (_ == null || _.Count < 1) {
                    await ReplyAsync(
                        null, false,
                        err.WithDescription($"{m.Author.Mention}, I found no results. :frowning:").Build()
                    );
                    return;
                }
                
                // choose the response with highest rating
                _.Sort(
                    (record1, record2) =>
                        (record1.Likes - record1.Dislikes) - (record2.Likes - record2.Dislikes)
                );

                if (!_.Any())
                {
                    await Context.Channel.SendMessageAsync("I found no results.");
                    return;
                }

                var embeds = _.Select((chosen, index) =>
                {
                    string def = chosen.Definition, eg = chosen.Example;
                    if (string.IsNullOrEmpty(eg.Trim())) eg = "[no example]";
                    return new EmbedBuilder()
                        .WithAuthor("Urban Dictionary", "https://vgy.me/ScvJzi.jpg")
                        .WithTitle($"Urban Dictionary definition for **{query}**")
                        .WithUrl(chosen.Permalink)
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
                        .WithFooter($"Definition {index + 1} of {_.Count} | {chosen.Likes} 👍 | {chosen.Dislikes} 👎")
                        .WithTimestamp(DateTime.Now)
                        .WithColor(0, 255, 255)
                        .Build();
                }).ToArray();

                if (embeds.Length == 1)
                    await Context.Channel.SendMessageAsync("", false, embeds.First());
                else
                {
                    await InteractivityService.SendPaginatorAsync(
                        new StaticPaginatorBuilder()
                            .WithEmotes(
                                new Dictionary<IEmote, PaginatorAction>
                                {
                                    {new Emoji("\u2B05"), PaginatorAction.Backward},
                                    {new Emoji("\u27A1"), PaginatorAction.Forward}
                                })
                            .WithCancelledEmbed(null)
                            .WithTimoutedEmbed(null)
                            .WithDeletion(DeletionOptions.None)
                            .WithFooter(PaginatorFooter.None).WithPages(embeds.Select(PageBuilder.FromEmbed)).Build(),
                        Context.Channel,
                        TimeSpan.FromSeconds(20));
                }
            }
            catch (HttpRequestException e)
            {
                await Context.Channel.SendMessageAsync(
                    null, false,
                    err
                        .WithDescription(
                            $"Apologize, {m.Author.Mention}, but an error occurred :frowning:\n```{e}```"
                        )
                        .Build()
                );
            }
        }
    }
}
