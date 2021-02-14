namespace PicoShelter_ApiServer.BLL.DTO.EmailMessagesDto
{
    public record PasswordResetDto : EmailMessageDto
    {
        public string resetPasswordLink { get; init; }
        public int timeoutMinutes { get; init; }
    }
}
