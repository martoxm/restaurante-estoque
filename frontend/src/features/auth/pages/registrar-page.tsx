"use client";

import { useForm } from "react-hook-form";
import Link from "next/link";
import { useRegistrar } from "@/features/auth/hooks/useRegistrar";
import type { RegistrarRequest } from "@/features/auth/types/auth";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Card,
  CardContent,
  CardDescription,
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

export function RegistrarPage() {
  const form = useForm<RegistrarRequest>({
    defaultValues: { nome: "", email: "", senha: "", nomeRestaurante: "" },
  });

  const registrarMutation = useRegistrar();

  function onSubmit(data: RegistrarRequest) {
    registrarMutation.mutate(data);
  }

  return (
    <div className="flex flex-1 items-center justify-center bg-zinc-50 p-4 dark:bg-black">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle>Criar conta</CardTitle>
          <CardDescription>
            Informe seus dados para criar uma conta no sistema.
          </CardDescription>
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
                rules={{ required: "O nome é obrigatório." }}
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Nome</FormLabel>
                    <FormControl>
                      <Input
                        type="text"
                        placeholder="Seu nome completo"
                        autoComplete="name"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="email"
                rules={{
                  required: "O e-mail é obrigatório.",
                  pattern: {
                    value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                    message: "O e-mail informado não é válido.",
                  },
                }}
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>E-mail</FormLabel>
                    <FormControl>
                      <Input
                        type="email"
                        placeholder="seu@email.com"
                        autoComplete="email"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="senha"
                rules={{ required: "A senha é obrigatória." }}
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Senha</FormLabel>
                    <FormControl>
                      <Input
                        type="password"
                        placeholder="••••••••"
                        autoComplete="new-password"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="nomeRestaurante"
                rules={{ required: "O nome do restaurante é obrigatório." }}
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Nome do restaurante</FormLabel>
                    <FormControl>
                      <Input
                        type="text"
                        placeholder="Ex: Restaurante do Gabriel"
                        autoComplete="organization"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {registrarMutation.isError && (
                <p className="text-sm text-destructive">
                  Não foi possível criar a conta. Verifique os dados
                  informados (o e-mail pode já estar em uso).
                </p>
              )}

              <Button
                type="submit"
                disabled={registrarMutation.isPending}
                className="mt-2"
              >
                {registrarMutation.isPending ? "Criando conta..." : "Criar conta"}
              </Button>

              <p className="text-center text-sm text-muted-foreground">
                Já tem conta?{" "}
                <Link href="/login" className="underline">
                  Entrar
                </Link>
              </p>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  );
}
