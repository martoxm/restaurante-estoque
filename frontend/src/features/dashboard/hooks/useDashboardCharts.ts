import { useMemo } from "react";
import { useProdutos } from "@/features/produtos/hooks/useProdutos";
import { useCategorias } from "@/features/categorias/hooks/useCategorias";

export interface DistribuicaoCategoriaItem {
  categoria: string;
  produtos: number;
  fill: string;
}

export interface EstoquePorCategoriaItem {
  categoria: string;
  quantidade: number;
  fill: string;
}

export function useDashboardCharts() {
  const {
    data: produtos,
    isLoading: isLoadingProdutos,
    isError: isErrorProdutos,
  } = useProdutos({ tamanhoPagina: 100 });
  const {
    data: categorias,
    isLoading: isLoadingCategorias,
    isError: isErrorCategorias,
  } = useCategorias();

  const distribuicaoCategorias = useMemo<DistribuicaoCategoriaItem[]>(() => {
    if (!categorias) return [];

    return categorias.itens
      .filter((categoria) => categoria.quantidadeProdutos > 0)
      .map((categoria, index) => ({
        categoria: categoria.nome,
        produtos: categoria.quantidadeProdutos,
        fill: `var(--chart-${(index % 5) + 1})`,
      }));
  }, [categorias]);

  const estoquePorCategoria = useMemo<EstoquePorCategoriaItem[]>(() => {
    if (!produtos) return [];

    const totais = new Map<string, number>();
    for (const produto of produtos.itens) {
      const current = totais.get(produto.nomeCategoria) ?? 0;
      totais.set(produto.nomeCategoria, current + produto.quantidadeEmEstoque);
    }

    return [...totais.entries()].map(([categoria, quantidade], index) => ({
      categoria,
      quantidade,
      fill: `var(--chart-${(index % 5) + 1})`,
    }));
  }, [produtos]);

  return {
    distribuicaoCategorias,
    estoquePorCategoria,
    isLoading: isLoadingProdutos || isLoadingCategorias,
    isError: isErrorProdutos || isErrorCategorias,
  };
}
