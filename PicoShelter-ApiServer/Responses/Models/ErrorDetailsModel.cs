using PicoShelter_ApiServer.BLL.Extensions;
using PicoShelter_ApiServer.BLL.Infrastructure;

namespace PicoShelter_ApiServer.Responses.Models
{
    public record ErrorDetailsModel
    {
        public string type { get; init; }
        public string message { get; init; }
        public object data { get; init; }

        public ErrorDetailsModel(ExceptionType type, string message, object data = null)
        {
            this.type = type.ToString();
            this.message = message;
            this.data = data;
        }

        public ErrorDetailsModel(ExceptionType type, object data = null) : this(type, type.GetMessage(), data) { }
    }
}
