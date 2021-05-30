using System.IO;

namespace PicoShelter_ApiServer.Responses.Models.Stats
{
    public class DriveInfoModel
    {
        public long freeSpace { get; set; }
        public long totalSpace { get; set; }
        public string driveName { get; set; }
        public string driveType { get; set; }
        public string driveFormat { get; set; }

        public bool isRepository { get; set; }

        public DriveInfoModel()
        {

        }

        public DriveInfoModel(DriveInfo drive, bool isRepository = false) : this()
        {
            freeSpace = drive.TotalFreeSpace;
            totalSpace = drive.TotalSize;
            driveName = drive.Name;
            driveType = drive.DriveType.ToString();
            driveFormat = drive.DriveFormat;

            this.isRepository = isRepository;
        }
    }
}
