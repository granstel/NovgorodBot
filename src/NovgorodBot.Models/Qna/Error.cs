using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NovgorodBot.Models.Qna
{
    [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class Error
    {
        [JsonProperty]
        public string Code { get; set; }

        [JsonProperty]
        public string Message { get; set; }
    }
}
