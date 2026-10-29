"use client";

import { useForm } from "react-hook-form";
import Link from "next/link";
import { useCategorias } from "@/features/categorias/hooks/useCategorias";
import { useCriarProduto } from "@/features/produtos/hooks/useCriarProduto";
import type { CriarProdutoRequest } from "@/features/produtos/types/produto";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
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

type ProdutoFormValues = Omit<
  CriarProdutoRequest,
  "preco" | "quantidadeInicial"
> & {
  preco: string;
  quantidadeInicial: string;
};

export function NovoProdutoPage() {
  const form = useForm<ProdutoFormValues>({
    defaultValues: {
      nome: "",
      descricao: "",
      preco: "",
      categoriaId: "",
      quantidadeInicial: "0",
    },
  });

  const { data: categorias, isLoading: isLoadingCategorias } = useCategorias();

  const criarProdutoMutation = useCriarProduto();

  function onSubmit(data: ProdutoFormValues) {
    criarProdutoMutation.mutate({
      nome: data.nome,
      descricao: data.descricao || undefined,
      preco: Number(data.preco),
      categoriaId: data.categoriaId,
      quantidadeInicial: Number(data.quantidadeInicial),
    }, {
      onError: (error) => aplicarErrosDaApi(error, form),
    });
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <h1 className="font-heading text-lg font-semibold">Novo produto</h1>
        <Button variant="outline" asChild>
          <Link href="/produtos">Voltar</Link>
        </Button>
      </div>

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
                    <Select
                      value={field.value}
                      onValueChange={field.onChange}
                      disabled={isLoadingCategorias}
                    >
                      <FormControl>
                        <SelectTrigger className="w-full">
                          <SelectValue placeholder="Selecione uma categoria" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {categorias?.itens.map((categoria) => (
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

              <FormField
                control={form.control}
                name="quantidadeInicial"
                rules={{
                  min: {
                    value: 0,
                    message: "A quantidade não pode ser negativa.",
                  },
                }}
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Quantidade inicial em estoque</FormLabel>
                    <FormControl>
                      <Input type="number" step="1" min="0" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormRootError />

              <Button
                type="submit"
                disabled={criarProdutoMutation.isPending}
                className="mt-2"
              >
                {criarProdutoMutation.isPending
                  ? "Salvando..."
                  : "Salvar produto"}
              </Button>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  );
}
