using NovgorodBot.Models;

namespace NovgorodBot.Services
{
    public interface IGeolocationService
    {
        GeoArea GetArea(Geolocation location);
    }
}