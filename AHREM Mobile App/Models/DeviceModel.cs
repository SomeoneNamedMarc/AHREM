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
        public bool IsActive { get; set; }
        public string Firmware { get; set; } = string.Empty;
        public string MAC { get; set; } = string.Empty;

        // These will be client-assigned
        public string RoomName { get; set; } = string.Empty;
        public int PPMThresh { get; set; } = 80;
        public int TempThresh { get; set; } = 80;
        public int HumThresh { get; set; } = 80;

        // Optional: computed property to simplify UI binding
        public string Status => IsActive ? "Active" : "Inactive";
    }
}
