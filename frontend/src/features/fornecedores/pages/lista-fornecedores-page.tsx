"use client";

import { useState } from "react";
import Link from "next/link";
import { isAxiosError } from "axios";
import { useUsuarioAtual } from "@/features/auth/hooks/useUsuarioAtual";
import { useFornecedores } from "@/features/fornecedores/hooks/useFornecedores";
import { useExcluirFornecedor } from "@/features/fornecedores/hooks/useExcluirFornecedor";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

export function ListaFornecedoresPage() {
  const { data, isLoading, isError } = useFornecedores();
  const { isAdmin } = useUsuarioAtual();
  const excluirFornecedorMutation = useExcluirFornecedor();

  const [erroExclusao, setErroExclusao] = useState<string | null>(null);

  function handleExcluir(id: string, nome: string) {
    if (!confirm(`Excluir o fornecedor "${nome}"?`)) return;

    setErroExclusao(null);
    excluirFornecedorMutation.mutate(id, {
      onError: (error) => {
        if (isAxiosError(error) && error.response?.status === 403) {
          setErroExclusao(
            "Você não tem permissão para excluir fornecedores — essa ação é restrita a administradores."
          );
          return;
        }

        const detail = isAxiosError<{ detail?: string }>(error)
          ? error.response?.data?.detail
          : undefined;
        setErroExclusao(detail ?? "Não foi possível excluir o fornecedor.");
      },
    });
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <h1 className="font-heading text-lg font-semibold">Fornecedores</h1>
        <Button asChild>
          <Link href="/fornecedores/novo">Novo fornecedor</Link>
        </Button>
      </div>

      {isLoading && (
        <p className="text-sm text-muted-foreground">
          Carregando fornecedores...
        </p>
      )}

      {isError && (
        <p className="text-sm text-destructive">
          Não foi possível carregar os fornecedores. Verifique se a API está
          rodando.
        </p>
      )}

      {erroExclusao && <p className="text-sm text-destructive">{erroExclusao}</p>}

      {data && (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Nome</TableHead>
              <TableHead>Telefone</TableHead>
              <TableHead>E-mail</TableHead>
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
                  Nenhum fornecedor cadastrado.
                </TableCell>
              </TableRow>
            )}

            {data.itens.map((fornecedor) => (
              <TableRow key={fornecedor.id}>
                <TableCell>{fornecedor.nome}</TableCell>
                <TableCell>{fornecedor.telefone || "—"}</TableCell>
                <TableCell>{fornecedor.email || "—"}</TableCell>
                <TableCell>
                  {isAdmin ? (
                    <div className="flex justify-end gap-2">
                      <Button variant="outline" size="sm" asChild>
                        <Link href={`/fornecedores/${fornecedor.id}/editar`}>
                          Editar
                        </Link>
                      </Button>
                      <Button
                        variant="destructive"
                        size="sm"
                        disabled={excluirFornecedorMutation.isPending}
                        onClick={() =>
                          handleExcluir(fornecedor.id, fornecedor.nome)
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
