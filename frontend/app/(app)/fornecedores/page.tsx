"use client";

import { useState } from "react";
import Link from "next/link";
import { isAxiosError } from "axios";
import { useCurrentUser } from "@/hooks/useCurrentUser";
import { useSuppliers } from "@/hooks/useSuppliers";
import { useDeleteSupplier } from "@/hooks/useDeleteSupplier";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

export default function FornecedoresPage() {
  const { data, isLoading, isError } = useSuppliers();
  const { isAdmin } = useCurrentUser();
  const deleteSupplierMutation = useDeleteSupplier();

  const [deleteError, setDeleteError] = useState<string | null>(null);

  function handleDelete(id: string, name: string) {
    if (!confirm(`Excluir o fornecedor "${name}"?`)) return;

    setDeleteError(null);
    deleteSupplierMutation.mutate(id, {
      onError: (error) => {
        if (isAxiosError(error) && error.response?.status === 403) {
          setDeleteError(
            "Você não tem permissão para excluir fornecedores — essa ação é restrita a administradores."
          );
          return;
        }

        const detail = isAxiosError<{ detail?: string }>(error)
          ? error.response?.data?.detail
          : undefined;
        setDeleteError(detail ?? "Não foi possível excluir o fornecedor.");
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

      {deleteError && <p className="text-sm text-destructive">{deleteError}</p>}

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
            {data.items.length === 0 && (
              <TableRow>
                <TableCell
                  colSpan={4}
                  className="text-center text-muted-foreground"
                >
                  Nenhum fornecedor cadastrado.
                </TableCell>
              </TableRow>
            )}

            {data.items.map((supplier) => (
              <TableRow key={supplier.id}>
                <TableCell>{supplier.name}</TableCell>
                <TableCell>{supplier.phone || "—"}</TableCell>
                <TableCell>{supplier.email || "—"}</TableCell>
                <TableCell>
                  {isAdmin ? (
                    <div className="flex justify-end gap-2">
                      <Button variant="outline" size="sm" asChild>
                        <Link href={`/fornecedores/${supplier.id}/editar`}>
                          Editar
                        </Link>
                      </Button>
                      <Button
                        variant="destructive"
                        size="sm"
                        disabled={deleteSupplierMutation.isPending}
                        onClick={() =>
                          handleDelete(supplier.id, supplier.name)
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
