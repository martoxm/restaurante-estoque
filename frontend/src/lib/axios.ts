import axios from "axios";
import Cookies from "js-cookie";

export const api = axios.create({
  baseURL: `${process.env.NEXT_PUBLIC_API_URL}/api/v1`,
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use((config) => {
  const token = Cookies.get("token");

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

const AUTH_PUBLIC_PATHS = ["/auth/login", "/auth/register"];

api.interceptors.response.use(
  (response) => response,
  (error) => {
    const requestUrl: string = error.config?.url ?? "";
    const isPublicAuthRequest = AUTH_PUBLIC_PATHS.some((path) =>
      requestUrl.includes(path)
    );

    if (
      error.response?.status === 401 &&
      !isPublicAuthRequest &&
      typeof window !== "undefined"
    ) {
      Cookies.remove("token", { path: "/" });
      window.location.href = "/login";
    }

    return Promise.reject(error);
  }
);
