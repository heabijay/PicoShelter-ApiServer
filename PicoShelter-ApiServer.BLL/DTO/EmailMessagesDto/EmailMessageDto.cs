namespace PicoShelter_ApiServer.BLL.DTO.EmailMessagesDto
{
    public record EmailMessageDto
    {
        public string homeUrl { get; init; }
        public string username { get; init; }
        public string targetEmail { get; init; }
    }
}
