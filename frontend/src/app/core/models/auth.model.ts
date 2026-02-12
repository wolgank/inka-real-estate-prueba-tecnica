import { z } from 'zod';

export const LoginSchema = z.object({
  username: z.string().min(1, 'El usuario es requerido'),
  password: z.string().min(6, 'La contraseña debe tener al menos 6 caracteres')
});

export type LoginRequest = z.infer<typeof LoginSchema>;

export interface LoginResponse {
  token: string;
  username: string;
  role: 'Admin' | 'Employee';
}