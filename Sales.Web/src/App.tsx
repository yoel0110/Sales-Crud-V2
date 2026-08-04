import { useState, useEffect, useCallback } from 'react'
import { ProductForm } from './components/ProductForm'
import { ProductList } from './components/ProductList'
import { Filter, type ProductDto, type ProductFilters, type ProductFormData } from './types'
import { createProduct, deleteProduct, getProducts, updateProduct } from './services/productService'
import { CATEGORIES } from './data/categories'
import './App.css'

function App() {
  const [products, setProducts] = useState<ProductDto[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [editingProduct, setEditingProduct] = useState<ProductDto | null>(null)
  const [showForm, setShowForm] = useState(false)
  const [filters, setFilters] = useState<ProductFilters>({
    filter: Filter.LESS_THAN,
    price: 100,
    minPrice: 0,
    maxPrice: 1000,
    length: 10,
    category: 'Toys',
  })

  const loadProducts = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const data = await getProducts(filters)
      setProducts(data)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error desconocido')
    } finally {
      setLoading(false)
    }
  }, [filters])

  useEffect(() => {
    loadProducts()
  }, [loadProducts])

  const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target
    setFilters((prev) => ({
      ...prev,
      [name]: name === 'category' ? value : Number(value),
    }))
  }

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    loadProducts()
  }

  const handleSave = async (formData: ProductFormData) => {
    try {
      if (editingProduct) {
        await updateProduct(formData)
      } else {
        await createProduct(formData)
      }
      setShowForm(false)
      setEditingProduct(null)
      loadProducts()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error desconocido')
    }
  }

  const handleEdit = (product: ProductDto) => {
    setEditingProduct(product)
    setShowForm(true)
  }

  const handleDelete = async (id: number) => {
    if (!window.confirm('¿Eliminar este producto?')) {
      return
    }
    try {
      await deleteProduct(id)
      loadProducts()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error desconocido')
    }
  }

  const handleNew = () => {
    setEditingProduct(null)
    setShowForm(true)
  }

  const handleCancel = () => {
    setShowForm(false)
    setEditingProduct(null)
  }

  return (
    <div className="app">
      <header className="app-header">
        <h1>Sales CRUD</h1>
        <p>Productos</p>
      </header>

      <main className="app-main">
        {error && (
          <div className="alert error">
            <span>{error}</span>
            <button type="button" onClick={() => setError(null)}>×</button>
          </div>
        )}

        <section className="filters">
          <form onSubmit={handleSearch}>
            <div className="filter-group">
              <label htmlFor="filter">Filtro</label>
              <select id="filter" name="filter" value={filters.filter} onChange={handleFilterChange}>
                <option value={Filter.LESS_THAN}>Menor que</option>
                <option value={Filter.MORE_THAN}>Mayor que</option>
                <option value={Filter.BETWEN}>Entre</option>
              </select>
            </div>

            {filters.filter === Filter.BETWEN ? (
              <>
                <div className="filter-group">
                  <label htmlFor="minPrice">Precio mín</label>
                  <input id="minPrice" name="minPrice" type="number" step="0.01" value={filters.minPrice} onChange={handleFilterChange} />
                </div>
                <div className="filter-group">
                  <label htmlFor="maxPrice">Precio máx</label>
                  <input id="maxPrice" name="maxPrice" type="number" step="0.01" value={filters.maxPrice} onChange={handleFilterChange} />
                </div>
              </>
            ) : (
              <div className="filter-group">
                <label htmlFor="price">Precio</label>
                <input id="price" name="price" type="number" step="0.01" value={filters.price} onChange={handleFilterChange} />
              </div>
            )}

            <div className="filter-group">
              <label htmlFor="category">Categoría</label>
              <select id="category" name="category" value={filters.category} onChange={handleFilterChange}>
                {CATEGORIES.map((category) => (
                  <option key={category.id} value={category.name}>
                    {category.name}
                  </option>
                ))}
              </select>
            </div>

            <div className="filter-group">
              <label htmlFor="length">Cantidad</label>
              <input id="length" name="length" type="number" value={filters.length} onChange={handleFilterChange} />
            </div>

            <button type="submit" className="btn-primary">Buscar</button>
          </form>
        </section>

        <section className="actions">
          <button type="button" className="btn-primary" onClick={handleNew}>Nuevo producto</button>
        </section>

        {showForm && (
          <div className="modal-overlay" onClick={handleCancel}>
            <div className="modal-content" onClick={(e) => e.stopPropagation()}>
              <ProductForm product={editingProduct} onSave={handleSave} onCancel={handleCancel} />
            </div>
          </div>
        )}

        <section className="list-section">
          {loading ? (
            <p className="loading">Cargando...</p>
          ) : (
            <ProductList products={products} onEdit={handleEdit} onDelete={handleDelete} />
          )}
        </section>
      </main>
    </div>
  )
}

export default App
