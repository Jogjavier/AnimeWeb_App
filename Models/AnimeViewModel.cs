namespace AnimeWeb_App.Models
{
    public class AnimeViewModel
    {
        public Anime Anime { get; set; }
        public List<AnimeCharacterWrapper> Characters { get; set; }
        public int CharactersPage { get; set; }
        public int CharactersTotalPages { get; set; }
        public List<AnimeStaff> Staff { get; set; }
        public int StaffPage { get; set; }
        public int StaffTotalPages { get; set; }
    }

}
