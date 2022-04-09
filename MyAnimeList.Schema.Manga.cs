#nullable enable
using System;
using Newtonsoft.Json;

namespace sunshine
{
    internal class Manga : MyAnimeListBaseObject
    {
        [JsonProperty("image_url")] public string ImageUrl = "";
        [JsonProperty("publishing")] internal bool Publishing;
        [JsonProperty("type")] public string Type = "";
        [JsonProperty("volumes")] internal long? Volumes;
        [JsonProperty("chapters")] internal long? Chapters;
        [JsonProperty("score")] internal float? Score;
        [JsonProperty("start_date")] public string StartDate = "";
        [JsonProperty("end_date")] public string? EndDate;
        [JsonProperty("synopsis")] public string Synopsis = "";
        [JsonProperty("title")] public string Title = "";
    }

    internal class MangaDetailed : Manga
    {
        [JsonProperty("rank", NullValueHandling = NullValueHandling.Ignore)] internal long Rank;
        [JsonProperty("title_english")] public string? TitleEnglish;
        [JsonProperty("title_japanese")] public string? TitleJapanese;
        [JsonProperty("status")] public string Status = "";
        [JsonProperty("genres")] public MyAnimeListObject[] Genres = Array.Empty<MyAnimeListObject>();
        [JsonProperty("authors")] public MyAnimeListObject[] Authors = Array.Empty<MyAnimeListObject>();
        [JsonProperty("title_synonyms")] internal string[]? TitleSynonyms;
        [JsonProperty("published")] internal MangaPublished Published;
    }

    internal class MangaPublished
    {
        [JsonProperty("from")] internal string From = "";
        [JsonProperty("to")] internal string To = "";
        [JsonProperty("string")] internal string Stringified = "";
    }
}