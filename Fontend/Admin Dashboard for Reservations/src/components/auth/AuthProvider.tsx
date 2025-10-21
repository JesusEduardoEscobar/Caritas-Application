import { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { loginUser } from '../../Services/authLogin';
import type { Admin, AuthContextType } from '../../types/models';
import { toast } from 'sonner';

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}

interface AuthProviderProps {
  children: ReactNode;
}

// Helper function to decode JWT (simple version for demo)
function decodeJWT(token: string): Admin | null {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    
    console.log('Payload del token:', payload);
    
    // Check if token is expired
    if (payload.exp && payload.exp * 1000 < Date.now()) {
      console.log('Token expirado');
      return null;
    }
    
    // Validar que existan los campos requeridos
    if (!payload.email || !payload.name || !payload.role) {
      console.error('Token inválido: faltan campos requeridos', {
        email: !!payload.email,
        name: !!payload.name,
        role: !!payload.role
      });
      return null;
    }
    
    console.log('Token válido, expira en:', new Date(payload.exp * 1000).toLocaleString());
    
    return {
      id: payload.id || payload.sub || payload.userId || 'unknown',
      email: payload.email,
      name: payload.name,
      role: payload.role,
      shelter_id: payload.shelter_id || payload.shelterId,
      createdAt: payload.createdAt || payload.iat ? new Date(payload.iat * 1000).toISOString() : new Date().toISOString()
    };
  } catch (error) {
    console.error('Error decoding JWT:', error);
    return null;
  }
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [admin, setAdmin] = useState<Admin | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Función para validar y cargar el token
  const validateAndLoadToken = () => {
  const savedToken = localStorage.getItem('token');
  const savedUser = localStorage.getItem('user');

  if (!savedToken || !savedUser) {
    setAdmin(null);
    setToken(null);
    return false;
  }

  const user = JSON.parse(savedUser);
  setAdmin(user);
  setToken(savedToken);
  return true;
};


  // Cargar token al iniciar
  useEffect(() => {
    console.log('Iniciando AuthProvider - verificando token...');
    validateAndLoadToken();
    setIsLoading(false);
  }, []);

  // Verificar token periódicamente (cada 30 segundos)
  useEffect(() => {
    if (!token) return;

    console.log('Iniciando verificación periódica del token');
    
    const interval = setInterval(() => {
      console.log('Verificando validez del token...');
      const isValid = validateAndLoadToken();
      
      if (!isValid) {
        console.log('Token ya no es válido - limpiando intervalo');
        clearInterval(interval);
      }
    }, 10000); // Cada 1 segundos

    return () => {
      console.log('Limpiando intervalo de verificación');
      clearInterval(interval);
    };
  }, [token]);

  // Verificar token cuando la pestaña vuelve a estar activa
  useEffect(() => {
    const handleVisibilityChange = () => {
      if (!document.hidden && token) {
        console.log('👁️ Pestaña visible - verificando token...');
        validateAndLoadToken();
      }
    };

    document.addEventListener('visibilitychange', handleVisibilityChange);

    return () => {
      document.removeEventListener('visibilitychange', handleVisibilityChange);
    };
  }, [token]);

  const login = async (email: string, password: string) => {
    setIsLoading(true);
    
    try {
      console.log('Intentando login para:', email);
      const result = await loginUser(email, password);
      
      if (result.success && result.token && result.user) {
        console.log('Login exitoso:', result.user.email);
        setAdmin(result.user);
        setToken(result.token);
        localStorage.setItem('token', result.token);
        toast.success(`¡Bienvenido, ${result.user.name}!`);
        setIsLoading(false);
        return { success: true };
      }
      
      console.log('Login fallido:', result.error);
      setIsLoading(false);
      return { success: false, error: result.error };
    } catch (error) {
      console.error('Error de conexión:', error);
      setIsLoading(false);
      return { success: false, error: 'Error de conexión con el servidor' };
    }
  };

  const logout = () => {
    console.log('🚪 Cerrando sesión...');
    setAdmin(null);
    setToken(null);
    localStorage.removeItem('token');
    toast.info('Sesión cerrada correctamente');
  };

  const value = {
    admin,
    token,
    login,
    logout,
    isLoading,
    isAuthenticated: !!token && !!admin
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}