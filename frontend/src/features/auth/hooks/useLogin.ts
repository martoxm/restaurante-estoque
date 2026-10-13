import { useMutation } from "@tanstack/react-query"
import { useRouter } from "next/navigation"
import Cookies from "js-cookie"
import { login } from "@/features/auth/services/auth"

const TOKEN_EXPIRATION_MS = 60 * 60 * 1000

export function useLogin() {
  const router = useRouter() //Pega a instância do roteador do Next.js, guardada na variável router. Vamos usar router.push(...) mais abaixo pra trocar de página.


  return useMutation({
    mutationFn: login, //Diz ao useMutation: "quando alguém chamar mutate(dadosDeLogin), execute a função login com esses dados". É essa função que dispara a chamada real pro backend e devolve uma Promise com a resposta (token, etc.).
    onSuccess: (data) => {//Define um callback que só roda se a chamada der certo (status 2xx). O data aqui é exatamente o que a função login devolveu — no caso, algo como { token: "..." }.
      Cookies.set("token", data.token, {
        expires: new Date(Date.now() + TOKEN_EXPIRATION_MS),
        path: "/",//O path "/" significa que o cookie vale pra TODO o site, não só pra uma página específica.
        sameSite: "lax",
      })
      router.push("/")//Depois de salvar o cookie, chama router.push("/") pra redirecionar o usuário pra página inicial do sistema.
    },
  })
}
