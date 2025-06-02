using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHREM_Mobile_App.Models
{
    public class DeviceModel
    {
        public int ID { get; set; }
        public string? RoomName { get; set; }
        public string? Status { get; set; }
        public int? PPMThresh { get; set; }
        public int? TempThresh { get; set; }
        public int? HumThresh { get; set; }
    }
}
