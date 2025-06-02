using AHREM_Mobile_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHREM_Mobile_App.Services
{
    public class DeviceService
    {
        private static int _nextID = 1;
        private static readonly ObservableCollection<DeviceModel> _devices = new();

        public ObservableCollection<DeviceModel> GetAllDevices() => _devices;

        public static void AddDevice(DeviceModel device)
        {
            device.ID = _nextID++;
            _devices.Add(device);
        }

        public DeviceService()
        {
            // Seed data
            AddDevice(new DeviceModel { RoomName = "Kitchen", Status = "Active" });
            AddDevice(new DeviceModel { RoomName = "Classroom 306", Status = "Inactive" });
            AddDevice(new DeviceModel { RoomName = "Classroom 308", Status = "Active", PPMThresh = 80, TempThresh = 23 });
        }
    }
}
