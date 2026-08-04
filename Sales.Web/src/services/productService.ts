import type {
  ApiResponse,
  ProductDto,
  ProductFilters,
  ProductFormData,
} from '../types';

const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5138').replace(/\/$/, '');

const buildQueryString = (params: Record<string, string | number>) => {
  const searchParams = new URLSearchParams();
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') {
      searchParams.append(key, String(value));
    }
  });
  return searchParams.toString();
};

export const getProducts = async (
  filters: ProductFilters,
): Promise<ProductDto[]> => {
  const query = buildQueryString({
    filter: filters.filter,
    minPrice: filters.minPrice,
    maxPrice: filters.maxPrice,
    price: filters.price,
    length: filters.length,
    category: filters.category,
  });

  const response = await fetch(`${API_BASE_URL}/api/v1/product/all?${query}`);
  if (!response.ok) {
    throw new Error(`Error fetching products: ${response.statusText}`);
  }

  const result: ApiResponse<ProductDto[]> = await response.json();
  return result.data;
};

export const createProduct = async (
  product: ProductFormData,
): Promise<string> => {
  const response = await fetch(`${API_BASE_URL}/api/v1/product/create`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      productId: product.productId,
      productName: product.productName,
      category: {
        id: product.categoryId,
        name: product.categoryName,
      },
      price: product.price,
      stock: product.stock,
    }),
  });

  if (!response.ok) {
    throw new Error(`Error creating product: ${response.statusText}`);
  }

  const result: ApiResponse<string> = await response.json();
  return result.data;
};

export const updateProduct = async (
  product: ProductFormData,
): Promise<ProductDto> => {
  const response = await fetch(`${API_BASE_URL}/api/v1/product/update`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      productId: product.productId,
      productName: product.productName,
      category: {
        id: product.categoryId,
        name: product.categoryName,
      },
      price: product.price,
      stock: product.stock,
    }),
  });

  if (!response.ok) {
    throw new Error(`Error updating product: ${response.statusText}`);
  }

  const result: ApiResponse<ProductDto> = await response.json();
  return result.data;
};

export const deleteProduct = async (id: number): Promise<ProductDto> => {
  const response = await fetch(
    `${API_BASE_URL}/api/v1/product/removeby?id=${id}`,
    {
      method: 'DELETE',
    },
  );

  if (!response.ok) {
    throw new Error(`Error deleting product: ${response.statusText}`);
  }

  const result: ApiResponse<ProductDto> = await response.json();
  return result.data;
};
