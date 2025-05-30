namespace AHREM_API.Models
{
    public class DeviceData
    {
        public int ID { get; set; }
        public string RoomName { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public float Radon { get; set; }
        public float PPM { get; set; }
        public float AirQuality { get; set; }
        public int DeviceID { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
