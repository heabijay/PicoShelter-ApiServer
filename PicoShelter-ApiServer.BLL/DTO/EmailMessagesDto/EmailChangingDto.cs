namespace PicoShelter_ApiServer.BLL.DTO.EmailMessagesDto
{
    public record EmailChangingDto : EmailMessageDto
    {
        public string newEmail { get; init; }
        public string confirmEmailLink { get; init; }
        public int timeoutMinutes { get; init; }
    }
}
