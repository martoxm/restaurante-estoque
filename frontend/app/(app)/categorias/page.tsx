"use client";

import { useState } from "react";
import Link from "next/link";
import { isAxiosError } from "axios";
import { useCategories } from "@/hooks/useCategories";
import { useDeleteCategory } from "@/hooks/useDeleteCategory";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

export default function CategoriasPage() {
  const { data, isLoading, isError } = useCategories();
  const deleteCategoryMutation = useDeleteCategory();

  const [deleteError, setDeleteError] = useState<string | null>(null);

  function handleDelete(id: string, name: string) {
    if (!confirm(`Excluir a categoria "${name}"?`)) return;

    setDeleteError(null);
    deleteCategoryMutation.mutate(id, {
      onError: (error) => {
        const detail = isAxiosError<{ detail?: string }>(error)
          ? error.response?.data?.detail
          : undefined;
        setDeleteError(detail ?? "Não foi possível excluir a categoria.");
      },
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

      {deleteError && <p className="text-sm text-destructive">{deleteError}</p>}

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
            {data.items.length === 0 && (
              <TableRow>
                <TableCell
                  colSpan={4}
                  className="text-center text-muted-foreground"
                >
                  Nenhuma categoria cadastrada.
                </TableCell>
              </TableRow>
            )}

            {data.items.map((category) => (
              <TableRow key={category.id}>
                <TableCell>{category.name}</TableCell>
                <TableCell>{category.description || "—"}</TableCell>
                <TableCell>{category.productCount}</TableCell>
                <TableCell>
                  <div className="flex justify-end gap-2">
                    <Button variant="outline" size="sm" asChild>
                      <Link href={`/categorias/${category.id}/editar`}>
                        Editar
                      </Link>
                    </Button>
                    <Button
                      variant="destructive"
                      size="sm"
                      disabled={deleteCategoryMutation.isPending}
                      onClick={() => handleDelete(category.id, category.name)}
                    >
                      Excluir
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
