import { useMutation } from "@tanstack/react-query"
import { useRouter } from "next/navigation"
import Cookies from "js-cookie"
import { login } from "@/features/auth/services/auth"

const TOKEN_EXPIRATION_MS = 60 * 60 * 1000

export function useLogin() {
  const router = useRouter() //Aqui a gente pega o roteador do Next.js e guarda na variável router. Mais embaixo é ele que a gente usa, com router.push(...), pra trocar de página.


  return useMutation({
    mutationFn: login, //Aqui a gente fala pro useMutation: "quando alguém chamar mutate(dadosDeLogin), roda a função login com esses dados". É ela que faz a chamada de verdade pro backend e devolve uma Promise com a resposta (token, etc.).
    onSuccess: (data) => {//Esse callback só roda se a chamada der certo (status 2xx). Repara que o data é exatamente o que a função login devolveu — no caso, algo como { token: "..." }.
      Cookies.set("token", data.token, {
        expires: new Date(Date.now() + TOKEN_EXPIRATION_MS),
        path: "/",//O path "/" quer dizer que o cookie vale pro site INTEIRO, não só pra uma página específica.
        sameSite: "lax",
      })
      router.push("/")//Com o cookie salvo, a gente chama router.push("/") pra mandar o usuário pra página inicial do sistema.
    },
  })
}
