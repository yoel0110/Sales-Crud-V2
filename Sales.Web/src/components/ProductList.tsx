import type { ProductDto } from '../types';

interface ProductListProps {
  products: ProductDto[];
  onEdit: (product: ProductDto) => void;
  onDelete: (id: number) => void;
}

export const ProductList = ({ products, onEdit, onDelete }: ProductListProps) => {
  if (products.length === 0) {
    return <p className="empty">No se encontraron productos.</p>;
  }

  return (
    <table className="product-table">
      <thead>
        <tr>
          <th>Id</th>
          <th>Nombre</th>
          <th>Categoría</th>
          <th>Precio</th>
          <th>Stock</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        {products.map((product) => (
          <tr key={product.productId}>
            <td>{product.productId}</td>
            <td>{product.productName}</td>
            <td>{product.category.name}</td>
            <td>{product.price.toFixed(2)}</td>
            <td>{product.stock}</td>
            <td>
              <div className="row-actions">
                <button
                  type="button"
                  className="btn-edit"
                  onClick={() => onEdit(product)}
                >
                  Editar
                </button>
                <button
                  type="button"
                  className="btn-delete"
                  onClick={() => onDelete(product.productId)}
                >
                  Eliminar
                </button>
              </div>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
};
