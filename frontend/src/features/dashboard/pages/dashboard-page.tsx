"use client";

import Link from "next/link";
import { Package, Tags, Truck, AlertTriangle } from "lucide-react";
import { useProdutos } from "@/features/produtos/hooks/useProdutos";
import { useCategorias } from "@/features/categorias/hooks/useCategorias";
import { useFornecedores } from "@/features/fornecedores/hooks/useFornecedores";
import { useUsuarioAtual } from "@/features/auth/hooks/useUsuarioAtual";
import { useRestaurante } from "@/features/restaurantes/hooks/useRestaurante";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useDashboardCharts } from "@/features/dashboard/hooks/useDashboardCharts";
import { ProdutosPorCategoriaChart } from "@/features/dashboard/components/produtos-por-categoria-chart";
import { EstoquePorCategoriaChart } from "@/features/dashboard/components/estoque-por-categoria-chart";

const LIMITE_ESTOQUE_BAIXO = 10;

export function DashboardPage() {
  const { data: produtos, isLoading: isLoadingProdutos } = useProdutos({
    tamanhoPagina: 100,
  });
  const { data: categorias, isLoading: isLoadingCategorias } = useCategorias();
  const { data: fornecedores, isLoading: isLoadingFornecedores } = useFornecedores();

  const { restauranteId } = useUsuarioAtual();
  const { data: restaurante, isLoading: isLoadingRestaurante } =
    useRestaurante(restauranteId);

  const { distribuicaoCategorias, estoquePorCategoria } = useDashboardCharts();

  const quantidadeEstoqueBaixo =
    produtos?.itens.filter(
      (produto) => produto.quantidadeEmEstoque < LIMITE_ESTOQUE_BAIXO
    ).length ?? 0;

  const cards = [
    {
      label: "Produtos cadastrados",
      value: produtos?.totalItens,
      isLoading: isLoadingProdutos,
      icon: Package,
      href: "/produtos",
    },
    {
      label: "Categorias",
      value: categorias?.totalItens,
      isLoading: isLoadingCategorias,
      icon: Tags,
      href: "/categorias",
    },
    {
      label: "Fornecedores",
      value: fornecedores?.totalItens,
      isLoading: isLoadingFornecedores,
      icon: Truck,
      href: "/fornecedores",
    },
    {
      label: "Produtos com estoque baixo",
      value: quantidadeEstoqueBaixo,
      isLoading: isLoadingProdutos,
      icon: AlertTriangle,
      href: "/produtos",
    },
  ];

  return (
    <div className="flex flex-col gap-4">
      <div>
        <h1 className="font-heading text-lg font-semibold">Dashboard</h1>
        <p className="text-sm text-muted-foreground">
          {isLoadingRestaurante
            ? "Carregando restaurante..."
            : restaurante
              ? `Restaurante: ${restaurante.nome}`
              : null}
        </p>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {cards.map(({ label, value, isLoading, icon: Icon, href }) => {
          const card = (
            <Card>
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                  {label}
                </CardTitle>
                <Icon className="size-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <p className="text-2xl font-semibold">
                  {isLoading ? "..." : (value ?? "—")}
                </p>
              </CardContent>
            </Card>
          );

          return href ? (
            <Link key={label} href={href} className="block">
              {card}
            </Link>
          ) : (
            <div key={label}>{card}</div>
          );
        })}
      </div>

      <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
        <ProdutosPorCategoriaChart data={distribuicaoCategorias} />
        <EstoquePorCategoriaChart data={estoquePorCategoria} />
      </div>
    </div>
  );
}
