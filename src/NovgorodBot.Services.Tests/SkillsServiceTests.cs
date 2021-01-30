using AutoFixture;
using Moq;
using NUnit.Framework;

namespace NovgorodBot.Services.Tests
{
    [TestFixture]
    public class SkillsServiceTests
    {
        private MockRepository _mockRepository;

        private SkillsService _target;

        private Fixture _fixture;

        [SetUp]
        public void InitTest()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _target = new SkillsService();

            _fixture = new Fixture();
        }

        [Test]
        public void WhenCorrectAre_ThenReturnsSkills()
        {
            var areaId = 0;

            var result = _target.GetSkills(areaId);

            Assert.NotNull(result, "Список навыков по области = null");
            Assert.IsNotEmpty(result, "Список навыков по области пуст");
        }
    }
}