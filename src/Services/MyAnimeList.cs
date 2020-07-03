using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace sunshine.Services
{
    namespace MyAnimeListResults
    {
        // Anime
        public class Anime
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Include)]
            public long mal_id;
            public long? episodes;
            public string url, image_url, title, synopsis, type;
            public bool airing;
            public float score;
            public string start_date, end_date, rated;
        }

        public class AnimeDetailed : Anime
        {
            public string title_english, title_japanese, status, duration, rating, premiered, source;
            public string[] title_synonyms;
            public AnimeAirings aired;
            public MALObject[] genres;
            public AnimeStudio[] studios;
        }

        public struct AnimeAirings
        {
            public string from, to;
            // bruh
            [JsonProperty(PropertyName = "string")]
            public string @string;
        }
        public struct MALObject { public long mal_id; public string type, name, url; }
        public struct AnimeStudio { public string type, name, url; }

        // manga
        public class Manga
        {
            // we only need this tbh
            public long mal_id;
        }
        public class MangaDetailed
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Include)]
            public long mal_id, rank;
            public long? volumes, chapters;
            public string url, image_url, title, title_english, title_japanese,
                status, synopsis, type;
            public MALObject[] genres, authors;
            public string[] title_synonyms;
            public MangaPublished published;
            public bool publishing;
            public float score;
        }

        public class MangaPublished
        {
            public string from, to;
            [JsonProperty(PropertyName = "string")]
            public string @string;
        }

    }
    public class MyAnimeList
    {
        private readonly string baseURL = "https://api.jikan.moe/v3";
        private readonly HttpClient client = new HttpClient();

        private async Task<string> _base_search(string obj, string q)
        {
            var __ = HttpUtility.UrlEncode(q);
            return await client.GetStringAsync($"{baseURL}/search/{obj}?q={__}");
        }

        private async Task<string> _base_info(string obj, long i)
        {
            return await client.GetStringAsync($"{baseURL}/{obj}/{i}");
        }

        public async Task<List<MyAnimeListResults.Anime>> anime(string _)
        {
            return ((JObject)JsonConvert.DeserializeObject(
                await _base_search("anime", _)
            ))["results"]
                .ToObject<List<MyAnimeListResults.Anime>>();
        }

        public async Task<MyAnimeListResults.AnimeDetailed> anime(long _)
        {
            return ((JObject)JsonConvert.DeserializeObject(
                await _base_info("anime", _)
            )).ToObject<MyAnimeListResults.AnimeDetailed>();
        }

        public async Task<List<MyAnimeListResults.Manga>> manga(string _)
        {
            return ((JObject)JsonConvert.DeserializeObject(
                await _base_search("manga", _)
            ))["results"]
                .ToObject<List<MyAnimeListResults.Manga>>();
        }

        public async Task<MyAnimeListResults.MangaDetailed> manga(long _)
        {
            return ((JObject)JsonConvert.DeserializeObject(
                await _base_info("manga", _)
            )).ToObject<MyAnimeListResults.MangaDetailed>();
        }
    }
}