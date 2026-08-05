import type {
  ApiResponse,
  ProductDto,
  ProductFilters,
  ProductFormData,
  ServiceResult,
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

const parseBackendError = async (response: Response): Promise<string> => {
  try {
    const result: ApiResponse<unknown> = await response.json();
    return result.message || response.statusText;
  } catch {
    return response.statusText;
  }
};

export const getProducts = async (
  filters: ProductFilters,
): Promise<ServiceResult<ProductDto[]>> => {
  const query = buildQueryString({
    filter: filters.filter,
    minPrice: filters.minPrice,
    maxPrice: filters.maxPrice,
    price: filters.price,
    length: filters.length,
    category: filters.category,
  });

  const response = await fetch(`${API_BASE_URL}/api/v1/product/all?${query}`, {
    credentials: 'include',
  });
  if (!response.ok) {
    const message = await parseBackendError(response);
    return { isSuccess: false, message, data: [] };
  }

  const result: ApiResponse<ProductDto[]> = await response.json();
  return { isSuccess: result.isSuccess, message: result.message, data: result.data };
};

export const createProduct = async (
  product: ProductFormData,
): Promise<ServiceResult<string>> => {
  const response = await fetch(`${API_BASE_URL}/api/v1/product/create`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
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
    const message = await parseBackendError(response);
    return { isSuccess: false, message, data: '' };
  }

  const result: ApiResponse<string> = await response.json();
  return { isSuccess: result.isSuccess, message: result.message, data: result.data };
};

export const updateProduct = async (
  product: ProductFormData,
): Promise<ServiceResult<ProductDto>> => {
  const response = await fetch(`${API_BASE_URL}/api/v1/product/update`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
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
    const message = await parseBackendError(response);
    return { isSuccess: false, message, data: {} as ProductDto };
  }

  const result: ApiResponse<ProductDto> = await response.json();
  return { isSuccess: result.isSuccess, message: result.message, data: result.data };
};

export const deleteProduct = async (id: number): Promise<ServiceResult<ProductDto>> => {
  const response = await fetch(
    `${API_BASE_URL}/api/v1/product/removeby?id=${id}`,
    {
      method: 'DELETE',
      credentials: 'include',
    },
  );

  if (!response.ok) {
    const message = await parseBackendError(response);
    return { isSuccess: false, message, data: {} as ProductDto };
  }

  const result: ApiResponse<ProductDto> = await response.json();
  return { isSuccess: result.isSuccess, message: result.message, data: result.data };
};
