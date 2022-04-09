#nullable enable
using System;
using System.Text.Json;
using Newtonsoft.Json;

namespace sunshine
{
    internal class MyAnimeListBaseObject
    {
        [JsonProperty("mal_id")] public long MyAnimeListId;
        [JsonProperty("url")] public string Url = "";
    }

    internal class MyAnimeListObject : MyAnimeListBaseObject
    {
        [JsonProperty("type")] public string Type = "";
        [JsonProperty("name")] public string Name = "";
    }

    internal class Anime : MyAnimeListBaseObject
    {
        [JsonProperty("episodes", NullValueHandling = NullValueHandling.Include)] public long? Episodes;
        [JsonProperty("image_url")] public string ImageUrl = "";
        [JsonProperty("title")] public string Title = "";
        [JsonProperty("synopsis")] public string Synopsis = "";
        [JsonProperty("type")] public string Type = "";
        [JsonProperty("airing")] public bool Airing;
        [JsonProperty("score", NullValueHandling = NullValueHandling.Include)] public float Score;
        [JsonProperty("start_date")] public string? StartDate = "";
        [JsonProperty("end_date")] public string? EndDate;
        [JsonProperty("rated")] public string Rated = "";
    }

    internal class AnimeDetailed : Anime
    {
        [JsonProperty("title_english")] public string? TitleEnglish;
        [JsonProperty("title_japanese")] public string? TitleJapanese;
        [JsonProperty("title_synonyms")] public string[]? TitleSynonyms = Array.Empty<string>();
        [JsonProperty("status")] public string Status = "";
        [JsonProperty("duration")] public string Duration = "";
        [JsonProperty("rating")] public string Rating = "";
        [JsonProperty("premiered")] public string? Premiered = "";
        [JsonProperty("source")] public string Source = "";
        [JsonProperty("aired")] public AnimeAirings Aired;
        [JsonProperty("genres")] public MyAnimeListObject[] Genres = Array.Empty<MyAnimeListObject>();
        [JsonProperty("studios")] public AnimeStudio[] Studios = Array.Empty<AnimeStudio>();
    }

    internal class AnimeStudio : MyAnimeListObject { }
    internal struct AnimeAirings
    {
        [JsonProperty("from")] public string From;
        [JsonProperty("to")] public string? To;
        [JsonProperty("string")] public string Stringified;
    }
}