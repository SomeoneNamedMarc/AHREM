using AHREM_API.Models;
using MySqlConnector;
using System.Data;
using System.Diagnostics;

namespace AHREM_API.Services
{
    public class DBService
    {
        private readonly string _connectionString;
        private readonly MySqlConnection _connection;
        public DBService(IConfiguration config) 
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
            _connection = new MySqlConnection(_connectionString);
        }

        public User GetUser(int id)
        {
            if (id != null && id >= 0)
            {
                _connection.Open();
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = $"SELECT * FROM user WHERE ID = {id}";
                    MySqlDataReader sqlData = cmd.ExecuteReader();
                    if (sqlData.Read())
                    {
                        return new User
                        {
                            ID = sqlData.GetInt16("ID"),
                            Email = sqlData.GetString("Email"),
                            Password = sqlData.GetString("Password"),
                            IsAdmin = sqlData.GetBoolean("IsAdmin")
                        };
                    }
                }
            }
            return null;
        }
        public User GetUser(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                _connection.Open();
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = $"SELECT * FROM user WHERE Email = \"{email}\"";
                    MySqlDataReader sqlData = cmd.ExecuteReader();
                    if (sqlData.Read())
                    {
                        return new User
                        {
                            ID = sqlData.GetInt16("ID"),
                            Email = sqlData.GetString("Email"),
                            Password = sqlData.GetString("Password"),
                            IsAdmin = sqlData.GetBoolean("IsAdmin")
                        };
                    }
                }
            }
            return null;
        }

        public List<User> GetAllUsers()
        {
            _connection.Open();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "SELECT * FROM user";

                MySqlDataReader sqlData = cmd.ExecuteReader();

                List<User> tempList = new List<User>();
                while (sqlData.Read())
                {
                    tempList.Add(new User
                    {
                        ID = sqlData.GetInt16("ID"),
                        Email = sqlData.GetString("Email"),
                        Password = sqlData.GetString("Password"),
                        IsAdmin = sqlData.GetBoolean("IsAdmin")
                    });
                }
                return tempList;
            }
        }

        public List<DeviceData> GetDeviceDataForDeviceId(int deviceId)
        {
            if(_connection != null)
            {
                _connection.Open();
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = $"SELECT * FROM data WHERE DeviceID = {deviceId}";

                    MySqlDataReader sqlData = cmd.ExecuteReader();

                    List<DeviceData> tempList = new List<DeviceData>();

                    while (sqlData.Read())
                    {
                        tempList.Add(
                            new DeviceData
                            {
                                ID = sqlData.GetInt16("ID"),
                                RoomName = sqlData.GetString("RoomName"),
                                Temperature = sqlData.GetFloat("Temperature"),
                                Humidity = sqlData.GetFloat("Humidity"),
                                Radon = sqlData.GetFloat("Radon"),
                                PPM = sqlData.GetFloat("PPM"),
                                AirQuality = sqlData.GetFloat("AirQuality"),
                                DeviceID = sqlData.GetInt16("DeviceID"),
                                TimeStamp = sqlData.GetDateTime("TimeStamp")
                            }
                        );
                    }
                    return tempList;
                }
            }
            return new List<DeviceData>();
        }

        public List<DeviceData> GetDeviceDataForRoomName(string roomName)
        {
            if (_connection != null)
            {
                _connection.Open();
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = $"SELECT * FROM data WHERE RoomName = \"{roomName}\"";

                    MySqlDataReader sqlData = cmd.ExecuteReader();

                    List<DeviceData> tempList = new List<DeviceData>();

                    while (sqlData.Read())
                    {
                        tempList.Add(
                            new DeviceData
                            {
                                ID = sqlData.GetInt16("ID"),
                                RoomName = sqlData.GetString("RoomName"),
                                Temperature = sqlData.GetFloat("Temperature"),
                                Humidity = sqlData.GetFloat("Humidity"),
                                Radon = sqlData.GetFloat("Radon"),
                                PPM = sqlData.GetFloat("PPM"),
                                AirQuality = sqlData.GetFloat("AirQuality"),
                                DeviceID = sqlData.GetInt16("DeviceID"),
                                TimeStamp = sqlData.GetDateTime("TimeStamp")
                            }
                        );
                    }
                    return tempList;
                }
            }
            return new List<DeviceData>();
        }

        public Device GetDevice(int i)
        {
            if(_connection != null)
            {
                _connection.Open();
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = $"SELECT * FROM devices WHERE ID = {i}";

                    MySqlDataReader sqlData = cmd.ExecuteReader();

                    if (sqlData.Read())
                    {
                        return new Device
                        {
                            ID = sqlData.GetInt16("ID"),
                            IsActive = sqlData.GetBoolean("IsActive"),
                            Firmware = sqlData.GetString("Firmware"),
                            MAC = sqlData.GetString("MAC")
                        };
                    }
                }
            }
            return null;
        }

        public List<Device> GetAllDevices()
        {
            if (_connection != null)
            {
                _connection.Open();
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = $"SELECT * FROM devices";

                    MySqlDataReader sqlData = cmd.ExecuteReader();

                    List<Device> tempList = new List<Device>();

                    while (sqlData.Read())
                    {
                        tempList.Add(new Device
                        {
                            ID = sqlData.GetInt16("ID"),
                            IsActive = sqlData.GetBoolean("IsActive"),
                            Firmware = sqlData.GetString("Firmware"),
                            MAC = sqlData.GetString("MAC")
                        });
                    }

                    return tempList;
                }
            }
            return null;
        }

        public bool PostDeviceData(DeviceData deviceData)
        {
            if (_connection != null)
            {
                string query = "INSERT INTO data (ID, RoomName, Temperature, Humidity, Radon, PPM, AirQuality, DeviceID, TimeStamp) VALUES (@ID, @RoomName, @Temperature, @Humidity, @Radon, @PPM, @AirQuality, @DeviceID, @TimeStamp)";
                _connection.Open();
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("@ID", deviceData.ID);
                    cmd.Parameters.AddWithValue("@RoomName", deviceData.RoomName);
                    cmd.Parameters.AddWithValue("@Temperature", deviceData.Temperature);
                    cmd.Parameters.AddWithValue("@Humidity", deviceData.Humidity);
                    cmd.Parameters.AddWithValue("@Radon", deviceData.Radon);
                    cmd.Parameters.AddWithValue("@PPM", deviceData.PPM);
                    cmd.Parameters.AddWithValue("@AirQuality", deviceData.AirQuality);
                    cmd.Parameters.AddWithValue("@DeviceID", deviceData.DeviceID);
                    cmd.Parameters.AddWithValue("@TimeStamp", deviceData.TimeStamp);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            return false;
        }

        public bool AddDevice(Device device)
        {
            if (_connection != null)
            {
                string query = "INSERT INTO devices (ID, IsActive, Firmware, MAC) VALUES (@ID, @IsActive, @Firmware, @MAC)";

                _connection.Open();

                using (var cmd = _connection.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("@ID", device.ID);
                    cmd.Parameters.AddWithValue("@RoomName", device.IsActive);
                    cmd.Parameters.AddWithValue("@Temperature", device.Firmware);
                    cmd.Parameters.AddWithValue("@Humidity", device.MAC);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            return false;
        }

        public bool DeleteDevice(int id)
        {
            if (_connection != null)
            {
                _connection.Open();

                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = "DELETE FROM devices WHERE ID = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    int affectedRows = cmd.ExecuteNonQuery();
                    return affectedRows > 0;
                }
            }
            return false;
        }

        /*
        /// <summary>
        /// Queries db, provide MySQL object and query string.
        /// </summary>
        /// <param name="mySqlConnection"></param>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        public List<object> QueryDB(MySqlConnection mySqlConnection, string strQuery)
        {
            List<object> strData = new List<object>();

            if (string.IsNullOrEmpty(strQuery) || mySqlConnection.Equals(null))
            {
                return null;
            }
            mySqlConnection.Open();
            using (var cmd = mySqlConnection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 300;
                cmd.CommandText = strQuery;

                MySqlDataReader sqlData = cmd.ExecuteReader();

                while (sqlData.Read())
                {
                    Debug.WriteLine($"sqlData depth: {sqlData.VisibleFieldCount}. Data: {sqlData[1]}");
                }

                if (sqlData == null)
                {
                    cmd.Dispose();
                    return null;
                }
                else
                {

                    strData.Add("test");
                    cmd.Dispose();
                }

                mySqlConnection.Close();

                return strData;
            }
        }
        private DateTime UnixTimeToDateTime(long unixTime)
        {
            DateTime epoch = DateTime.UnixEpoch;
            return epoch.AddSeconds(unixTime).ToLocalTime();
        }

        private long DateTimeToUnixTime(DateTime dateTime)
        {
            DateTime epoch = DateTime.UnixEpoch;
            return (long)(dateTime.ToUniversalTime() - epoch).TotalSeconds;
        }
        
        public MySqlConnection ConnectToDB(string connectionString)
        {
            try
            {
                var mySqlConnection = new MySqlConnection(connectionString);
                return mySqlConnection;
            }
            catch (MySqlException e)
            {
                Debug.WriteLine($"Exception caught: {e.ToString}");
                return null;
            }
        }
        */
    }
}
