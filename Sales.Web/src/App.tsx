import { useState, useEffect, useCallback } from 'react'
import { ProductForm } from './components/ProductForm'
import { ProductList } from './components/ProductList'
import { ResponseModal } from './components/ResponseModal'
import { ConfirmModal } from './components/ConfirmModal'
import { Login } from './components/Login'
import { Filter, type ProductDto, type ProductFilters, type ProductFormData } from './types'
import { createProduct, deleteProduct, getProducts, updateProduct } from './services/productService'
import { checkAuth, logout } from './services/authService'
import { CATEGORIES } from './data/categories'
import './App.css'

function App() {
  const [products, setProducts] = useState<ProductDto[]>([])
  const [loading, setLoading] = useState(false)
  const [editingProduct, setEditingProduct] = useState<ProductDto | null>(null)
  const [showForm, setShowForm] = useState(false)
  const [isProcessing, setIsProcessing] = useState(false)
  const [modal, setModal] = useState({ isOpen: false, isSuccess: false, message: '' })
  const [confirmDelete, setConfirmDelete] = useState<{ isOpen: boolean; productId: number | null }>({
    isOpen: false,
    productId: null,
  })
  const [isAuthenticated, setIsAuthenticated] = useState(false)
  const [checkingAuth, setCheckingAuth] = useState(true)
  const [username, setUsername] = useState('')

  const formatErrorMessage = (err: unknown): string => {
    const message = err instanceof Error ? err.message : 'Error desconocido'
    if (message === 'Failed to fetch') {
      return 'No se pudo conectar con el servidor. Intentalo más tarde.'
    }
    return message
  }
  const [filters, setFilters] = useState<ProductFilters>({
    filter: Filter.LESS_THAN,
    price: 0,
    minPrice: 0,
    maxPrice: 1000,
    length: 10,
    category: 'Toys',
  })

  const openModal = (isSuccess: boolean, message: string) => {
    setModal({ isOpen: true, isSuccess, message })
  }

  const closeModal = () => {
    setModal((prev) => ({ ...prev, isOpen: false }))
  }

  const loadProducts = useCallback(async () => {
    setLoading(true)
    try {
      const result = await getProducts(filters)
      if (!result.isSuccess) {
        openModal(false, result.message)
      }
      setProducts(result.data)
    } catch (err) {
      openModal(false, formatErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }, [filters])

  useEffect(() => {
    const verifyAuth = async () => {
      try {
        const result = await checkAuth()
        if (result.isSuccess) {
          setIsAuthenticated(true)
          setUsername(result.data)
        } else {
          setIsAuthenticated(false)
        }
      } catch {
        setIsAuthenticated(false)
      } finally {
        setCheckingAuth(false)
      }
    }

    verifyAuth()
  }, [])

  useEffect(() => {
    if (isAuthenticated) {
      loadProducts()
    }
  }, [isAuthenticated, loadProducts])

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
    setIsProcessing(true)
    try {
      const result = editingProduct
        ? await updateProduct(formData)
        : await createProduct(formData)

      if (!result.isSuccess) {
        openModal(false, result.message)
        return
      }

      openModal(
        true,
        result.message || (editingProduct ? 'Producto actualizado correctamente' : 'Producto creado correctamente'),
      )
      setShowForm(false)
      setEditingProduct(null)
      loadProducts()
    } catch (err) {
      openModal(false, formatErrorMessage(err))
    } finally {
      setIsProcessing(false)
    }
  }

  const handleEdit = (product: ProductDto) => {
    setEditingProduct(product)
    setShowForm(true)
  }

  const handleDelete = (id: number) => {
    setConfirmDelete({ isOpen: true, productId: id })
  }

  const handleConfirmDelete = async () => {
    if (confirmDelete.productId === null) {
      return
    }
    setIsProcessing(true)
    try {
      const result = await deleteProduct(confirmDelete.productId)
      if (!result.isSuccess) {
        openModal(false, result.message)
        return
      }
      openModal(true, result.message || 'Producto eliminado correctamente')
      loadProducts()
    } catch (err) {
      openModal(false, formatErrorMessage(err))
    } finally {
      setIsProcessing(false)
      setConfirmDelete({ isOpen: false, productId: null })
    }
  }

  const handleCancelDelete = () => {
    setConfirmDelete({ isOpen: false, productId: null })
  }

  const handleNew = () => {
    setEditingProduct(null)
    setShowForm(true)
  }

  const handleCancel = () => {
    setShowForm(false)
    setEditingProduct(null)
  }

  const handleLogin = (loggedUser: string) => {
    setIsAuthenticated(true)
    setUsername(loggedUser)
  }

  const handleLoginError = (message: string) => {
    openModal(false, message)
  }

  const handleLogout = async () => {
    try {
      await logout()
    } catch {
      // ignore
    } finally {
      setIsAuthenticated(false)
      setUsername('')
    }
  }

  if (checkingAuth) {
    return (
      <div className="app">
        <main className="app-main">
          <p className="loading">Verificando sesión...</p>
        </main>
      </div>
    )
  }

  if (!isAuthenticated) {
    return (
      <div className="app">
        <header className="app-header">
          <h1>Sales CRUD</h1>
          <p>Inicio de sesión</p>
        </header>
        <main className="app-main">
          <Login onLogin={handleLogin} onError={handleLoginError} />
        </main>
        <ResponseModal
          isOpen={modal.isOpen}
          isSuccess={modal.isSuccess}
          message={modal.message}
          onClose={closeModal}
        />
      </div>
    )
  }

  return (
    <div className="app">
      <header className="app-header">
        <h1>Sales CRUD</h1>
        <p>Productos</p>
        <div className="user-info">
          <span id="logged-user">{username}</span>
          <button type="button" id="logout-button" className="btn-secondary" onClick={handleLogout}>
            Cerrar sesión
          </button>
        </div>
      </header>

      <main className="app-main">
        <section className="filters">
          <form onSubmit={handleSearch}>
            <div className="filter-group">
              <label htmlFor="filter">Filtro</label>
              <select id="filter" name="filter" value={filters.filter} onChange={handleFilterChange}>
                <option value={Filter.LESS_THAN}>Menor que</option>
                <option value={Filter.MORE_THAN}>Mayor que</option>
                <option value={Filter.BETWEEN}>Entre</option>
              </select>
            </div>

            {filters.filter === Filter.BETWEEN ? (
              <>
                <div className="filter-group">
                  <label htmlFor="minPrice">Precio mín</label>
                  <input id="minPrice" name="minPrice" type="number" value={filters.minPrice} onChange={handleFilterChange} />
                </div>
                <div className="filter-group">
                  <label htmlFor="maxPrice">Precio máx</label>
                  <input id="maxPrice" name="maxPrice" type="number" value={filters.maxPrice} onChange={handleFilterChange} />
                </div>
              </>
            ) : (
              <div className="filter-group">
                <label htmlFor="price">Precio</label>
                <input id="price" name="price" type="number" value={filters.price} onChange={handleFilterChange} />
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
          <button type="button" id="new-product" className="btn-primary" onClick={handleNew}>Nuevo producto</button>
        </section>

        {showForm && (
          <div className="modal-overlay" onClick={isProcessing ? undefined : handleCancel}>
            <div className="modal-content" onClick={(e) => e.stopPropagation()}>
              <ProductForm
                product={editingProduct}
                onSave={handleSave}
                onCancel={handleCancel}
                isProcessing={isProcessing}
              />
            </div>
          </div>
        )}

        <section className="list-section">
          {loading ? (
            <p className="loading">Cargando...</p>
          ) : (
            <ProductList products={products} onEdit={handleEdit} onDelete={handleDelete} isProcessing={isProcessing} />
          )}
        </section>
      </main>

      <ResponseModal
        isOpen={modal.isOpen}
        isSuccess={modal.isSuccess}
        message={modal.message}
        onClose={closeModal}
      />

      <ConfirmModal
        isOpen={confirmDelete.isOpen}
        title="Confirmar eliminación"
        message={
          confirmDelete.productId !== null
            ? `¿Está seguro de eliminar "${products.find((p) => p.productId === confirmDelete.productId)?.productName ?? 'este producto'}"? Esta acción no se puede deshacer.`
            : '¿Está seguro de eliminar este producto? Esta acción no se puede deshacer.'
        }
        confirmText="Eliminar"
        onConfirm={handleConfirmDelete}
        onCancel={handleCancelDelete}
      />
    </div>
  )
}

export default App
