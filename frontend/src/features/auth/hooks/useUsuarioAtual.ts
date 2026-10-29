"use client";

import { useEffect, useState } from "react";
import { obterToken } from "@/lib/sessao";
import { decodeToken } from "@/lib/jwt";
import { ROLE_CLAIM, type DecodedToken } from "@/features/auth/types/auth";

const ADMIN_ROLE = "Admin";

export function useUsuarioAtual() {
  const [roles, setRoles] = useState<string[]>([]);
  const [restauranteId, setRestauranteId] = useState<string | undefined>();

  useEffect(() => {
    const token = obterToken();
    const decoded = token ? decodeToken<DecodedToken>(token) : null;
    const rawRoles = decoded?.[ROLE_CLAIM] as string | string[] | undefined;

    setRoles(!rawRoles ? [] : Array.isArray(rawRoles) ? rawRoles : [rawRoles]);
    setRestauranteId(decoded?.restauranteId);
  }, []);

  return {
    roles,
    isAdmin: roles.includes(ADMIN_ROLE),
    restauranteId,
  };
}
