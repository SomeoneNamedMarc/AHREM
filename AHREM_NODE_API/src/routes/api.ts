import express from 'express';
import { DBService } from '../services/dbService';

const router = express.Router();

// User routes
router.get('/user/:id', async (req, res) => {
  const user = await DBService.getUserById(Number(req.params.id));
  res.json(user ?? {});
});

router.get('/user/email/:email', async (req, res) => {
  const user = await DBService.getUserByEmail(req.params.email);
  res.json(user ?? {});
});

router.get('/users', async (_, res) => {
  const users = await DBService.getAllUsers();
  res.json(users);
});

// Device routes
router.get('/device/:id', async (req, res) => {
  const device = await DBService.getDeviceById(Number(req.params.id));
  res.json(device ?? {});
});

router.get('/devices', async (_, res) => {
  const devices = await DBService.getAllDevices();
  res.json(devices);
});

router.delete('/device/:id', async (req, res) => {
  const success = await DBService.deleteDevice(Number(req.params.id));
  res.status(success ? 200 : 400).json({ success });
});

// Device Data routes
router.get('/device-data/:deviceId', async (req, res) => {
  const data = await DBService.getDeviceDataByDeviceId(Number(req.params.deviceId));
  res.json(data);
});

router.get('/room-data/:roomName', async (req, res) => {
  const data = await DBService.getDeviceDataByRoomName(req.params.roomName);
  res.json(data);
});

router.post('/device-data', async (req, res) => {
  const success = await DBService.addDeviceData(req.body);
  res.status(success ? 201 : 400).json({ success });
});

router.post('/device', async (req, res) => {
  const success = await DBService.addDevice(req.body);
  res.status(success ? 201 : 400).json({ success });
});

export default router;
