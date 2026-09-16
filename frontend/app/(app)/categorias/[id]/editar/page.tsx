"use client";

import { useEffect } from "react";
import { useForm } from "react-hook-form";
import Link from "next/link";
import { useParams } from "next/navigation";
import { useCategory } from "@/hooks/useCategory";
import { useUpdateCategory } from "@/hooks/useUpdateCategory";
import type { UpdateCategoryRequest } from "@/types/category";
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

export default function EditarCategoriaPage() {
  const params = useParams<{ id: string }>();
  const { id } = params;

  const { data: category, isLoading, isError } = useCategory(id);

  const form = useForm<UpdateCategoryRequest>({
    defaultValues: { name: "", description: "" },
  });

  useEffect(() => {
    if (category) {
      form.reset({
        name: category.name,
        description: category.description ?? "",
      });
    }
  }, [category, form]);

  const updateCategoryMutation = useUpdateCategory();

  function onSubmit(data: UpdateCategoryRequest) {
    updateCategoryMutation.mutate({
      id,
      data: {
        name: data.name,
        description: data.description || undefined,
      },
    });
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <h1 className="font-heading text-lg font-semibold">Editar categoria</h1>
        <Button variant="outline" asChild>
          <Link href="/categorias">Voltar</Link>
        </Button>
      </div>

      {isLoading && (
        <p className="text-sm text-muted-foreground">Carregando categoria...</p>
      )}

      {isError && (
        <p className="text-sm text-destructive">
          Não foi possível carregar essa categoria.
        </p>
      )}

      {category && (
        <Card className="max-w-lg">
          <CardHeader>
            <CardTitle>Dados da categoria</CardTitle>
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
                    required: "O nome da categoria é obrigatório.",
                    maxLength: {
                      value: 100,
                      message: "O nome deve ter no máximo 100 caracteres.",
                    },
                  }}
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Nome</FormLabel>
                      <FormControl>
                        <Input placeholder="Ex: Bebidas" {...field} />
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
                        <Input
                          placeholder="Ex: Refrigerantes, sucos e água"
                          {...field}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                {updateCategoryMutation.isError && (
                  <p className="text-sm text-destructive">
                    Não foi possível salvar as alterações. Confira os dados e
                    tente novamente.
                  </p>
                )}

                <Button
                  type="submit"
                  disabled={updateCategoryMutation.isPending}
                  className="mt-2"
                >
                  {updateCategoryMutation.isPending
                    ? "Salvando..."
                    : "Salvar alterações"}
                </Button>
              </form>
            </Form>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
