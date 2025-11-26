namespace AnimeWeb_App.Models
{
        public class Anime
        {
            public int Mal_Id { get; set; }
            public string Title { get; set; }
            public string Type { get; set; }
            public string Status { get; set; }
            public int? Episodes { get; set; }
            public string Duration { get; set; }
            public string Rating { get; set; }
            public int? Year { get; set; }
            public AnimeImages Images { get; set; }

            // --- Compañías ---
            public List<Company> Producers { get; set; }
            public List<Company> Licensors { get; set; }
            public List<Company> Studios { get; set; }
        }

        public class Company
        {
            public int Mal_Id { get; set; }
            public string Name { get; set; }
        }

        public class AnimeImages
        {
            public AnimeImageSet Jpg { get; set; }
        }

        public class AnimeImageSet
        {
            public string Image_Url { get; set; }
            public string Large_Image_Url { get; set; }
        }

        public class AnimeDetailResponse
        {
            public Anime Data { get; set; }
        }
}
