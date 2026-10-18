import { api } from "@/lib/axios";
import type { LoginRequest, LoginResponse, RegistrarRequest } from "@/features/auth/types/auth";

export async function login(data: LoginRequest): Promise<LoginResponse> {
  const response = await api.post<LoginResponse>("/auth/login", data);

  return response.data;
}

export async function registrar(data: RegistrarRequest): Promise<LoginResponse> {
  const response = await api.post<LoginResponse>("/auth/registrar", data);

  return response.data;
}
