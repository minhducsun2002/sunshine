using System;
using Newtonsoft.Json;

namespace sunshine
{
    internal class Character : MyAnimeListBaseObject
    {
        [JsonProperty("name")] public string Name = string.Empty;
        [JsonProperty("image_url")] public string ImageUrl = string.Empty;
        [JsonProperty("alternative_names")] public string[] AlternativeNames = Array.Empty<string>();
    }
    
    internal class CharacterSearch : Character
    {
        [JsonProperty("anime")] public MyAnimeListObject[] Anime = Array.Empty<MyAnimeListObject>();
        [JsonProperty("manga")] public MyAnimeListObject[] Manga = Array.Empty<MyAnimeListObject>();
    }
    
    internal class CharacterDetailed : Character
    {
        [JsonProperty("name_kanji")] public string NameKanji = string.Empty;
        [JsonProperty("about")] public string About = string.Empty;
        [JsonProperty("nicknames")] public string[] Nicknames = Array.Empty<string>();
        [JsonProperty("animeography")] public CharacterAppearanceRecordDetailed[] Animeography = Array.Empty<CharacterAppearanceRecordDetailed>();
        [JsonProperty("mangaography")] public CharacterAppearanceRecordDetailed[] Mangaography = Array.Empty<CharacterAppearanceRecordDetailed>();
        [JsonProperty("voice_actors")] public CharacterVoiceActor[] VoiceActors = Array.Empty<CharacterVoiceActor>();
    }

    internal class CharacterVoiceActor : MyAnimeListObject
    {
        [JsonProperty("language")] public string Language = string.Empty;
        [JsonProperty("image_url")] public string ImageUrl = string.Empty;
    }
    
    internal class CharacterAppearanceRecordDetailed : MyAnimeListBaseObject 
    {
        [JsonProperty("name")] public string Name = string.Empty;
        [JsonProperty("image_url")] public string ImageUrl = string.Empty;
        [JsonProperty("role")] public string Role = string.Empty;
    }
}