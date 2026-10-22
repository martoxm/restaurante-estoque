import { defineConfig, globalIgnores } from "eslint/config";
import nextVitals from "eslint-config-next/core-web-vitals";
import nextTs from "eslint-config-next/typescript";

const eslintConfig = defineConfig([
  ...nextVitals,
  ...nextTs,
  // Aqui a gente sobrescreve a lista padrão de arquivos ignorados do eslint-config-next.
  globalIgnores([
    // Esses são os mesmos que o eslint-config-next já ignorava por padrão:
    ".next/**",
    "out/**",
    "build/**",
    "next-env.d.ts",
  ]),
]);

export default eslintConfig;
