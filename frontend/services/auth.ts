import { api } from "@/lib/axios";
import type { LoginRequest, LoginResponse, RegisterRequest } from "@/types/auth";

export async function login(data: LoginRequest): Promise<LoginResponse> {
  const response = await api.post<LoginResponse>("/auth/login", data);

  return response.data;
}

export async function register(data: RegisterRequest): Promise<LoginResponse> {
  const response = await api.post<LoginResponse>("/auth/register", data);

  return response.data;
}
