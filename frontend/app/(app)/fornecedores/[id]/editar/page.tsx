"use client";

import { useEffect } from "react";
import { useForm } from "react-hook-form";
import Link from "next/link";
import { useParams } from "next/navigation";
import { useSupplier } from "@/hooks/useSupplier";
import { useUpdateSupplier } from "@/hooks/useUpdateSupplier";
import type { UpdateSupplierRequest } from "@/types/supplier";
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

export default function EditarFornecedorPage() {
  const params = useParams<{ id: string }>();
  const { id } = params;

  const { data: supplier, isLoading, isError } = useSupplier(id);

  const form = useForm<UpdateSupplierRequest>({
    defaultValues: { name: "", phone: "", email: "" },
  });

  useEffect(() => {
    if (supplier) {
      form.reset({
        name: supplier.name,
        phone: supplier.phone ?? "",
        email: supplier.email ?? "",
      });
    }
  }, [supplier, form]);

  const updateSupplierMutation = useUpdateSupplier();

  function onSubmit(data: UpdateSupplierRequest) {
    updateSupplierMutation.mutate({
      id,
      data: {
        name: data.name,
        phone: data.phone || undefined,
        email: data.email || undefined,
      },
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

      {supplier && (
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
                  name="name"
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
                  name="phone"
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

                {updateSupplierMutation.isError && (
                  <p className="text-sm text-destructive">
                    Não foi possível salvar as alterações. Confira os dados e
                    tente novamente.
                  </p>
                )}

                <Button
                  type="submit"
                  disabled={updateSupplierMutation.isPending}
                  className="mt-2"
                >
                  {updateSupplierMutation.isPending
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
