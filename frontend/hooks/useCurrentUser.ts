"use client";

import { useEffect, useState } from "react";
import Cookies from "js-cookie";
import { decodeToken } from "@/lib/jwt";
import { ROLE_CLAIM, type DecodedToken } from "@/types/auth";

const ADMIN_ROLE = "Admin";

export function useCurrentUser() {
  const [roles, setRoles] = useState<string[]>([]);

  useEffect(() => {
    const token = Cookies.get("token");
    const decoded = token ? decodeToken<DecodedToken>(token) : null;
    const rawRoles = decoded?.[ROLE_CLAIM] as string | string[] | undefined;

    setRoles(!rawRoles ? [] : Array.isArray(rawRoles) ? rawRoles : [rawRoles]);
  }, []);

  return {
    roles,
    isAdmin: roles.includes(ADMIN_ROLE),
  };
}
