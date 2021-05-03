using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PicoShelter_ApiServer.Responses.Models.Stats
{
    public class DbStatsModel
    {
        public int imagesCount { get; set; }
        public int albumsCount { get; set; }
        public int accountsCount { get; set; }

        public Dictionary<string, int> confirmations { get; set; }
    }
}
