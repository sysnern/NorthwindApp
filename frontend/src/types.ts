export interface Product {
  productId: number;
  productName: string;
  supplierId: number;
  categoryId: number;
  unitPrice: number;
  unitsInStock: number;
  discontinued: boolean;
}

export interface Category {
  categoryId: number;
  categoryName: string;
}

export interface Supplier {
  supplierId: number;
  companyName: string;
}
