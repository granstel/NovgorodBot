using AutoFixture;
using Moq;
using NovgorodBot.Models;
using NUnit.Framework;

namespace NovgorodBot.Services.Tests
{
    [TestFixture]
    public class GeolocationServiceTests
    {
        private MockRepository _mockRepository;

        private GeolocationService _target;

        private Fixture _fixture;

        [SetUp]
        public void InitTest()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _target = new GeolocationService();

            _fixture = new Fixture();
        }

        [Test]
        public void WhenLocationAtArea_ThenReturnsTrue()
        {
            var location = _fixture.Create<Geolocation>();

            var area = _fixture.Build<GeoArea>()
                .With(a => a.MinLat, location.Lat)
                .With(a => a.MinLon, location.Lon)
                .Create();

            var result = _target.IsGeolocationAtArea(location, area);

            Assert.True(result);
        }
    }
}
