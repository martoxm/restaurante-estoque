import { useMutation } from "@tanstack/react-query"
import { useRouter } from "next/navigation"
import { salvarSessao } from "@/lib/sessao"
import { login } from "@/features/auth/services/auth"


export function useLogin() {
  const router = useRouter() //Aqui a gente pega o roteador do Next.js e guarda na variável router. Mais embaixo é ele que a gente usa, com router.push(...), pra trocar de página.


  return useMutation({
    mutationFn: login, //Aqui a gente fala pro useMutation: "quando alguém chamar mutate(dadosDeLogin), roda a função login com esses dados". É ela que faz a chamada de verdade pro backend e devolve uma Promise com a resposta (token, etc.).
    onSuccess: (data) => {//Esse callback só roda se a chamada der certo (status 2xx). Repara que o data é exatamente o que a função login devolveu — no caso, algo como { token: "..." }.
      salvarSessao(data)//Guarda o token e o refreshToken (Etapa M4b) nos cookies.
      router.push("/")//Com o cookie salvo, a gente chama router.push("/") pra mandar o usuário pra página inicial do sistema.
    },
  })
}
