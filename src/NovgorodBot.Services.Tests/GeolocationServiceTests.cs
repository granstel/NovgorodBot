using AutoFixture;
using GranSteL.Helpers.Redis;
using Moq;
using NovgorodBot.Models;
using NUnit.Framework;

namespace NovgorodBot.Services.Tests
{
    [TestFixture]
    public class GeolocationServiceTests
    {
        private MockRepository _mockRepository;

        private Mock<IRedisCacheService> _cacheMock;

        private GeolocationService _target;

        private Fixture _fixture;

        [SetUp]
        public void InitTest()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _cacheMock = _mockRepository.Create<IRedisCacheService>();

            _target = new GeolocationService(_cacheMock.Object);

            _fixture = new Fixture();
        }

        [Test]
        public void WhenLocationAtArea_ThenReturnsArea()
        {
            var location = _fixture.Build<Geolocation>()
                    .With(l => l.Lon, 31.2f)
                    .With(l => l.Lat, 58.5f)
                    .Create();

            var result = _target.GetArea(location);

            Assert.NotNull(result);
        }
        
        [Test]
        public void WhenLocationNull_ThenReturnsNull()
        {
            var result = _target.GetArea(null);

            Assert.Null(result);
        }
    }
}
