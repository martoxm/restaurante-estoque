import { useMutation } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import Cookies from "js-cookie";
import { registrar } from "@/features/auth/services/auth";

const TOKEN_EXPIRATION_MS = 60 * 60 * 1000;

export function useRegistrar() {
  const router = useRouter();

  return useMutation({
    mutationFn: registrar,
    onSuccess: (data) => {
      Cookies.set("token", data.token, {
        expires: new Date(Date.now() + TOKEN_EXPIRATION_MS),
        path: "/",
        sameSite: "lax",
      });
      router.push("/");
    },
  });
}
