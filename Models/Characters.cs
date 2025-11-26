namespace AnimeWeb_App.Models
{
    public class AnimeCharacterResponse
    {
        public List<AnimeCharacterWrapper> Data { get; set; }
    }

    public class AnimeCharacterWrapper
    {
        public Character Character { get; set; }
        public string Role { get; set; }
    }

    public class Character
    {
        public int Mal_Id { get; set; }
        public string Name { get; set; }
        public CharacterImages Images { get; set; }
    }

    public class CharacterImages
    {
        public CharacterImageSet Jpg { get; set; }
    }

    public class CharacterImageSet
    {
        public string Image_Url { get; set; }
    }
}
