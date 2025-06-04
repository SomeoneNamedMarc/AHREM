import express from 'express';
import { DBService } from '../services/dbService';

const router = express.Router();

// User routes
router.get('/users/:id', async (req, res) => {
  const user = await DBService.getUserById(Number(req.params.id));
  res.json(user ?? {});
});

router.get('/users/by-email', async (req, res) => {
  const user = await DBService.getUserByEmail(req.query.email as string);
  res.json(user ?? {});
});

router.get('/users', async (_, res) => {
  const users = await DBService.getAllUsers();
  res.json(users);
});

// Device routes
router.get('/devices/:id', async (req, res) => {
  const device = await DBService.getDeviceById(Number(req.params.id));
  res.json(device ?? {});
});

router.get('/devices', async (_, res) => {
  const devices = await DBService.getAllDevices();
  res.json(devices);
});

router.delete('/devices/:id', async (req, res) => {
  const success = await DBService.deleteDevice(Number(req.params.id));
  res.status(success ? 200 : 400).json({ success });
});

// Device Data routes
router.get('/devices/:deviceId/data', async (req, res) => {
  const data = await DBService.getDeviceDataByDeviceId(Number(req.params.deviceId));
  res.json(data);
});

router.get('/data/by-room', async (req, res) => {
  const roomName = req.query.roomName as string;
  const data = await DBService.getDeviceDataByRoomName(roomName);
  res.json(data);
});

router.post('/data', async (req, res) => {
  const success = await DBService.addDeviceData(req.body);
  res.status(success ? 201 : 400).json({ success });
});

router.post('/devices', async (req, res) => {
  const success = await DBService.addDevice(req.body);
  res.status(success ? 201 : 400).json({ success });
});

// Auth route (new)
router.post('/auth/login', async (req, res) => {
  const { email, password } = req.body;
  const user = await DBService.getUserByEmail(email);
  if (user && user.Password === password) {
    res.status(200).json({ success: true });
  } else {
    res.status(401).json({ success: false });
  }
});

export default router;
