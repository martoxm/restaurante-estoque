import { api } from "@/lib/axios";
import type { ResultadoPaginado } from "@/features/produtos/types/produto";
import type {
  CriarFornecedorRequest,
  FornecedorDetalhes,
  FornecedorItemLista,
  AtualizarFornecedorRequest,
} from "@/features/fornecedores/types/fornecedor";

export async function listarFornecedores(): Promise<ResultadoPaginado<FornecedorItemLista>> {
  const response = await api.get<ResultadoPaginado<FornecedorItemLista>>("/fornecedores", {
    params: { tamanhoPagina: 100 },
  });

  return response.data;
}

export async function obterFornecedorPorId(id: string): Promise<FornecedorDetalhes> {
  const response = await api.get<FornecedorDetalhes>(`/fornecedores/${id}`);

  return response.data;
}

export async function criarFornecedor(
  data: CriarFornecedorRequest
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>("/fornecedores", data);

  return response.data;
}

export async function atualizarFornecedor(
  id: string,
  data: AtualizarFornecedorRequest
): Promise<void> {
  await api.put(`/fornecedores/${id}`, data);
}

export async function excluirFornecedor(id: string): Promise<void> {
  await api.delete(`/fornecedores/${id}`);
}
