using System;

namespace NovgorodBot.Models
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

        public bool IsOldUser { get; set; }

        public Geolocation Geolocation { get; set; }

        public string RequestType { get; set; }

        public bool IsUserAllowGeolocation()
        {
            var result = RequestType.Equals("Geolocation.Allowed", StringComparison.InvariantCultureIgnoreCase);

            return result;
        }

        public bool IsUserRejectGeolocation()
        {
            var result = RequestType.Equals("Geolocation.Rejected", StringComparison.InvariantCultureIgnoreCase);

            return result;
        }
    }
}