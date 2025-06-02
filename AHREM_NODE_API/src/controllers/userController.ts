import { Request, Response } from 'express';
import db from '../config/db';
import redisClient from '../config/redis';

export const getUsers = async (req: Request, res: Response) => {
  try {
    const cached = await redisClient.get('users');
    if (cached) {
      return res.json(JSON.parse(cached));
    }

    const conn = await db.getConnection();
    const rows = await conn.query('SELECT id, name FROM users');
    conn.release();

    await redisClient.setEx('users', 3600, JSON.stringify(rows)); // cache 1 hour
    res.json(rows);
  } catch (err) {
    res.status(500).json({ error: (err as Error).message });
  }
};
