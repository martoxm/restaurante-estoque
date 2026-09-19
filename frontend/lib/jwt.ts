export function decodeToken<T>(token: string): T | null {
  try {
    const payload = token.split(".")[1];

    const base64 = payload.replace(/-/g, "+").replace(/_/g, "/");
    const json = atob(base64);

    return JSON.parse(json) as T;
  } catch {
    return null;
  }
}
