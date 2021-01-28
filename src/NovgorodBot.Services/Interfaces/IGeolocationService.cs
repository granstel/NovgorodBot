using NovgorodBot.Models;

namespace NovgorodBot.Services
{
    public interface IGeolocationService
    {
        bool IsGeolocationAtArea(Geolocation location, GeoArea area);
    }
}