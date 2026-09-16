"use client";

import { useForm } from "react-hook-form";
import Link from "next/link";
import { useParams } from "next/navigation";
import { useCategories } from "@/hooks/useCategories";
import { useProduct } from "@/hooks/useProduct";
import { useUpdateProduct } from "@/hooks/useUpdateProduct";
import type { PagedResult } from "@/types/product";
import type { ProductDetails, UpdateProductRequest } from "@/types/product";
import type { CategoryListItem } from "@/types/category";
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

type ProductFormValues = Omit<UpdateProductRequest, "price"> & {
  price: string;
};

export default function EditarProdutoPage() {
  const params = useParams<{ id: string }>();
  const { id } = params;

  const { data: product, isLoading, isError } = useProduct(id);

  const { data: categories, isLoading: isLoadingCategories } = useCategories();

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

      {product && isLoadingCategories && (
        <p className="text-sm text-muted-foreground">
          Carregando categorias...
        </p>
      )}

      {/* Só montamos o formulário quando produto E categorias já chegaram, e
          passamos os dois como props (em vez de usar form.reset() num
          useEffect depois de montar com campos vazios). Isso importa porque
          o <Select> (Radix) só reconhece o valor inicial de categoria se ele
          já vier certo desde o primeiro render — setar o valor DEPOIS que o
          <Select> já montou (via reset) não é confiável: o campo chega a
          ficar com o valor certo por um instante, mas volta a ficar vazio
          logo em seguida (comportamento observado no bug da Etapa 22c). Por
          isso o formulário vira um componente à parte, que só nasce quando
          já tem os dados prontos pra calcular defaultValues correto de
          primeira. */}
      {product && categories && (
        <ProductEditForm id={id} product={product} categories={categories} />
      )}
    </div>
  );
}

function ProductEditForm({
  id,
  product,
  categories,
}: {
  id: string;
  product: ProductDetails;
  categories: PagedResult<CategoryListItem>;
}) {
  const form = useForm<ProductFormValues>({
    defaultValues: {
      name: product.name,
      description: product.description ?? "",
      price: String(product.price),
      categoryId: product.categoryId,
    },
  });

  const updateProductMutation = useUpdateProduct();

  function onSubmit(data: ProductFormValues) {
    updateProductMutation.mutate({
      id,
      data: {
        name: data.name,
        description: data.description || undefined,
        price: Number(data.price),
        categoryId: data.categoryId,
      },
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
                  <Select value={field.value} onValueChange={field.onChange}>
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue placeholder="Selecione uma categoria" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {categories.items.map((category) => (
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

            {/* Quantidade em estoque não é editável aqui — só muda via
                movimentação (Entrada/Saída), ver CreateStockMovementCommand
                no backend. Mostramos só como referência. */}
            <p className="text-sm text-muted-foreground">
              Estoque atual: {product.quantityInStock}{" "}
              <Link href="/movimentacoes/nova" className="underline">
                (registrar movimentação)
              </Link>
            </p>

            {updateProductMutation.isError && (
              <p className="text-sm text-destructive">
                Não foi possível salvar as alterações. Confira os dados e
                tente novamente.
              </p>
            )}

            <Button
              type="submit"
              disabled={updateProductMutation.isPending}
              className="mt-2"
            >
              {updateProductMutation.isPending
                ? "Salvando..."
                : "Salvar alterações"}
            </Button>
          </form>
        </Form>
      </CardContent>
    </Card>
  );
}
