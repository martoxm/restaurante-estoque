"use client";

import { Bar, BarChart, CartesianGrid, Cell, XAxis, YAxis } from "recharts";
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
import type { StockByCategoryItem } from "@/features/dashboard/hooks/useDashboardCharts";

const chartConfig = {
  quantity: { label: "Unidades em estoque", color: "var(--chart-1)" },
} satisfies ChartConfig;

export function StockByCategoryChart({
  data,
}: {
  data: StockByCategoryItem[];
}) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Estoque por categoria</CardTitle>
        <CardDescription>
          Soma da quantidade em estoque dos produtos de cada categoria
        </CardDescription>
      </CardHeader>
      <CardContent>
        {data.length === 0 ? (
          <p className="py-10 text-center text-sm text-muted-foreground">
            Sem dados suficientes para montar o gráfico.
          </p>
        ) : (
          <ChartContainer config={chartConfig} className="max-h-64 w-full">
            <BarChart data={data}>
              <CartesianGrid vertical={false} />
              <XAxis
                dataKey="category"
                tickLine={false}
                axisLine={false}
                tickMargin={8}
              />
              <YAxis tickLine={false} axisLine={false} width={32} />
              <ChartTooltip content={<ChartTooltipContent />} />
              <Bar dataKey="quantity" radius={4}>
                {data.map((entry) => (
                  <Cell key={entry.category} fill={entry.fill} />
                ))}
              </Bar>
            </BarChart>
          </ChartContainer>
        )}
      </CardContent>
    </Card>
  );
}
