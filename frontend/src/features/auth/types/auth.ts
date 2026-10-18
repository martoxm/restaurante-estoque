export interface LoginRequest {
  email: string;
  senha: string;
}

export interface LoginResponse {
  token: string;
  nome: string;
  email: string;
}

export interface RegistrarRequest {
  nome: string;
  email: string;
  senha: string;
  nomeRestaurante: string;
}


export const ROLE_CLAIM =
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

export interface DecodedToken {
  sub: string;
  email: string;
  restauranteId?: string;
  [claim: string]: unknown;
}
