import { useState, useEffect } from "react";
import type { ProductDto, ProductFormData } from "../types";
import { CATEGORIES } from "../data/categories";

interface ProductFormProps {
  product: ProductDto | null;
  onSave: (product: ProductFormData) => void;
  onCancel: () => void;
  isProcessing?: boolean;
}

export const ProductForm = ({
  product,
  onSave,
  onCancel,
  isProcessing = false,
}: ProductFormProps) => {
  const [formData, setFormData] = useState<ProductFormData>({
    productId: 0,
    productName: "",
    categoryId: 0,
    categoryName: "",
    price: "",
    stock: "",
  });

  useEffect(() => {
    if (product) {
      setFormData({
        productId: product.productId,
        productName: product.productName,
        categoryId: product.category.id,
        categoryName: product.category.name,
        price: product.price,
        stock: product.stock,
      });
    } else {
      setFormData({
        productId: 0,
        productName: "",
        categoryId: 0,
        categoryName: "",
        price: "",
        stock: "",
      });
    }
  }, [product]);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>,
  ) => {
    const { name, value } = e.target;

    setFormData((prev) => {
      if (name === "productName" || name === "categoryName") {
        return {
          ...prev,
          [name]: value,
        };
      }

      return {
        ...prev,
        [name]: value === "" ? "" : Number(value),
      };
    });
  };
  const handleCategoryChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const categoryId = Number(e.target.value);
    const category = CATEGORIES.find((c) => c.id === categoryId);
    setFormData((prev) => ({
      ...prev,
      categoryId,
      categoryName: category ? category.name : "",
    }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSave({
      ...formData,
      price: Number(formData.price),
      stock: Number(formData.stock),
    });
  };

  return (
    <form onSubmit={handleSubmit} className="product-form">
      <h2>{product ? "Editar Producto" : "Nuevo Producto"}</h2>

      <div className="form-group">
        <label htmlFor="productName">Nombre</label>
        <input
          id="productName"
          name="productName"
          type="text"
          value={formData.productName}
          onChange={handleChange}
          required
        />
      </div>

      <div className="form-group">
        <label htmlFor="categoryId">Categoría</label>
        <select
          id="categoryId"
          name="categoryId"
          value={formData.categoryId}
          onChange={handleCategoryChange}
          required
        >
          <option value={0}>Seleccionar...</option>
          {CATEGORIES.map((category) => (
            <option key={category.id} value={category.id}>
              {category.name}
            </option>
          ))}
        </select>
      </div>

      <div className="form-row">
        <div className="form-group">
          <label htmlFor="price">Precio</label>
          <input
            id="price"
            name="price"
            type="number"
            step="0.01"
            value={formData.price}
            onChange={handleChange}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="stock">Stock</label>
          <input
            id="stock"
            name="stock"
            type="number"
            value={formData.stock}
            onChange={handleChange}
            required
          />
        </div>
      </div>

      <div className="form-actions">
        <button type="submit" id="add" className="btn-primary" disabled={isProcessing}>
          {isProcessing ? 'Guardando...' : (product ? "Actualizar" : "Crear")}
        </button>
        <button type="button" className="btn-secondary" onClick={onCancel} disabled={isProcessing}>
          Cancelar
        </button>
      </div>
    </form>
  );
};
