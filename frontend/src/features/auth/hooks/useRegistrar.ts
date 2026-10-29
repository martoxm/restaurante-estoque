import { useMutation } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { salvarSessao } from "@/lib/sessao";
import { registrar } from "@/features/auth/services/auth";

export function useRegistrar() {
  const router = useRouter();

  return useMutation({
    mutationFn: registrar,
    onSuccess: (data) => {
      salvarSessao(data);
      router.push("/");
    },
  });
}
