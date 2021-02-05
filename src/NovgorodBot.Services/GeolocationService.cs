using System;
using System.Collections.Generic;
using System.Linq;
using GranSteL.Helpers.Redis;
using NovgorodBot.Models;

namespace NovgorodBot.Services
{
    public class GeolocationService : IGeolocationService
    {
        private const string CacheKey = "LOCATIONS";

        private static readonly Random Rnd = new Random();

        private readonly IRedisCacheService _cache;

        public GeolocationService(IRedisCacheService cache)
        {
            _cache = cache;
        }

        public GeoArea GetArea(Geolocation location)
        {
            if (location == null)
            {
                return null;
            }

            _cache.TryGet(CacheKey, out IList<GeoArea> areas);

            //var area = areas.FirstOrDefault(a => a.IsCovers(location));
            //return area;
            var item = Rnd.Next(areas.Count);
            return areas[item];
        }
    }
}
