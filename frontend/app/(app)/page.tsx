"use client";

import Link from "next/link";
import { Package, Tags, Truck, AlertTriangle } from "lucide-react";
import { useProducts } from "@/hooks/useProducts";
import { useCategories } from "@/hooks/useCategories";
import { useSuppliers } from "@/hooks/useSuppliers";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

const LOW_STOCK_THRESHOLD = 10;

export default function DashboardPage() {
  const { data: products, isLoading: isLoadingProducts } = useProducts({
    pageSize: 100,
  });
  const { data: categories, isLoading: isLoadingCategories } = useCategories();
  const { data: suppliers, isLoading: isLoadingSuppliers } = useSuppliers();

  const lowStockCount =
    products?.items.filter(
      (product) => product.quantityInStock <= LOW_STOCK_THRESHOLD
    ).length ?? 0;

  const cards = [
    {
      label: "Produtos cadastrados",
      value: products?.totalCount,
      isLoading: isLoadingProducts,
      icon: Package,
      href: "/produtos",
    },
    {
      label: "Categorias",
      value: categories?.totalCount,
      isLoading: isLoadingCategories,
      icon: Tags,
      href: "/categorias",
    },
    {
      label: "Fornecedores",
      value: suppliers?.totalCount,
      isLoading: isLoadingSuppliers,
      icon: Truck,
      href: "/fornecedores",
    },
    {
      label: "Produtos com estoque baixo",
      value: lowStockCount,
      isLoading: isLoadingProducts,
      icon: AlertTriangle,
      href: "/produtos",
    },
  ];

  return (
    <div className="flex flex-col gap-4">
      <h1 className="font-heading text-lg font-semibold">Dashboard</h1>

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
    </div>
  );
}
