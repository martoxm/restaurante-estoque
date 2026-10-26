"use client";

import { useState } from "react";
import Link from "next/link";
import { obterMensagemErro } from "@/lib/api-errors";
import { useCategorias } from "@/features/categorias/hooks/useCategorias";
import { useUsuarioAtual } from "@/features/auth/hooks/useUsuarioAtual";
import { useExcluirCategoria } from "@/features/categorias/hooks/useExcluirCategoria";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

export function ListaCategoriasPage() {
  const { data, isLoading, isError } = useCategorias();
  const { isAdmin } = useUsuarioAtual();
  const excluirCategoriaMutation = useExcluirCategoria();

  const [erroExclusao, setErroExclusao] = useState<string | null>(null);

  function handleExcluir(id: string, nome: string) {
    if (!confirm(`Excluir a categoria "${nome}"?`)) return;

    setErroExclusao(null);
    excluirCategoriaMutation.mutate(id, {
      onError: (error) =>
        setErroExclusao(obterMensagemErro(error, "Não foi possível excluir a categoria.")),
    });
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <h1 className="font-heading text-lg font-semibold">Categorias</h1>
        <Button asChild>
          <Link href="/categorias/nova">Nova categoria</Link>
        </Button>
      </div>

      {isLoading && (
        <p className="text-sm text-muted-foreground">
          Carregando categorias...
        </p>
      )}

      {isError && (
        <p className="text-sm text-destructive">
          Não foi possível carregar as categorias. Verifique se a API está
          rodando e se o CORS já foi configurado (etapa 12 do backend).
        </p>
      )}

      {erroExclusao && <p className="text-sm text-destructive">{erroExclusao}</p>}

      {data && (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Nome</TableHead>
              <TableHead>Descrição</TableHead>
              <TableHead>Produtos vinculados</TableHead>
              <TableHead className="text-right">Ações</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {data.itens.length === 0 && (
              <TableRow>
                <TableCell
                  colSpan={4}
                  className="text-center text-muted-foreground"
                >
                  Nenhuma categoria cadastrada.
                </TableCell>
              </TableRow>
            )}

            {data.itens.map((categoria) => (
              <TableRow key={categoria.id}>
                <TableCell>{categoria.nome}</TableCell>
                <TableCell>{categoria.descricao || "—"}</TableCell>
                <TableCell>{categoria.quantidadeProdutos}</TableCell>
                <TableCell>
                  {isAdmin ? (
                    <div className="flex justify-end gap-2">
                      <Button variant="outline" size="sm" asChild>
                        <Link href={`/categorias/${categoria.id}/editar`}>
                          Editar
                        </Link>
                      </Button>
                      <Button
                        variant="destructive"
                        size="sm"
                        disabled={excluirCategoriaMutation.isPending}
                        onClick={() =>
                          handleExcluir(categoria.id, categoria.nome)
                        }
                      >
                        Excluir
                      </Button>
                    </div>
                  ) : (
                    <p className="text-right text-sm text-muted-foreground">
                      —
                    </p>
                  )}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}
    </div>
  );
}
