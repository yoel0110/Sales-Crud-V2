import { useState } from 'react';

interface LoginProps {
  onLogin: (username: string) => void;
  onError: (message: string) => void;
}

export const Login = ({ onLogin, onError }: LoginProps) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    try {
      const response = await fetch(
        `${import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5138'}/api/v1/auth/login`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          credentials: 'include',
          body: JSON.stringify({ username, password }),
        },
      );

      const result = await response.json();

      if (!response.ok || !result.isSuccess) {
        onError(result.message || 'Error al iniciar sesión.');
        return;
      }

      onLogin(result.data.username ?? username);
    } catch {
      onError('No se pudo conectar con el servidor.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="login-container">
      <form onSubmit={handleSubmit} className="login-form">
        <h2>Iniciar sesión</h2>

        <div className="form-group">
          <label htmlFor="username">Usuario</label>
          <input
            id="username"
            name="username"
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
            disabled={isLoading}
          />
        </div>

        <div className="form-group">
          <label htmlFor="password">Contraseña</label>
          <input
            id="password"
            name="password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            disabled={isLoading}
          />
        </div>

        <button
          type="submit"
          id="login-button"
          className="btn-primary"
          disabled={isLoading || !username || !password}
        >
          {isLoading ? 'Ingresando...' : 'Ingresar'}
        </button>
      </form>
    </div>
  );
};
