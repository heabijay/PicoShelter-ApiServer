namespace PicoShelter_ApiServer.BLL.DTO.EmailMessagesDto
{
    public record AlbumInviteDto : EmailMessageDto
    {
        public string albumTitle { get; init; }
        public string albumCode { get; init; }
        public string joinLink { get; init; }
        public int timeoutDays { get; init; }
    }
}
