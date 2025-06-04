import { pool } from '../config/db';
import { User } from '../models/User';
import { Device } from '../models/Device';
import { DeviceData } from '../models/DeviceData';

export const DBService = {
  async getUserById(id: number): Promise<User | null> {
    const [rows] = await pool.query('SELECT * FROM user WHERE ID = ?', [id]);
    const users = rows as User[];
    if (users[0]) {
      users[0].IsAdmin = Boolean(users[0].IsAdmin); // convert TinyInt to boolean
    }
    return users[0] || null;
  },

  async getUserByEmail(email: string): Promise<User | null> {
    const [rows] = await pool.query('SELECT * FROM user WHERE Email = ?', [email]);
    const users = rows as User[];
    if (users[0]) {
      users[0].IsAdmin = Boolean(users[0].IsAdmin); // convert TinyInt to boolean
    }
    return users[0] || null;
  },

  async getAllUsers(): Promise<User[]> {
    const [rows] = await pool.query('SELECT * FROM user');
    const users = rows as User[];
    // convert IsAdmin for every user in the list
    users.forEach(user => {
      user.IsAdmin = Boolean(user.IsAdmin);
    });
    return users;
  },


  async getDeviceById(id: number): Promise<Device | null> {
    const [rows] = await pool.query('SELECT * FROM devices WHERE ID = ?', [id]);
    const devices = rows as Device[];
    return devices[0] || null;
  },

  async getAllDevices(): Promise<Device[]> {
    const [rows] = await pool.query('SELECT * FROM devices');
    return rows as Device[];
  },

  async getDeviceDataByDeviceId(deviceId: number): Promise<DeviceData[]> {
    const [rows] = await pool.query('SELECT * FROM data WHERE DeviceID = ?', [deviceId]);
    return rows as DeviceData[];
  },

  async getDeviceDataByRoomName(roomName: string): Promise<DeviceData[]> {
    const [rows] = await pool.query('SELECT * FROM data WHERE RoomName = ?', [roomName]);
    return rows as DeviceData[];
  },

  async addDeviceData(data: DeviceData): Promise<boolean> {
    const query = `INSERT INTO data 
      (ID, RoomName, Temperature, Humidity, Radon, PPM, AirQuality, DeviceID, TimeStamp) 
      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)`;
    const values = [
      data.ID,
      data.RoomName,
      data.Temperature,
      data.Humidity,
      data.Radon,
      data.PPM,
      data.AirQuality,
      data.DeviceID,
      data.TimeStamp
    ];
    const [result] = await pool.query(query, values);
    return (result as any).affectedRows > 0;
  },

  async addDevice(device: Device): Promise<boolean> {
    const query = `INSERT INTO devices (ID, IsActive, Firmware, MAC) VALUES (?, ?, ?, ?)`;
    const values = [device.ID, device.IsActive, device.Firmware, device.MAC];
    const [result] = await pool.query(query, values);
    return (result as any).affectedRows > 0;
  },

  async deleteDevice(id: number): Promise<boolean> {
    const [result] = await pool.query('DELETE FROM devices WHERE ID = ?', [id]);
    return (result as any).affectedRows > 0;
  }
};
