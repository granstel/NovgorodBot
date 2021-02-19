using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using GranSteL.Helpers.Redis;
using NovgorodBot.Services;
using Moq;
using NUnit.Framework;
using Yandex.Dialogs.Models;
using Yandex.Dialogs.Models.Input;
using Request = NovgorodBot.Models.Request;
using Response = NovgorodBot.Models.Response;

namespace NovgorodBot.Messengers.Yandex.Tests
{
    [TestFixture]
    public class YandexServiceTests
    {
        private MockRepository _mockRepository;

        private Mock<IConversationService> _conversationService;
        private Mock<IMapper> _mapper;
        private Mock<IRedisCacheService> _cache;

        private YandexService _target;

        private Fixture _fixture;

        [SetUp]
        public void InitTest()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _conversationService = _mockRepository.Create<IConversationService>();
            _mapper = _mockRepository.Create<IMapper>();
            _cache = _mockRepository.Create<IRedisCacheService>();

            _target = new YandexService(_conversationService.Object, _mapper.Object, _cache.Object);

            _fixture = new Fixture();
        }

        [Test]
        public async Task ProcessIncomingAsync_Invokations_Success()
        {
            var inputModel = _fixture.Build<InputModel>()
                .With(i => i.Session)
                .Create();

            var request = _fixture.Build<Request>()
                .OmitAutoProperties()
                .Create();

            _mapper.Setup(m => m.Map<Request>(It.IsAny<InputModel>())).Returns(request);

            _conversationService.Setup(s => s.GetResponseAsync(request)).ReturnsAsync(() => null);

            _mapper.Setup(m => m.Map(It.IsAny<Request>(), It.IsAny<Response>())).Returns(() => null);

            var output = _fixture.Build<OutputModel>()
                .With(o => o.Session)
                .OmitAutoProperties()
                .Create();

            _mapper.Setup(m => m.Map<OutputModel>(It.IsAny<Response>())).Returns(output);
            _mapper.Setup(m => m.Map(It.IsAny<InputModel>(), It.IsAny<OutputModel>())).Returns(() => null);


            var result = await _target.ProcessIncomingAsync(inputModel);


            _mockRepository.VerifyAll();

            Assert.NotNull(result);
        }
    }
}
