import { pool } from '../config/db';
import { User } from '../models/User';
import { Device } from '../models/Device';
import { DeviceData } from '../models/DeviceData';

export const DBService = {
  async getUserById(id: number): Promise<User | null> {
    const [rows] = await pool.query('SELECT * FROM user WHERE ID = ?', [id]);
    const users = rows as any[];
    if (users[0]) {
      return {
        id: users[0].ID,
        email: users[0].Email,
        password: users[0].Password,
        isAdmin: Boolean(users[0].IsAdmin),
      };
    }
    return null;
  },

  async getUserByEmail(email: string): Promise<User | null> {
    const [rows] = await pool.query('SELECT * FROM user WHERE Email = ?', [email]);
    const users = rows as any[];
    if (users[0]) {
      return {
        id: users[0].ID,
        email: users[0].Email,
        password: users[0].Password,
        isAdmin: Boolean(users[0].IsAdmin),
      };
    }
    return null;
  },

  async getAllUsers(): Promise<User[]> {
    const [rows] = await pool.query('SELECT * FROM user');
    const users = rows as any[];
    return users.map(u => ({
      id: u.ID,
      email: u.Email,
      password: u.Password,
      isAdmin: Boolean(u.IsAdmin),
    }));
  },

  async getDeviceById(id: number): Promise<Device | null> {
    const [rows] = await pool.query('SELECT * FROM devices WHERE ID = ?', [id]);
    const devices = rows as any[];
    if (devices[0]) {
      return {
        id: devices[0].ID,
        isActive: Boolean(devices[0].IsActive),
        firmware: devices[0].Firmware,
        mac: devices[0].MAC,
      };
    }
    return null;
  },

  async getAllDevices(): Promise<Device[]> {
    const [rows] = await pool.query('SELECT * FROM devices');
    const devices = rows as any[];
    return devices.map(d => ({
      id: d.ID,
      isActive: Boolean(d.IsActive),
      firmware: d.Firmware,
      mac: d.MAC,
    }));
  },

  async addDevice(device: Device): Promise<boolean> {
    const query = `INSERT INTO devices (ID, IsActive, Firmware, MAC) VALUES (?, ?, ?, ?)`;
    const values = [
      device.id,
      device.isActive ? 1 : 0,
      device.firmware,
      device.mac,
    ];
    const [result] = await pool.query(query, values);
    return (result as any).affectedRows > 0;
  },

  async deleteDevice(id: number): Promise<boolean> {
    const [result] = await pool.query('DELETE FROM devices WHERE ID = ?', [id]);
    return (result as any).affectedRows > 0;
  },

  async getDeviceDataByDeviceId(deviceId: number): Promise<DeviceData[]> {
    const [rows] = await pool.query('SELECT * FROM data WHERE DeviceID = ?', [deviceId]);
    const data = rows as any[];
    return data.map(d => ({
      id: d.ID,
      roomName: d.RoomName,
      temperature: d.Temperature,
      humidity: d.Humidity,
      radon: d.Radon,
      ppm: d.PPM,
      airQuality: d.AirQuality,
      deviceId: d.DeviceID,
      timeStamp: d.TimeStamp,
    }));
  },

  async getDeviceDataByRoomName(roomName: string): Promise<DeviceData[]> {
    const [rows] = await pool.query('SELECT * FROM data WHERE RoomName = ?', [roomName]);
    const data = rows as any[];
    return data.map(d => ({
      id: d.ID,
      roomName: d.RoomName,
      temperature: d.Temperature,
      humidity: d.Humidity,
      radon: d.Radon,
      ppm: d.PPM,
      airQuality: d.AirQuality,
      deviceId: d.DeviceID,
      timeStamp: d.TimeStamp,
    }));
  },

  async addDeviceData(data: DeviceData): Promise<boolean> {
    const query = `
      INSERT INTO data 
      (ID, RoomName, Temperature, Humidity, Radon, PPM, AirQuality, DeviceID, TimeStamp) 
      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
    `;
    const values = [
      data.id,
      data.roomName,
      data.temperature,
      data.humidity,
      data.radon,
      data.ppm,
      data.airQuality,
      data.deviceId,
      data.timeStamp,
    ];
    const [result] = await pool.query(query, values);
    return (result as any).affectedRows > 0;
  },
};
