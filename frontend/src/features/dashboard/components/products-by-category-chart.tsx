"use client";

import { Pie, PieChart, Cell } from "recharts";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardDescription,
} from "@/components/ui/card";
import {
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
  type ChartConfig,
} from "@/components/ui/chart";
import type { CategoryDistributionItem } from "@/features/dashboard/hooks/useDashboardCharts";

const chartConfig = {
  products: { label: "Produtos" },
} satisfies ChartConfig;

export function ProductsByCategoryChart({
  data,
}: {
  data: CategoryDistributionItem[];
}) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Produtos por categoria</CardTitle>
        <CardDescription>
          Quantos produtos cada categoria tem cadastrado
        </CardDescription>
      </CardHeader>
      <CardContent>
        {data.length === 0 ? (
          <p className="py-10 text-center text-sm text-muted-foreground">
            Sem dados suficientes para montar o gráfico.
          </p>
        ) : (
          <div className="flex flex-col items-center gap-4 sm:flex-row">
            {/* Dentro de um flex row (chart + legenda lado a lado), o
                ResponsiveContainer do Recharts precisa de uma largura
                explícita — sem isso, ele mede "0px de largura disponível"
                e o gráfico simplesmente não desenha nada. */}
            <ChartContainer config={chartConfig} className="h-64 w-64 shrink-0">
              <PieChart>
                <ChartTooltip content={<ChartTooltipContent nameKey="category" />} />
                <Pie
                  data={data}
                  dataKey="products"
                  nameKey="category"
                  innerRadius={50}
                  outerRadius={90}
                  paddingAngle={2}
                >
                  {data.map((entry) => (
                    <Cell key={entry.category} fill={entry.fill} />
                  ))}
                </Pie>
              </PieChart>
            </ChartContainer>

            {/* Legenda ao lado do gráfico — construída direto a partir de
                `data` (não do payload do Recharts) porque cada categoria é
                dinâmica, e a lista deixa claro qual cor é qual categoria sem
                precisar passar o mouse em cima da fatia. */}
            <ul className="flex w-full flex-col gap-2 sm:w-auto">
              {data.map((entry) => (
                <li key={entry.category} className="flex items-center gap-2 text-sm">
                  <span
                    className="size-2.5 shrink-0 rounded-xs"
                    style={{ backgroundColor: entry.fill }}
                  />
                  <span className="text-foreground">{entry.category}</span>
                  <span className="text-muted-foreground">({entry.products})</span>
                </li>
              ))}
            </ul>
          </div>
        )}
      </CardContent>
    </Card>
  );
}
