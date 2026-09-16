"use client";

import { useForm } from "react-hook-form";
import Link from "next/link";
import { useCategories } from "@/hooks/useCategories";
import { useCreateProduct } from "@/hooks/useCreateProduct";
import type { CreateProductRequest } from "@/types/product";
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

type ProductFormValues = Omit<
  CreateProductRequest,
  "price" | "initialQuantity"
> & {
  price: string;
  initialQuantity: string;
};

export default function NovoProdutoPage() {
  const form = useForm<ProductFormValues>({
    defaultValues: {
      name: "",
      description: "",
      price: "",
      categoryId: "",
      initialQuantity: "0",
    },
  });

  const { data: categories, isLoading: isLoadingCategories } = useCategories();

  const createProductMutation = useCreateProduct();

  function onSubmit(data: ProductFormValues) {
    createProductMutation.mutate({
      name: data.name,
      description: data.description || undefined,
      price: Number(data.price),
      categoryId: data.categoryId,
      initialQuantity: Number(data.initialQuantity),
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
                name="name"
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
                name="description"
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
                name="categoryId"
                rules={{ required: "Selecione uma categoria." }}
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Categoria</FormLabel>
                    {/* O componente Select do shadcn/ui não é um <select>
                        nativo, então ele não se conecta ao react-hook-form
                        sozinho como o Input — por isso passamos value/
                        onValueChange manualmente, em vez de {...field}. */}
                    <Select
                      value={field.value}
                      onValueChange={field.onChange}
                      disabled={isLoadingCategories}
                    >
                      <FormControl>
                        <SelectTrigger className="w-full">
                          <SelectValue placeholder="Selecione uma categoria" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {categories?.items.map((category) => (
                          <SelectItem key={category.id} value={category.id}>
                            {category.name}
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
                name="price"
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
                name="initialQuantity"
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

              {createProductMutation.isError && (
                <p className="text-sm text-destructive">
                  Não foi possível criar o produto. Confira os dados e tente
                  novamente.
                </p>
              )}

              <Button
                type="submit"
                disabled={createProductMutation.isPending}
                className="mt-2"
              >
                {createProductMutation.isPending
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
