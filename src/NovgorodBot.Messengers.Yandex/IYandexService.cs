using NovgorodBot.Services;
using Yandex.Dialogs.Models;
using Yandex.Dialogs.Models.Input;

namespace NovgorodBot.Messengers.Yandex
{
    public interface IYandexService : IMessengerService<InputModel, OutputModel>
    {
    }
}