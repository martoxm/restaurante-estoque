export interface ResultadoPaginado<T> {
  itens: T[];
  pagina: number;
  tamanhoPagina: number;
  totalItens: number;
  totalPaginas: number;
}

export interface ProdutoItemLista {
  id: string;
  nome: string;
  preco: number;
  quantidadeEmEstoque: number;
  categoriaId: string;
  nomeCategoria: string;
}

export interface CriarProdutoRequest {
  nome: string;
  descricao?: string;
  preco: number;
  categoriaId: string;
  quantidadeInicial: number;
}

export interface ProdutoDetalhes {
  id: string;
  nome: string;
  descricao?: string | null;
  preco: number;
  quantidadeEmEstoque: number;
  categoriaId: string;
  nomeCategoria: string;
}

export interface AtualizarProdutoRequest {
  nome: string;
  descricao?: string;
  preco: number;
  categoriaId: string;
}
