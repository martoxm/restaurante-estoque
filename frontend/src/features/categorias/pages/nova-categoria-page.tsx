"use client";

import { useForm } from "react-hook-form";
import Link from "next/link";
import { useCriarCategoria } from "@/features/categorias/hooks/useCriarCategoria";
import type { CriarCategoriaRequest } from "@/features/categorias/types/categoria";
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

export function NovaCategoriaPage() {
  const form = useForm<CriarCategoriaRequest>({
    defaultValues: { nome: "", descricao: "" },
  });

  const criarCategoriaMutation = useCriarCategoria();

  function onSubmit(data: CriarCategoriaRequest) {
    criarCategoriaMutation.mutate({
      nome: data.nome,
      descricao: data.descricao || undefined,
    }, {
      onError: (error) => aplicarErrosDaApi(error, form),
    });
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <h1 className="font-heading text-lg font-semibold">Nova categoria</h1>
        <Button variant="outline" asChild>
          <Link href="/categorias">Voltar</Link>
        </Button>
      </div>

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
                      <Input placeholder="Ex: Refrigerantes, sucos e água" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormRootError />

              <Button
                type="submit"
                disabled={criarCategoriaMutation.isPending}
                className="mt-2"
              >
                {criarCategoriaMutation.isPending
                  ? "Salvando..."
                  : "Salvar categoria"}
              </Button>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  );
}
