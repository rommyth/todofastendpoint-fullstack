import client from './client';
import type { LoginRequest, LoginResponse, RegisterRequest } from '../types/auth';

const AUTH_URL = '/api';

export const authApi = {
  login: async (data: LoginRequest) => {
    const response = await client.post<LoginResponse>(`${AUTH_URL}/login`, data);
    return response.data;
  },

  register: async (data: RegisterRequest) => {
    const response = await client.post(`${AUTH_URL}/register`, data);
    return response.data;
  },
};
