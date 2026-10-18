export interface CategoriaItemLista {
  id: string;
  nome: string;
  descricao?: string | null;
  quantidadeProdutos: number;
}

export interface CategoriaDetalhes {
  id: string;
  nome: string;
  descricao?: string | null;
  quantidadeProdutos: number;
}

export interface CriarCategoriaRequest {
  nome: string;
  descricao?: string;
}

export interface AtualizarCategoriaRequest {
  nome: string;
  descricao?: string;
}
