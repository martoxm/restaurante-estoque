"use client";

import { useRouter } from "next/navigation";
import Cookies from "js-cookie";
import { LogOut } from "lucide-react";
import { Button } from "@/components/ui/button";

export function Header() {
  const router = useRouter();

  function handleLogout() {
    Cookies.remove("token", { path: "/" });
    router.push("/login");
  }

  return (
    <header className="flex h-14 items-center justify-between border-b bg-background px-4">
      <span className="font-heading text-sm font-semibold">
        Controle de Estoque
      </span>

      <Button variant="outline" size="sm" onClick={handleLogout}>
        <LogOut className="size-4" />
        Sair
      </Button>
    </header>
  );
}
