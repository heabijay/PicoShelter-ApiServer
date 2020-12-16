using System.Collections.Generic;

namespace PicoShelter_ApiServer.BLL.DTO
{
    public record PaginationResultDto<T> where T : class
    {
        public List<T> data { get; init; }
        public int totalCount { get; init; }

        public PaginationResultDto(List<T> data, int totalCount)
        {
            this.data = data;
            this.totalCount = totalCount;
        }
    }
}
