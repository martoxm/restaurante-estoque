import Cookies from "js-cookie";


export const COOKIE_TOKEN = "token";
export const COOKIE_REFRESH_TOKEN = "refreshToken";

const DURACAO_SESSAO_MS = 14 * 24 * 60 * 60 * 1000;

export interface TokensSessao {
  token: string;
  refreshToken: string;
}

export function salvarSessao({ token, refreshToken }: TokensSessao) {
  const opcoes = {
    expires: new Date(Date.now() + DURACAO_SESSAO_MS),
    path: "/",
    sameSite: "lax" as const,
  };

  Cookies.set(COOKIE_TOKEN, token, opcoes);
  Cookies.set(COOKIE_REFRESH_TOKEN, refreshToken, opcoes);
}

export function limparSessao() {
  Cookies.remove(COOKIE_TOKEN, { path: "/" });
  Cookies.remove(COOKIE_REFRESH_TOKEN, { path: "/" });
}

export function obterToken() {
  return Cookies.get(COOKIE_TOKEN);
}

export function obterRefreshToken() {
  return Cookies.get(COOKIE_REFRESH_TOKEN);
}
