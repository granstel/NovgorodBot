namespace NovgorodBot.Models
{
    public class GeoArea
    {
        public int Id { get; set; } 

        public float MinLat { get; set; }

        public float MinLon { get; set; }

        public float MaxLat { get; set; }

        public float MaxLon { get; set; }
        
        public string Name { get; set; }

        public bool IsMain { get; set; }

        public bool IsCovers(Geolocation location)
        {
            var result = location.Lon >= MinLon &&
                              location.Lon <= MaxLon &&
                              location.Lat >= MinLat &&
                              location.Lat <= MaxLat;

            return result;
        }
    }
}
