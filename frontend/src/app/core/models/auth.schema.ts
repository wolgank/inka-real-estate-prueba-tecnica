import { z } from 'zod';

export const LoginSchema = z.object({
  username: z.string().min(3, 'Mínimo 3 caracteres'),
  password: z.string().min(6, 'Mínimo 6 caracteres')
});

export type LoginForm = z.infer<typeof LoginSchema>;