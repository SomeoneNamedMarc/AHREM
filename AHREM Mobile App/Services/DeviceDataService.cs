using AHREM_Mobile_App.Models;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHREM_Mobile_App.Services
{
    public class DeviceDataService
    {
        private static int _nextID = 1;
        private static readonly ObservableCollection<DeviceData> _data = new();
        public ObservableCollection<DeviceData> GetAllData() => _data;

        public static void AddData(DeviceData data)
        {
            data.ID = _nextID++;
            _data.Add(data);
        }

        public DeviceDataService()
        {
            // Seed data
            Random rnd = new Random();

            for (int i = 0; i < 30; i++)
            {
                AddData(new DeviceData
                {
                    RoomName = "Our Room",
                    DeviceID = (i % 5) + 1,
                    Temperature = (float)(rnd.NextDouble() * 30),
                    Humidity = (float)(rnd.NextDouble() * 100),
                    Radon = (float)(rnd.NextDouble() * 100),
                    PPM = (float)(rnd.NextDouble() * (1200 - 100) + 100),
                    AirQuality = (float)(rnd.NextDouble() * 100),
                    TimeStamp = DateTime.Now.AddDays(-i)
                });
            }
        }
    }
}
