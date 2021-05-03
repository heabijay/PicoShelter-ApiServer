using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PicoShelter_ApiServer.Responses.Models.Stats
{
    public class StatsModel
    {
        public List<DriveInfoModel> drives { get; set; }
        public DbStatsModel db { get; set; }
    }
}
