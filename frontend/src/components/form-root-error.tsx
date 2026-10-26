"use client";

import { useFormState } from "react-hook-form";

export function FormRootError() {
  const { errors } = useFormState();
  const mensagem = errors.root?.server?.message;

  if (!mensagem) return null;

  return (
    <p role="alert" className="text-sm text-destructive">
      {mensagem}
    </p>
  );
}
