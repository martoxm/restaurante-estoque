export interface SupplierListItem {
  id: string;
  name: string;
  phone?: string | null;
  email?: string | null;
}

export interface SupplierDetails {
  id: string;
  name: string;
  phone?: string | null;
  email?: string | null;
}

export interface CreateSupplierRequest {
  name: string;
  phone?: string;
  email?: string;
}

export interface UpdateSupplierRequest {
  name: string;
  phone?: string;
  email?: string;
}
