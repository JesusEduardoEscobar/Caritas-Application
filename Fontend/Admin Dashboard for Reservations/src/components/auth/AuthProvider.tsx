import { createContext, useContext, useState, useEffect, ReactNode } from 'react';

interface Admin {
  id: string;
  email: string;
  name: string;
  role: 'admin' | 'super_admin';
  createdAt: string;
}

interface AuthContextType {
  admin: Admin | null;
  login: (email: string, password: string) => Promise<{ success: boolean; error?: string }>;
  register: (email: string, password: string, name: string) => Promise<{ success: boolean; error?: string }>;
  logout: () => void;
  isLoading: boolean;
}

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

export function AuthProvider({ children }: AuthProviderProps) {
  const [admin, setAdmin] = useState<Admin | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Check for saved session
    const savedSession = localStorage.getItem('admin_session');
    if (savedSession) {
      try {
        const adminData = JSON.parse(savedSession);
        setAdmin(adminData);
      } catch (error) {
        console.error('Error parsing saved session:', error);
        localStorage.removeItem('admin_session');
      }
    }
    setIsLoading(false);
  }, []);

  const login = async (email: string, password: string) => {
    setIsLoading(true);
    
    // TODO: Replace with actual API call
    try {
      // Mock login - replace with your backend API
      if (email === 'admin@test.com' && password === 'admin123') {
        const adminData: Admin = {
          id: '1',
          email: 'admin@test.com',
          name: 'Administrador Principal',
          role: 'super_admin',
          createdAt: new Date().toISOString()
        };
        
        setAdmin(adminData);
        localStorage.setItem('admin_session', JSON.stringify(adminData));
        setIsLoading(false);
        return { success: true };
      } else {
        setIsLoading(false);
        return { success: false, error: 'Credenciales incorrectas' };
      }
    } catch (error) {
      setIsLoading(false);
      return { success: false, error: 'Error de conexión' };
    }
  };

  const register = async (email: string, password: string, name: string) => {
    setIsLoading(true);
    
    // TODO: Replace with actual API call
    try {
      // Mock registration - replace with your backend API
      const adminData: Admin = {
        id: Date.now().toString(),
        email,
        name,
        role: 'admin',
        createdAt: new Date().toISOString()
      };
      
      setAdmin(adminData);
      localStorage.setItem('admin_session', JSON.stringify(adminData));
      setIsLoading(false);
      return { success: true };
    } catch (error) {
      setIsLoading(false);
      return { success: false, error: 'Error al crear la cuenta' };
    }
  };

  const logout = () => {
    setAdmin(null);
    localStorage.removeItem('admin_session');
  };

  const value = {
    admin,
    login,
    register,
    logout,
    isLoading
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}