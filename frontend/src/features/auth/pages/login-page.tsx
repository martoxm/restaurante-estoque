"use client";

import { useForm } from "react-hook-form";
import Link from "next/link";
import { useLogin } from "@/features/auth/hooks/useLogin";
import type { LoginRequest } from "@/features/auth/types/auth";
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

export function LoginPage() {
  const form = useForm<LoginRequest>({
    defaultValues: { email: "", senha: "" },
  });

  const loginMutation = useLogin();

  function onSubmit(data: LoginRequest) {
    loginMutation.mutate(data);
  }

  return (
    <div className="flex flex-1 items-center justify-center bg-zinc-50 p-4 dark:bg-black">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle>Entrar</CardTitle>
          <CardDescription>
            Informe seu e-mail e senha para acessar o sistema.
          </CardDescription>
        </CardHeader>
        <CardContent>
          {/* Esse <Form {...form}> é o FormProvider do react-hook-form. Ele
              "espalha" o form pros FormField de dentro, assim a gente não
              precisa passar form.control em cada um na mão. */}
          <Form {...form}>
            <form
              onSubmit={form.handleSubmit(onSubmit)}
              className="flex flex-col gap-4"
            >
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
                        autoComplete="username"
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
                        autoComplete="current-password"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {loginMutation.isError && (
                <p className="text-sm text-destructive">
                  E-mail ou senha inválidos.
                </p>
              )}

              <Button
                type="submit"
                disabled={loginMutation.isPending}
                className="mt-2"
              >
                {loginMutation.isPending ? "Entrando..." : "Entrar"}
              </Button>

              <p className="text-center text-sm text-muted-foreground">
                Não tem conta?{" "}
                <Link href="/registrar" className="underline">
                  Criar conta
                </Link>
              </p>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  );
}
