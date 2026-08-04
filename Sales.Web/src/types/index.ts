export const Filter = {
  BETWEN: 0,
  LESS_THAN: 1,
  MORE_THAN: 2,
} as const;

export type FilterValue = (typeof Filter)[keyof typeof Filter];

export interface CategoryDto {
  id: number;
  name: string;
}

export interface ProductDto {
  productId: number;
  productName: string;
  category: CategoryDto;
  price: number;
  stock: number;
}

export interface ApiResponse<T> {
  statusCode: number;
  message: string;
  isSuccess: boolean;
  data: T;
}

export interface ProductFilters {
  filter: FilterValue;
  price: number;
  minPrice: number;
  maxPrice: number;
  length: number;
  category: string;
}

export interface ProductFormData {
  productId: number;
  productName: string;
  categoryId: number;
  categoryName: string;
  price: number;
  stock: number;
}
