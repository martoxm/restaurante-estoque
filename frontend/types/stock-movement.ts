export type StockMovementTypeValue = "Entrada" | "Saida";

export interface CreateStockMovementRequest {
  productId: string;
  type: StockMovementTypeValue;
  quantity: number;
  notes?: string;
}
