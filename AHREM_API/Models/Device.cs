namespace AHREM_API.Models
{
    public class Device
    {
        public int ID { get; set; }
        public bool IsActive { get; set; }
        public string Firmware { get; set; }
        public string MAC { get; set; }

    }
}
