namespace PicoShelter_ApiServer.BLL.DTO.EmailMessagesDto
{
    public record EmailChangingNewDto : EmailMessageDto
    {
        public string oldEmail { get; init; }
        public string emailConfirmLink { get; init;}
        public int timeoutMinutes { get; init; }
    }
}
