namespace PicoShelter_ApiServer.BLL.DTO.EmailMessagesDto
{
    public record EmailConfirmationDto : EmailMessageDto
    {
        public string confirmEmailLink { get; init; }
        public int timeoutMinutes { get; init; }
    }
}
