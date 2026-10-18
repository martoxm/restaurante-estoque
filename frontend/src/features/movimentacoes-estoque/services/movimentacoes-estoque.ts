import { api } from "@/lib/axios";
import type { CriarMovimentacaoEstoqueRequest } from "@/features/movimentacoes-estoque/types/movimentacao-estoque";

export async function criarMovimentacaoEstoque(
  data: CriarMovimentacaoEstoqueRequest
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>("/movimentacoes-estoque", data);

  return response.data;
}
