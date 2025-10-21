import { useEffect } from 'react';
import { Button } from '../ui/button';
import { Avatar, AvatarFallback } from '../ui/avatar';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuSeparator, DropdownMenuTrigger } from '../ui/dropdown-menu';
import { useAuth } from './AuthProvider';
import { LogOut, User, Settings, Shield } from 'lucide-react';
import { Badge } from '../ui/badge';

export function AuthHeader() {
  const { admin, logout, token, isAuthenticated } = useAuth();

  // Verificar autenticación en cada render
  useEffect(() => {
    if (!token || !admin || !isAuthenticated) {
      console.warn('AuthHeader: Usuario no autenticado, debería estar en login');
    }
  }, [token, admin, isAuthenticated]);

  if (!admin || !isAuthenticated) {
    console.warn('AuthHeader: No hay admin o no está autenticado');
    return null;
  }

  // Protección contra campos undefined
  const userName = admin.name || 'Usuario';
  const userEmail = admin.email || '';
  
  const initials = userName
    .split(' ')
    .map(name => name[0])
    .join('')
    .toUpperCase()
    .slice(0, 2) || 'U';

  return (
    <div className="flex items-center space-x-4">
      <div className="hidden md:block text-right">
        <div className="flex items-center gap-2 justify-end">
          <p className="text-sm text-white">{userName}</p>
          {admin.role === 'admin' && (
            <Shield className="h-4 w-4" style={{ color: '#06b6d4' }} />
          )}
        </div>
        <p className="text-xs text-white/80 capitalize">{admin.role?.replace('_', ' ') || 'usuario'}</p>
      </div>
      
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button variant="ghost" className="relative h-10 w-10 rounded-full hover:bg-white/20">
            <Avatar className="h-10 w-10">
              <AvatarFallback style={{ backgroundColor: '#06b6d4', color: 'white' }}>
                {initials}
              </AvatarFallback>
            </Avatar>
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent className="w-56" align="end" forceMount>
          <div className="flex items-center justify-start gap-2 p-2">
            <div className="flex flex-col space-y-1 leading-none">
              <p className="text-sm">{userName}</p>
              <p className="text-xs text-muted-foreground">{userEmail}</p>
              {admin.role === 'admin' && (
                <Badge variant="outline" className="mt-1 w-fit" style={{ borderColor: '#06b6d4', color: '#06b6d4' }}>
                  Administrador
                </Badge>
              )}
            </div>
          </div>
          <DropdownMenuSeparator />
          <DropdownMenuItem onClick={logout} className="text-red-600">
            <LogOut className="mr-2 h-4 w-4" />
            <span>Cerrar sesión</span>
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </div>
  );
}