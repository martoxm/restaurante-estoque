import { api } from "@/lib/axios";
import type {
  CriarProdutoRequest,
  ResultadoPaginado,
  ProdutoDetalhes,
  ProdutoItemLista,
  AtualizarProdutoRequest,
} from "@/features/produtos/types/produto";

export interface ListarProdutosParams {
  pagina?: number;
  tamanhoPagina?: number;
  nome?: string;
  categoriaId?: string;
}

export async function listarProdutos(
  params: ListarProdutosParams = {}
): Promise<ResultadoPaginado<ProdutoItemLista>> {
  const response = await api.get<ResultadoPaginado<ProdutoItemLista>>("/produtos", {
    params,
  });

  return response.data;
}

export async function criarProduto(
  data: CriarProdutoRequest
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>("/produtos", data);

  return response.data;
}

export async function obterProdutoPorId(id: string): Promise<ProdutoDetalhes> {
  const response = await api.get<ProdutoDetalhes>(`/produtos/${id}`);

  return response.data;
}

export async function atualizarProduto(
  id: string,
  data: AtualizarProdutoRequest
): Promise<void> {
  await api.put(`/produtos/${id}`, data);
}

export async function excluirProduto(id: string): Promise<void> {
  await api.delete(`/produtos/${id}`);
}
