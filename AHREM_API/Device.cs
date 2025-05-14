namespace AHREM_API
{
    public class Device
    {
        public int? DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public string? Firmware { get; set; }
        public string? MACAddress { get; set; }

        public Device(
            int? deviceId,
            string? deviceName, 
            string? firmware, 
            string? mACAddress)
        {
            DeviceId = deviceId;
            DeviceName = deviceName;
            Firmware = firmware;
            MACAddress = mACAddress;
        }
    }
}
