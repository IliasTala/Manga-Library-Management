namespace ProjectBase.Model
{
    public class MangaModel
    {
        public MangaModel()
        {
        }

        public MangaModel(string id, string licence, string title, string author, string style, string cover, string publisher, string price)
        {
            Id = id;
            Licence = licence;
            Title = title;
            Author = author;
            Style = style;
            Cover = cover;
            Publisher = publisher;
            Price = price;
        }

        public string Id { get; set; }
        public string Licence { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Style { get; set; }
        public string Cover { get; set; }
        public string Publisher { get; set; }
        public string Price { get; set; }
    }
}
