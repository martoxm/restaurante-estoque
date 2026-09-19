"use client";

import { useEffect, useState } from "react";
import Cookies from "js-cookie";
import { decodeToken } from "@/lib/jwt";
import { ROLE_CLAIM, type DecodedToken } from "@/features/auth/types/auth";

const ADMIN_ROLE = "Admin";

export function useCurrentUser() {
  const [roles, setRoles] = useState<string[]>([]);
  const [restaurantId, setRestaurantId] = useState<string | undefined>();

  useEffect(() => {
    const token = Cookies.get("token");
    const decoded = token ? decodeToken<DecodedToken>(token) : null;
    const rawRoles = decoded?.[ROLE_CLAIM] as string | string[] | undefined;

    setRoles(!rawRoles ? [] : Array.isArray(rawRoles) ? rawRoles : [rawRoles]);
    setRestaurantId(decoded?.restaurantId);
  }, []);

  return {
    roles,
    isAdmin: roles.includes(ADMIN_ROLE),
    restaurantId,
  };
}
