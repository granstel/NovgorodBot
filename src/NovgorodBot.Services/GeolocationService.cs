using System.Collections.Generic;
using System.Linq;
using NovgorodBot.Models;

namespace NovgorodBot.Services
{
    public class GeolocationService : IGeolocationService
    {
        private static readonly List<GeoArea> Areas = new List<GeoArea>
        {
            {
                new GeoArea
                {
                    Id = 0,
                    Name = "Великий Новгород",
                    MinLon = 31.187262f,
                    MaxLon = 31.429352f,
                    MinLat = 58.470034f,
                    MaxLat = 58.653285f
                }
            }
        };
        
        public GeoArea GetArea(Geolocation location)
        {
            if (location == null)
            {
                return null;
            }

            var area = Areas.FirstOrDefault(a => a.IsCovers(location));

            return area;
        }
    }
}
