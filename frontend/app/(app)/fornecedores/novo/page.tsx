"use client";

import { useForm } from "react-hook-form";
import Link from "next/link";
import { useCreateSupplier } from "@/hooks/useCreateSupplier";
import type { CreateSupplierRequest } from "@/types/supplier";
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

export default function NovoFornecedorPage() {
  const form = useForm<CreateSupplierRequest>({
    defaultValues: { name: "", phone: "", email: "" },
  });

  const createSupplierMutation = useCreateSupplier();

  function onSubmit(data: CreateSupplierRequest) {
    createSupplierMutation.mutate({
      name: data.name,
      phone: data.phone || undefined,
      email: data.email || undefined,
    });
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <h1 className="font-heading text-lg font-semibold">Novo fornecedor</h1>
        <Button variant="outline" asChild>
          <Link href="/fornecedores">Voltar</Link>
        </Button>
      </div>

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
                      <Input placeholder="Ex: Distribuidora Central" {...field} />
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
                      <Input placeholder="Ex: contato@fornecedor.com" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {createSupplierMutation.isError && (
                <p className="text-sm text-destructive">
                  Não foi possível criar o fornecedor. Confira os dados e
                  tente novamente.
                </p>
              )}

              <Button
                type="submit"
                disabled={createSupplierMutation.isPending}
                className="mt-2"
              >
                {createSupplierMutation.isPending
                  ? "Salvando..."
                  : "Salvar fornecedor"}
              </Button>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  );
}
