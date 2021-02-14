using PicoShelter_ApiServer.DAL.Enums;
using System;

namespace PicoShelter_ApiServer.BLL.DTO
{
    public record ConfirmationInfoDto
    {
        public string Type { get; init; }
        public DateTime? ValidUntilUTC { get; init; }

        public ConfirmationInfoDto(string type, DateTime? validUntilUtc)
        {
            Type = type;
            ValidUntilUTC = validUntilUtc;
        }

        public ConfirmationInfoDto(ConfirmationType type, DateTime? validUntilUtc) : this(type.ToString(), validUntilUtc)
        {

        }
    }
}
