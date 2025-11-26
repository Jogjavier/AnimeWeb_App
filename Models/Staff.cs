namespace AnimeWeb_App.Models
{
    public class AnimeStaffResponse
    {
        public List<AnimeStaff> Data { get; set; }
    }

    public class AnimeStaff
    {
        public Person Person { get; set; }
        public List<string> Positions { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
        public PersonImages Images { get; set; }
    }

    public class PersonImages
    {
        public PersonImageSet Jpg { get; set; }
    }

    public class PersonImageSet
    {
        public string Image_Url { get; set; }
    }

}
