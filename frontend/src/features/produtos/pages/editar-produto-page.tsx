"use client";

import { useForm } from "react-hook-form";
import Link from "next/link";
import { useParams } from "next/navigation";
import { useCategorias } from "@/features/categorias/hooks/useCategorias";
import { useProduto } from "@/features/produtos/hooks/useProduto";
import { useAtualizarProduto } from "@/features/produtos/hooks/useAtualizarProduto";
import type { ResultadoPaginado } from "@/features/produtos/types/produto";
import type { ProdutoDetalhes, AtualizarProdutoRequest } from "@/features/produtos/types/produto";
import type { CategoriaItemLista } from "@/features/categorias/types/categoria";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { FormRootError } from "@/components/form-root-error";
import { aplicarErrosDaApi } from "@/lib/api-errors";

type ProdutoFormValues = Omit<AtualizarProdutoRequest, "preco"> & {
  preco: string;
};

export function EditarProdutoPage() {
  const params = useParams<{ id: string }>();
  const { id } = params;

  const { data: produto, isLoading, isError } = useProduto(id);

  const { data: categorias, isLoading: isLoadingCategorias } = useCategorias();

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <h1 className="font-heading text-lg font-semibold">Editar produto</h1>
        <Button variant="outline" asChild>
          <Link href="/produtos">Voltar</Link>
        </Button>
      </div>

      {isLoading && (
        <p className="text-sm text-muted-foreground">Carregando produto...</p>
      )}

      {isError && (
        <p className="text-sm text-destructive">
          Não foi possível carregar esse produto.
        </p>
      )}

      {produto && isLoadingCategorias && (
        <p className="text-sm text-muted-foreground">
          Carregando categorias...
        </p>
      )}

      {produto && categorias && (
        <EditarProdutoForm id={id} produto={produto} categorias={categorias} />
      )}
    </div>
  );
}

function EditarProdutoForm({
  id,
  produto,
  categorias,
}: {
  id: string;
  produto: ProdutoDetalhes;
  categorias: ResultadoPaginado<CategoriaItemLista>;
}) {
  const form = useForm<ProdutoFormValues>({
    defaultValues: {
      nome: produto.nome,
      descricao: produto.descricao ?? "",
      preco: String(produto.preco),
      categoriaId: produto.categoriaId,
    },
  });

  const atualizarProdutoMutation = useAtualizarProduto();

  function onSubmit(data: ProdutoFormValues) {
    atualizarProdutoMutation.mutate({
      id,
      data: {
        nome: data.nome,
        descricao: data.descricao || undefined,
        preco: Number(data.preco),
        categoriaId: data.categoriaId,
      },
    }, {
      onError: (error) => aplicarErrosDaApi(error, form),
    });
  }

  return (
    <Card className="max-w-lg">
      <CardHeader>
        <CardTitle>Dados do produto</CardTitle>
      </CardHeader>
      <CardContent>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(onSubmit)}
            className="flex flex-col gap-4"
          >
            <FormField
              control={form.control}
              name="nome"
              rules={{
                required: "O nome do produto é obrigatório.",
                maxLength: {
                  value: 150,
                  message: "O nome deve ter no máximo 150 caracteres.",
                },
              }}
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Nome</FormLabel>
                  <FormControl>
                    <Input placeholder="Ex: Arroz branco 5kg" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="descricao"
              rules={{
                maxLength: {
                  value: 500,
                  message: "A descrição deve ter no máximo 500 caracteres.",
                },
              }}
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Descrição (opcional)</FormLabel>
                  <FormControl>
                    <Input placeholder="Ex: Tipo 1, pacote de 5kg" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="categoriaId"
              rules={{ required: "Selecione uma categoria." }}
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Categoria</FormLabel>
                  <Select value={field.value} onValueChange={field.onChange}>
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue placeholder="Selecione uma categoria" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {categorias.itens.map((categoria) => (
                        <SelectItem key={categoria.id} value={categoria.id}>
                          {categoria.nome}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="preco"
              rules={{
                required: "O preço é obrigatório.",
                min: { value: 0, message: "O preço não pode ser negativo." },
              }}
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Preço (R$)</FormLabel>
                  <FormControl>
                    <Input type="number" step="0.01" min="0" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <p className="text-sm text-muted-foreground">
              Estoque atual: {produto.quantidadeEmEstoque}{" "}
              <Link href="/movimentacoes/nova" className="underline">
                (registrar movimentação)
              </Link>
            </p>

            <FormRootError />

            <Button
              type="submit"
              disabled={atualizarProdutoMutation.isPending}
              className="mt-2"
            >
              {atualizarProdutoMutation.isPending
                ? "Salvando..."
                : "Salvar alterações"}
            </Button>
          </form>
        </Form>
      </CardContent>
    </Card>
  );
}
