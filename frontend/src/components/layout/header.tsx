"use client";

import { useRouter } from "next/navigation";
import { limparSessao } from "@/lib/sessao";
import { LogOut, Warehouse } from "lucide-react";
import { Button } from "@/components/ui/button";
import { MobileNav } from "@/components/layout/mobile-nav";
import { ThemeToggle } from "@/components/layout/theme-toggle";

export function Header() {
  const router = useRouter();

  function handleLogout() {
    limparSessao();
    router.push("/login");
  }

  return (
    <header className="sticky top-0 z-10 flex h-14 items-center justify-between border-b bg-background/95 px-4 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="flex items-center gap-2">
        <MobileNav />
        <Warehouse className="size-5 text-primary" />
        <span className="font-heading text-sm font-semibold">
          Controle de Estoque
        </span>
      </div>

      <div className="flex items-center gap-2">
        <ThemeToggle />
        <Button variant="outline" size="sm" onClick={handleLogout}>
          <LogOut className="size-4" />
          Sair
        </Button>
      </div>
    </header>
  );
}
