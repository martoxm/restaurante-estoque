"use client";

import { useEffect } from "react";
import { useForm } from "react-hook-form";
import Link from "next/link";
import { useParams } from "next/navigation";
import { useFornecedor } from "@/features/fornecedores/hooks/useFornecedor";
import { useAtualizarFornecedor } from "@/features/fornecedores/hooks/useAtualizarFornecedor";
import type { AtualizarFornecedorRequest } from "@/features/fornecedores/types/fornecedor";
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

export function EditarFornecedorPage() {
  const params = useParams<{ id: string }>();
  const { id } = params;

  const { data: fornecedor, isLoading, isError } = useFornecedor(id);

  const form = useForm<AtualizarFornecedorRequest>({
    defaultValues: { nome: "", telefone: "", email: "" },
  });

  useEffect(() => {
    if (fornecedor) {
      form.reset({
        nome: fornecedor.nome,
        telefone: fornecedor.telefone ?? "",
        email: fornecedor.email ?? "",
      });
    }
  }, [fornecedor, form]);

  const atualizarFornecedorMutation = useAtualizarFornecedor();

  function onSubmit(data: AtualizarFornecedorRequest) {
    atualizarFornecedorMutation.mutate({
      id,
      data: {
        nome: data.nome,
        telefone: data.telefone || undefined,
        email: data.email || undefined,
      },
    }, {
      onError: (error) => aplicarErrosDaApi(error, form),
    });
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <h1 className="font-heading text-lg font-semibold">
          Editar fornecedor
        </h1>
        <Button variant="outline" asChild>
          <Link href="/fornecedores">Voltar</Link>
        </Button>
      </div>

      {isLoading && (
        <p className="text-sm text-muted-foreground">
          Carregando fornecedor...
        </p>
      )}

      {isError && (
        <p className="text-sm text-destructive">
          Não foi possível carregar esse fornecedor.
        </p>
      )}

      {fornecedor && (
        <Card className="max-w-lg">
          <CardHeader>
            <CardTitle>Dados do fornecedor</CardTitle>
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
                    required: "O nome do fornecedor é obrigatório.",
                    maxLength: {
                      value: 150,
                      message: "O nome deve ter no máximo 150 caracteres.",
                    },
                  }}
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Nome</FormLabel>
                      <FormControl>
                        <Input
                          placeholder="Ex: Distribuidora Central"
                          {...field}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="telefone"
                  rules={{
                    maxLength: {
                      value: 20,
                      message: "O telefone deve ter no máximo 20 caracteres.",
                    },
                  }}
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Telefone (opcional)</FormLabel>
                      <FormControl>
                        <Input placeholder="Ex: (11) 91234-5678" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="email"
                  rules={{
                    maxLength: {
                      value: 200,
                      message: "O e-mail deve ter no máximo 200 caracteres.",
                    },
                    pattern: {
                      value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                      message: "Informe um e-mail válido.",
                    },
                  }}
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>E-mail (opcional)</FormLabel>
                      <FormControl>
                        <Input
                          placeholder="Ex: contato@fornecedor.com"
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
                  disabled={atualizarFornecedorMutation.isPending}
                  className="mt-2"
                >
                  {atualizarFornecedorMutation.isPending
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
