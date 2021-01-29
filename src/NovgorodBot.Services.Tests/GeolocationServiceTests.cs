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
        public void WhenLocationAtArea_ThenReturnsArea()
        {
            var location = _fixture.Create<Geolocation>();

            var result = _target.GetArea(location);

            Assert.NotNull(result);
        }
    }
}
