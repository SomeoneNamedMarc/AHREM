import jwt from 'jsonwebtoken';
import { Request, Response, NextFunction } from 'express';

type Role = 'admin' | 'user';

interface UserPayload {
  id: number;
  email: string;
  role: Role;
}

export class AuthMockService {
  private static readonly secret = 'my_secret_key';
  private static readonly expiresIn = '1h';

  static generateToken(user: UserPayload): string {
    return jwt.sign(user, this.secret, { expiresIn: this.expiresIn });
  }

  static verifyToken(token: string): UserPayload | null {
    try {
      return jwt.verify(token, this.secret) as UserPayload;
    } catch {
      return null;
    }
  }

  static authenticateToken(req: Request, res: Response, next: NextFunction) {
    const authHeader = req.headers['authorization'];
    const token = authHeader?.split(' ')[1];

    if (!token) {
      return res.status(401).json({ message: 'Token missing' });
    }

    const user = this.verifyToken(token);
    if (!user) {
      return res.status(403).json({ message: 'Invalid token' });
    }

    // Brug type assertion for at undgå TypeScript-fejl
    (req as any).user = user;
    next();
  }

  static authorizeRole(requiredRole: Role) {
    return (req: Request, res: Response, next: NextFunction) => {
      const user = (req as any).user as UserPayload | undefined;
      if (!user || user.role !== requiredRole) {
        return res.status(403).json({ message: 'Access denied' });
      }
      next();
    };
  }
}
