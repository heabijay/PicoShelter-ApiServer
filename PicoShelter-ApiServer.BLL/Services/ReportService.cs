using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Extensions;
using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.DAL.Interfaces;
using System;
using System.Linq;

namespace PicoShelter_ApiServer.BLL.Services
{
    public class ReportService : IReportService
    {
        protected readonly IUnitOfWork _db;

        public ReportService(IUnitOfWork db)
        {
            _db = db;
        }

        public void SubmitImage(int imageId, int authorId, string comment)
        {
            var accessValidator = new Validators.AccessWithPublicEndpointImageValidator()
            {
                ImageEntity = _db.Images.Get(imageId)
            };
            if (!accessValidator.Validate())
                throw new UnauthorizedAccessException();

            _db.Reports.Add(new()
            {
                ImageId = imageId,
                AuthorId = authorId,
                Comment = comment
            });

            _db.Save();
        }


        public PaginationResultDto<ImageShortInfoDto> GetReportedImages(int? starts, int? count)
        {
            var imageIds = _db.Reports.Where(t => t.ProcessedAt == null).Select(t => t.ImageId).Distinct();

            imageIds = imageIds.Pagination(starts, count, out int total);

            var dto = imageIds.
                ToList()
                .Select(t => _db.Images.Get(t).MapToShortInfo())
                .ToList();

            return new PaginationResultDto<ImageShortInfoDto>(dto, total);
        }

        public PaginationResultDto<ReportMessageDto> GetReportsByImage(int imageId, int? starts, int? count)
        {
            var reports = _db.Reports.Where(t => t.ImageId == imageId && t.ProcessedAt == null);

            reports = reports.Pagination(starts, count, out int total);

            var dto = reports
                .ToList()
                .Select(t => new ReportMessageDto(
                    _db.Accounts.Get(t.AuthorId).MapToAccountInfo(),
                    t.Comment,
                    t.CreatedDateUTC)).ToList();

            return new PaginationResultDto<ReportMessageDto>(dto, total);
        }


        public void MarkReportsAsProcessed(int imageId, int ownerAdminId)
        {
            var reports = _db.Reports.Where(t => t.ImageId == imageId).ToList();

            foreach (var report in reports)
            {
                report.ProcessedAt = DateTime.UtcNow;
                report.ProcessedById = ownerAdminId;

                _db.Reports.Update(report);
            }

            _db.Save();
        }
    }
}
