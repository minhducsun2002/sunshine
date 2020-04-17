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
        public class Anime
        {
            public Int64 mal_id, episodes;
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
            public AnimeGenre[] genres;
        }

        public struct AnimeAirings {
            public string from, to;
            // bruh
            [JsonProperty(PropertyName = "string")]
            public string @string;
        }
        public struct AnimeGenre { public Int64 mal_id; public string type, name, url; }
    }
    public class MyAnimeList
    {
        private string baseURL = "https://api.jikan.moe/v3";
        private readonly HttpClient client = new HttpClient();
        public async Task<List<MyAnimeListResults.Anime>> anime (string _)
        {
            var __ = HttpUtility.UrlEncode(_);
            var response = await client.GetStringAsync($"{baseURL}/search/anime?q={__}");
            return ((JObject) JsonConvert.DeserializeObject(response))["results"]
                .ToObject<List<MyAnimeListResults.Anime>>();
        }

        public async Task<MyAnimeListResults.AnimeDetailed> anime (Int64 _)
        {
            var response = await client.GetStringAsync($"{baseURL}/anime/{_}");
            return ((JObject) JsonConvert.DeserializeObject(response)).ToObject<MyAnimeListResults.AnimeDetailed>();
        }
    }
}