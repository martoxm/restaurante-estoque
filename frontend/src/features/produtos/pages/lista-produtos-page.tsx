"use client";

import { useState } from "react";
import Link from "next/link";
import { isAxiosError } from "axios";
import { useProdutos } from "@/features/produtos/hooks/useProdutos";
import { useUsuarioAtual } from "@/features/auth/hooks/useUsuarioAtual";
import { useExcluirProduto } from "@/features/produtos/hooks/useExcluirProduto";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

const formatadorMoeda = new Intl.NumberFormat("pt-BR", {
  style: "currency",
  currency: "BRL",
});

export function ListaProdutosPage() {
  const { data, isLoading, isError } = useProdutos();
  const { isAdmin } = useUsuarioAtual();
  const excluirProdutoMutation = useExcluirProduto();

  const [erroExclusao, setErroExclusao] = useState<string | null>(null);

  function handleExcluir(id: string, nome: string) {
    if (!confirm(`Excluir o produto "${nome}"?`)) return;

    setErroExclusao(null);
    excluirProdutoMutation.mutate(id, {
      onError: (error) => {
        if (isAxiosError(error) && error.response?.status === 403) {
          setErroExclusao(
            "Você não tem permissão para excluir produtos — essa ação é restrita a administradores."
          );
          return;
        }

        const detail = isAxiosError<{ detail?: string }>(error)
          ? error.response?.data?.detail
          : undefined;
        setErroExclusao(detail ?? "Não foi possível excluir o produto.");
      },
    });
  }

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

      {erroExclusao && <p className="text-sm text-destructive">{erroExclusao}</p>}

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
            {data.itens.length === 0 && (
              <TableRow>
                <TableCell colSpan={5} className="text-center text-muted-foreground">
                  Nenhum produto cadastrado.
                </TableCell>
              </TableRow>
            )}

            {data.itens.map((produto) => (
              <TableRow key={produto.id}>
                <TableCell>{produto.nome}</TableCell>
                <TableCell>{produto.nomeCategoria}</TableCell>
                <TableCell>{formatadorMoeda.format(produto.preco)}</TableCell>
                <TableCell>{produto.quantidadeEmEstoque}</TableCell>
                <TableCell>
                  <div className="flex justify-end gap-2">
                    <Button variant="outline" size="sm" asChild>
                      <Link href={`/produtos/${produto.id}/editar`}>
                        Editar
                      </Link>
                    </Button>
                    {isAdmin && (
                      <Button
                        variant="destructive"
                        size="sm"
                        disabled={excluirProdutoMutation.isPending}
                        onClick={() => handleExcluir(produto.id, produto.nome)}
                      >
                        Excluir
                      </Button>
                    )}
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
