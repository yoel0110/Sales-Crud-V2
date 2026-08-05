import type { ApiResponse, LoginRequest, LoginResponse } from '../types';

const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5138').replace(/\/$/, '');

export const login = async (credentials: LoginRequest): Promise<ApiResponse<LoginResponse>> => {
  const response = await fetch(`${API_BASE_URL}/api/v1/auth/login`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify(credentials),
  });

  const result: ApiResponse<LoginResponse> = await response.json();
  return result;
};

export const logout = async (): Promise<ApiResponse<string>> => {
  const response = await fetch(`${API_BASE_URL}/api/v1/auth/logout`, {
    method: 'POST',
    credentials: 'include',
  });

  const result: ApiResponse<string> = await response.json();
  return result;
};

export const checkAuth = async (): Promise<ApiResponse<string>> => {
  const response = await fetch(`${API_BASE_URL}/api/v1/auth/me`, {
    method: 'GET',
    credentials: 'include',
  });

  const result: ApiResponse<string> = await response.json();
  return result;
};
