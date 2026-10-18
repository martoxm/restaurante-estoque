"use client";

import { useForm } from "react-hook-form";
import Link from "next/link";
import { useProdutos } from "@/features/produtos/hooks/useProdutos";
import { useCriarMovimentacaoEstoque } from "@/features/movimentacoes-estoque/hooks/useCriarMovimentacaoEstoque";
import type { TipoMovimentacaoEstoque } from "@/features/movimentacoes-estoque/types/movimentacao-estoque";
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

type MovimentacaoEstoqueFormValues = {
  produtoId: string;
  tipo: TipoMovimentacaoEstoque;
  quantidade: string;
  observacoes: string;
};

export function NovaMovimentacaoEstoquePage() {
  const form = useForm<MovimentacaoEstoqueFormValues>({
    defaultValues: {
      produtoId: "",
      tipo: "Entrada",
      quantidade: "",
      observacoes: "",
    },
  });

  const { data: produtos, isLoading: isLoadingProdutos } = useProdutos({
    tamanhoPagina: 100,
  });

  const criarMovimentacaoEstoqueMutation = useCriarMovimentacaoEstoque();

  function onSubmit(data: MovimentacaoEstoqueFormValues) {
    criarMovimentacaoEstoqueMutation.mutate({
      produtoId: data.produtoId,
      tipo: data.tipo,
      quantidade: Number(data.quantidade),
      observacoes: data.observacoes || undefined,
    });
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <h1 className="font-heading text-lg font-semibold">
          Nova movimentação de estoque
        </h1>
        <Button variant="outline" asChild>
          <Link href="/produtos">Voltar</Link>
        </Button>
      </div>

      <Card className="max-w-lg">
        <CardHeader>
          <CardTitle>Dados da movimentação</CardTitle>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form
              onSubmit={form.handleSubmit(onSubmit)}
              className="flex flex-col gap-4"
            >
              <FormField
                control={form.control}
                name="produtoId"
                rules={{ required: "Selecione um produto." }}
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Produto</FormLabel>
                    <Select
                      value={field.value}
                      onValueChange={field.onChange}
                      disabled={isLoadingProdutos}
                    >
                      <FormControl>
                        <SelectTrigger className="w-full">
                          <SelectValue placeholder="Selecione um produto" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {produtos?.itens.map((produto) => (
                          <SelectItem key={produto.id} value={produto.id}>
                            {produto.nome} (estoque atual: {produto.quantidadeEmEstoque})
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
                name="tipo"
                rules={{ required: "Selecione o tipo de movimentação." }}
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Tipo</FormLabel>
                    <Select value={field.value} onValueChange={field.onChange}>
                      <FormControl>
                        <SelectTrigger className="w-full">
                          <SelectValue />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="Entrada">Entrada</SelectItem>
                        <SelectItem value="Saida">Saída</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="quantidade"
                rules={{
                  required: "A quantidade é obrigatória.",
                  min: {
                    value: 1,
                    message: "A quantidade deve ser maior que zero.",
                  },
                }}
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Quantidade</FormLabel>
                    <FormControl>
                      <Input type="number" step="1" min="1" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="observacoes"
                rules={{
                  maxLength: {
                    value: 500,
                    message: "As observações devem ter no máximo 500 caracteres.",
                  },
                }}
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Observações (opcional)</FormLabel>
                    <FormControl>
                      <Input placeholder="Ex: Compra do fornecedor X" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {criarMovimentacaoEstoqueMutation.isError && (
                <p className="text-sm text-destructive">
                  Não foi possível registrar a movimentação. Confira se a
                  quantidade de saída não é maior que o estoque disponível e
                  tente novamente.
                </p>
              )}

              <Button
                type="submit"
                disabled={criarMovimentacaoEstoqueMutation.isPending}
                className="mt-2"
              >
                {criarMovimentacaoEstoqueMutation.isPending
                  ? "Salvando..."
                  : "Registrar movimentação"}
              </Button>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  );
}
