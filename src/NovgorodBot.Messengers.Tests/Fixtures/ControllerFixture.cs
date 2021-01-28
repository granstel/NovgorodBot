using NovgorodBot.Services;
using NovgorodBot.Services.Configuration;

namespace NovgorodBot.Messengers.Tests.Fixtures
{
    public class ControllerFixture : MessengerController<InputFixture, OutputFixture>
    {
        public ControllerFixture(IMessengerService<InputFixture, OutputFixture> messengerService, MessengerConfiguration configuration) : base(messengerService, configuration)
        {
        }
    }
}
