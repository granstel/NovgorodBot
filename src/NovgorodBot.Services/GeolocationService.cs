using System;
using System.Collections.Generic;
using System.Linq;
using NovgorodBot.Models;

namespace NovgorodBot.Services
{
    public class GeolocationService : IGeolocationService
    {
        private static readonly Random Rnd = new Random();

        private static readonly List<GeoArea> Areas = new List<GeoArea>
        {
                //new GeoArea
                //{
                //    Id = 0,
                //    Name = "Великий Новгород",
                //    MinLon = 31.187262f,
                //    MaxLon = 31.429352f,
                //    MinLat = 58.470034f,
                //    MaxLat = 58.653285f
                //},
                new GeoArea
                {
                    Id = 0,
                    Name = "Юрьев монастырь",
                    MinLon = 31.187262f,
                    MaxLon = 31.429352f,
                    MinLat = 58.470034f,
                    MaxLat = 58.653285f
                },
                new GeoArea
                {
                    Id = 0,
                    Name = "Церковь Николы на Липне",
                    MinLon = 31.187262f,
                    MaxLon = 31.429352f,
                    MinLat = 58.470034f,
                    MaxLat = 58.653285f
                },
                new GeoArea
                {
                    Id = 0,
                    Name = "Софийский собор",
                    MinLon = 31.187262f,
                    MaxLon = 31.429352f,
                    MinLat = 58.470034f,
                    MaxLat = 58.653285f
                },
                new GeoArea
                {
                    Id = 0,
                    Name = "Новгородский кремль",
                    MinLon = 31.187262f,
                    MaxLon = 31.429352f,
                    MinLat = 58.470034f,
                    MaxLat = 58.653285f
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
            //var item = Rnd.Next(Areas.Count);

            //return Areas[item];
        }
    }
}
