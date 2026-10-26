"use client";

import { useEffect } from "react";
import { useForm } from "react-hook-form";
import Link from "next/link";
import { useParams } from "next/navigation";
import { useCategoria } from "@/features/categorias/hooks/useCategoria";
import { useAtualizarCategoria } from "@/features/categorias/hooks/useAtualizarCategoria";
import type { AtualizarCategoriaRequest } from "@/features/categorias/types/categoria";
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
import { FormRootError } from "@/components/form-root-error";
import { aplicarErrosDaApi } from "@/lib/api-errors";

export function EditarCategoriaPage() {
  const params = useParams<{ id: string }>();
  const { id } = params;

  const { data: categoria, isLoading, isError } = useCategoria(id);

  const form = useForm<AtualizarCategoriaRequest>({
    defaultValues: { nome: "", descricao: "" },
  });

  useEffect(() => {
    if (categoria) {
      form.reset({
        nome: categoria.nome,
        descricao: categoria.descricao ?? "",
      });
    }
  }, [categoria, form]);

  const atualizarCategoriaMutation = useAtualizarCategoria();

  function onSubmit(data: AtualizarCategoriaRequest) {
    atualizarCategoriaMutation.mutate({
      id,
      data: {
        nome: data.nome,
        descricao: data.descricao || undefined,
      },
    }, {
      onError: (error) => aplicarErrosDaApi(error, form),
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

      {categoria && (
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
                  name="nome"
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
                        <Input
                          placeholder="Ex: Refrigerantes, sucos e água"
                          {...field}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormRootError />

                <Button
                  type="submit"
                  disabled={atualizarCategoriaMutation.isPending}
                  className="mt-2"
                >
                  {atualizarCategoriaMutation.isPending
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
