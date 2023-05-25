using PicoShelter_ApiServer.BLL.DTO;

namespace PicoShelter_ApiServer.BLL.Interfaces
{
    public interface IReportService
    {
        void SubmitImage(int imageId, int authorId, string comment);
        PaginationResultDto<ImageShortInfoDto> GetReportedImages(int? starts, int? count);
        PaginationResultDto<ReportMessageDto> GetReportsByImage(int imageId, int? starts, int? count);
        void MarkReportsAsProcessed(int imageId, int ownerAdminId);
    }
}
