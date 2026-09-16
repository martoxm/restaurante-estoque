import { useQuery } from "@tanstack/react-query";
import { getSuppliers } from "@/services/suppliers";

export function useSuppliers() {
  return useQuery({
    queryKey: ["suppliers"],
    queryFn: getSuppliers,
  });
}
