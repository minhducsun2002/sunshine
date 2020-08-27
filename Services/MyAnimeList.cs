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
        public class MALBaseObject
        {
            public long mal_id;
            public string url;
        }

        // Anime
        public class Anime : MALBaseObject
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Include)]
            public long? episodes;
            public string image_url, title, synopsis, type;
            public bool airing;
            [JsonProperty(NullValueHandling = NullValueHandling.Include)]
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
        public class MALObject : MALBaseObject
        {
            public string type, name;
        }
        public class AnimeStudio : MALObject {}

        // manga
        public class Manga : MALBaseObject {}
        public class MangaDetailed : Manga
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Include)]
            public long rank;
            public long? volumes, chapters;
            public string image_url, title, title_english, title_japanese,
                status, synopsis, type;
            public MALObject[] genres, authors;
            public string[] title_synonyms;
            public MangaPublished published;
            public bool publishing;
            public float? score;
        }

        public class MangaPublished
        {
            public string from, to;
            [JsonProperty(PropertyName = "string")]
            public string @string;
        }

        public class Character : MALBaseObject {}
        public class CharacterDetailed : Character
        {
            public string name, name_kanji, about, image_url;
            public string[] nicknames;
            public CharacterAppearanceRecord[] animeography, mangaography;
        }

        public class CharacterAppearanceRecord : MALBaseObject 
        {
            public string name, image_url;
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

        public async Task<List<MyAnimeListResults.Character>> character(string _)
        {
            return ((JObject)JsonConvert.DeserializeObject(
                await _base_search("character", _)
            ))["results"]
                .ToObject<List<MyAnimeListResults.Character>>();
        }

        public async Task<MyAnimeListResults.CharacterDetailed> character(long _)
        {
            return ((JObject)JsonConvert.DeserializeObject(
                await _base_info("character", _)
            )).ToObject<MyAnimeListResults.CharacterDetailed>();
        }
    }
}