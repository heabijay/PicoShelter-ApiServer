using System.Collections.Generic;

namespace PicoShelter_ApiServer.Responses.Models.Stats
{
    public class StatsModel
    {
        public List<DriveInfoModel> drives { get; set; }
        public DbStatsModel db { get; set; }
    }
}
