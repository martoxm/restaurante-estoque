"use client";

import { useForm } from "react-hook-form";
import Link from "next/link";
import { useProducts } from "@/hooks/useProducts";
import { useCreateStockMovement } from "@/hooks/useCreateStockMovement";
import type { StockMovementTypeValue } from "@/types/stock-movement";
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

type StockMovementFormValues = {
  productId: string;
  type: StockMovementTypeValue;
  quantity: string;
  notes: string;
};

export default function NovaMovimentacaoPage() {
  const form = useForm<StockMovementFormValues>({
    defaultValues: {
      productId: "",
      type: "Entrada",
      quantity: "",
      notes: "",
    },
  });

  const { data: products, isLoading: isLoadingProducts } = useProducts({
    pageSize: 100,
  });

  const createStockMovementMutation = useCreateStockMovement();

  function onSubmit(data: StockMovementFormValues) {
    createStockMovementMutation.mutate({
      productId: data.productId,
      type: data.type,
      quantity: Number(data.quantity),
      notes: data.notes || undefined,
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
                name="productId"
                rules={{ required: "Selecione um produto." }}
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Produto</FormLabel>
                    <Select
                      value={field.value}
                      onValueChange={field.onChange}
                      disabled={isLoadingProducts}
                    >
                      <FormControl>
                        <SelectTrigger className="w-full">
                          <SelectValue placeholder="Selecione um produto" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {products?.items.map((product) => (
                          <SelectItem key={product.id} value={product.id}>
                            {product.name} (estoque atual: {product.quantityInStock})
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
                name="type"
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
                name="quantity"
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
                name="notes"
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

              {createStockMovementMutation.isError && (
                <p className="text-sm text-destructive">
                  Não foi possível registrar a movimentação. Confira se a
                  quantidade de saída não é maior que o estoque disponível e
                  tente novamente.
                </p>
              )}

              <Button
                type="submit"
                disabled={createStockMovementMutation.isPending}
                className="mt-2"
              >
                {createStockMovementMutation.isPending
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
