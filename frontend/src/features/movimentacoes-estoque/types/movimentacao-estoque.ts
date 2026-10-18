export type TipoMovimentacaoEstoque = "Entrada" | "Saida";

export interface CriarMovimentacaoEstoqueRequest {
  produtoId: string;
  tipo: TipoMovimentacaoEstoque;
  quantidade: number;
  observacoes?: string;
}
