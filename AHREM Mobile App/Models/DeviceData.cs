using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHREM_Mobile_App.Models
{
    public class DeviceData
    {
        public int ID { get; set; }
        public int DeviceID { get; set; }
        public string? RoomName { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public float Radon { get; set; }
        public float PPM { get; set; }
        public float AirQuality { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
