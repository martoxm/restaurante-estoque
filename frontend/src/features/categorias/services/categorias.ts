import { api } from "@/lib/axios";
import type { ResultadoPaginado } from "@/features/produtos/types/produto";
import type {
  CategoriaDetalhes,
  CategoriaItemLista,
  CriarCategoriaRequest,
  AtualizarCategoriaRequest,
} from "@/features/categorias/types/categoria";

export async function listarCategorias(): Promise<ResultadoPaginado<CategoriaItemLista>> {
  const response = await api.get<ResultadoPaginado<CategoriaItemLista>>("/categorias", {
    params: { tamanhoPagina: 100 },
  });

  return response.data;
}

export async function obterCategoriaPorId(id: string): Promise<CategoriaDetalhes> {
  const response = await api.get<CategoriaDetalhes>(`/categorias/${id}`);

  return response.data;
}

export async function criarCategoria(
  data: CriarCategoriaRequest
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>("/categorias", data);

  return response.data;
}

export async function atualizarCategoria(
  id: string,
  data: AtualizarCategoriaRequest
): Promise<void> {
  await api.put(`/categorias/${id}`, data);
}

export async function excluirCategoria(id: string): Promise<void> {
  await api.delete(`/categorias/${id}`);
}
