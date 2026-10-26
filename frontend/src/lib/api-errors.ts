import { isAxiosError } from "axios";
import type { FieldValues, Path, UseFormReturn } from "react-hook-form";

type ProblemDetails = {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
};

const MENSAGEM_PADRAO = "Não foi possível concluir a operação. Tente novamente.";

export function obterMensagemErro(
  error: unknown,
  mensagemPadrao: string = MENSAGEM_PADRAO
): string {
  if (!isAxiosError<ProblemDetails>(error)) return mensagemPadrao;

  if (!error.response) {
    return "Não foi possível conectar ao servidor. Verifique se a API está rodando.";
  }

  if (error.response.status === 403) {
    return "Você não tem permissão para fazer isso — essa ação é restrita a administradores.";
  }

  const { detail, errors } = error.response.data ?? {};
  if (detail) return detail;

  const primeiraMensagem = errors ? Object.values(errors).flat()[0] : undefined;
  return primeiraMensagem ?? mensagemPadrao;
}

export function aplicarErrosDaApi<T extends FieldValues>(
  error: unknown,
  form: UseFormReturn<T>,
  mensagemPadrao?: string
) {
  const errors = isAxiosError<ProblemDetails>(error)
    ? error.response?.data?.errors
    : undefined;

  const camposDoForm = Object.keys(form.getValues());
  const mensagensSemCampo: string[] = [];

  for (const [campo, mensagens] of Object.entries(errors ?? {})) {
    if (camposDoForm.includes(campo)) {
      form.setError(campo as Path<T>, { type: "server", message: mensagens.join(" ") });
    } else {
      mensagensSemCampo.push(...mensagens);
    }
  }

  if (errors && Object.keys(errors).length > 0 && mensagensSemCampo.length === 0) {
    return;
  }

  form.setError("root.server", {
    type: "server",
    message: mensagensSemCampo[0] ?? obterMensagemErro(error, mensagemPadrao),
  });
}
