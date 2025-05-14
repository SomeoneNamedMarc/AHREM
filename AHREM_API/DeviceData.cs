namespace AHREM_API
{
    public class DeviceData
    {
        public int? DeviceId { get; set; }
        public float? Temperature { get; set; }
        public float? Humidity { get; set; }
        public float? Radon { get; set; }
        public float? PPM { get; set; }
        public float? AirQuality { get; set; }
        public string? RoomName { get; set; }
        public int? TimeStamp { get; set; }

        public DeviceData(
            int deviceID,
            float temperature,
            float humidity,
            float radon,
            float ppm,
            float airQuality,
            string roomName,
            int timeStamp)
        {
            DeviceId = deviceID;
            Temperature = temperature;
            Humidity = humidity;
            Radon = radon;
            PPM = ppm;
            AirQuality = airQuality;
            RoomName = roomName;
            TimeStamp = timeStamp;
        }
    }
}
