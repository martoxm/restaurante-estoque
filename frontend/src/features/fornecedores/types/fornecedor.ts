export interface FornecedorItemLista {
  id: string;
  nome: string;
  telefone?: string | null;
  email?: string | null;
}

export interface FornecedorDetalhes {
  id: string;
  nome: string;
  telefone?: string | null;
  email?: string | null;
}

export interface CriarFornecedorRequest {
  nome: string;
  telefone?: string;
  email?: string;
}

export interface AtualizarFornecedorRequest {
  nome: string;
  telefone?: string;
  email?: string;
}
