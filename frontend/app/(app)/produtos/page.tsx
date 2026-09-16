"use client";

import Link from "next/link";
import { useProducts } from "@/hooks/useProducts";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

const currencyFormatter = new Intl.NumberFormat("pt-BR", {
  style: "currency",
  currency: "BRL",
});

export default function ProdutosPage() {
  const { data, isLoading, isError } = useProducts();

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <h1 className="font-heading text-lg font-semibold">Produtos</h1>
        <div className="flex gap-2">
          <Button variant="outline" asChild>
            <Link href="/movimentacoes/nova">Nova movimentação</Link>
          </Button>
          <Button asChild>
            <Link href="/produtos/novo">Novo produto</Link>
          </Button>
        </div>
      </div>

      {isLoading && (
        <p className="text-sm text-muted-foreground">Carregando produtos...</p>
      )}

      {isError && (
        <p className="text-sm text-destructive">
          Não foi possível carregar os produtos. Verifique se a API está
          rodando e se o CORS já foi configurado (etapa 12 do backend).
        </p>
      )}

      {data && (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Nome</TableHead>
              <TableHead>Categoria</TableHead>
              <TableHead>Preço</TableHead>
              <TableHead>Estoque</TableHead>
              <TableHead className="text-right">Ações</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {data.items.length === 0 && (
              <TableRow>
                <TableCell colSpan={5} className="text-center text-muted-foreground">
                  Nenhum produto cadastrado.
                </TableCell>
              </TableRow>
            )}

            {data.items.map((product) => (
              <TableRow key={product.id}>
                <TableCell>{product.name}</TableCell>
                <TableCell>{product.categoryName}</TableCell>
                <TableCell>{currencyFormatter.format(product.price)}</TableCell>
                <TableCell>{product.quantityInStock}</TableCell>
                <TableCell>
                  <div className="flex justify-end">
                    <Button variant="outline" size="sm" asChild>
                      <Link href={`/produtos/${product.id}/editar`}>
                        Editar
                      </Link>
                    </Button>
                  </div>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}
    </div>
  );
}
