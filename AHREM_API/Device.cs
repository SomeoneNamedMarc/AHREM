namespace AHREM_API
{
    public class Device
    {
        public int? DeviceId { get; set; }
        public bool IsActive { get; set; }
        public string? DeviceName { get; set; }
        public string? Firmware { get; set; }
        public string? MACAddress { get; set; }
    }
}
