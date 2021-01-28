namespace NovgorodBot.Models.Internal
{
    public class Request
    {
        public Source Source {get; set;}

        public string ChatHash {get; set;}

        public string UserHash { get; set; }

        public string Text { get; set; }

        public string Language { get; set; }

        public string SessionId { get; set; }

        public bool? NewSession { get; set; }
    }
}