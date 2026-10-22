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
import type { DistribuicaoCategoriaItem } from "@/features/dashboard/hooks/useDashboardCharts";

const chartConfig = {
  produtos: { label: "Produtos" },
} satisfies ChartConfig;

export function ProdutosPorCategoriaChart({
  data,
}: {
  data: DistribuicaoCategoriaItem[];
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
            {/* Repara que aqui a gente está dentro de um flex row (gráfico +
                legenda lado a lado). Nesse caso o ResponsiveContainer do
                Recharts precisa de uma largura explícita — sem isso ele mede
                "0px de largura disponível" e o gráfico não desenha nada. */}
            <ChartContainer config={chartConfig} className="h-64 w-64 shrink-0">
              <PieChart>
                <ChartTooltip content={<ChartTooltipContent nameKey="categoria" />} />
                <Pie
                  data={data}
                  dataKey="produtos"
                  nameKey="categoria"
                  innerRadius={50}
                  outerRadius={90}
                  paddingAngle={2}
                >
                  {data.map((entry) => (
                    <Cell key={entry.categoria} fill={entry.fill} />
                  ))}
                </Pie>
              </PieChart>
            </ChartContainer>

            {/* Essa é a legenda do lado do gráfico. A gente monta ela direto
                a partir de `data` (e não do payload do Recharts) porque as
                categorias são dinâmicas. Assim a lista deixa claro qual cor é
                qual categoria, sem precisar passar o mouse em cima da fatia. */}
            <ul className="flex w-full flex-col gap-2 sm:w-auto">
              {data.map((entry) => (
                <li key={entry.categoria} className="flex items-center gap-2 text-sm">
                  <span
                    className="size-2.5 shrink-0 rounded-xs"
                    style={{ backgroundColor: entry.fill }}
                  />
                  <span className="text-foreground">{entry.categoria}</span>
                  <span className="text-muted-foreground">({entry.produtos})</span>
                </li>
              ))}
            </ul>
          </div>
        )}
      </CardContent>
    </Card>
  );
}
