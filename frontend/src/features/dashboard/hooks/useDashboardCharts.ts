import { useMemo } from "react";
import { useProducts } from "@/features/products/hooks/useProducts";
import { useCategories } from "@/features/categories/hooks/useCategories";

export interface CategoryDistributionItem {
  category: string;
  products: number;
  fill: string;
}

export interface StockByCategoryItem {
  category: string;
  quantity: number;
  fill: string;
}

export function useDashboardCharts() {
  const {
    data: products,
    isLoading: isLoadingProducts,
    isError: isErrorProducts,
  } = useProducts({ pageSize: 100 });
  const {
    data: categories,
    isLoading: isLoadingCategories,
    isError: isErrorCategories,
  } = useCategories();

  const categoryDistribution = useMemo<CategoryDistributionItem[]>(() => {
    if (!categories) return [];

    return categories.items
      .filter((category) => category.productCount > 0)
      .map((category, index) => ({
        category: category.name,
        products: category.productCount,
        fill: `var(--chart-${(index % 5) + 1})`,
      }));
  }, [categories]);

  const stockByCategory = useMemo<StockByCategoryItem[]>(() => {
    if (!products) return [];

    const totals = new Map<string, number>();
    for (const product of products.items) {
      const current = totals.get(product.categoryName) ?? 0;
      totals.set(product.categoryName, current + product.quantityInStock);
    }

    return [...totals.entries()].map(([category, quantity], index) => ({
      category,
      quantity,
      fill: `var(--chart-${(index % 5) + 1})`,
    }));
  }, [products]);

  return {
    categoryDistribution,
    stockByCategory,
    isLoading: isLoadingProducts || isLoadingCategories,
    isError: isErrorProducts || isErrorCategories,
  };
}
