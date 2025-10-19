import { Button } from '../ui/button';
import { Avatar, AvatarFallback } from '../ui/avatar';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuSeparator, DropdownMenuTrigger } from '../ui/dropdown-menu';
import { useAuth } from './AuthProvider';
import { LogOut, User, Settings } from 'lucide-react';

export function AuthHeader() {
  const { admin, logout } = useAuth();

  if (!admin) return null;

  const initials =( admin.name ?? '')
    .split(' ')
    .map(name => name[0])
    .join('')
    .toUpperCase()
    .slice(0, 2);

  return (
    <div className="flex items-center space-x-4">
      <div className="hidden md:block text-right">
        <p className="text-sm">{admin.name ?? `Admin${admin.id}`}</p>
        <p className="text-xs text-muted-foreground capitalize">{admin.role }</p>
      </div>
      
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button variant="ghost" className="relative h-10 w-10 rounded-full">
            <Avatar className="h-10 w-10">
              <AvatarFallback className="bg-indigo-100 text-indigo-600">
                {initials}
              </AvatarFallback>
            </Avatar>
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent className="w-56" align="end" forceMount>
          <div className="flex items-center justify-start gap-2 p-2">
            <div className="flex flex-col space-y-1 leading-none">
              <p className="text-sm">{admin.name}</p>
              <p className="text-xs text-muted-foreground">{admin.email}</p>
            </div>
          </div>
          <DropdownMenuItem onClick={logout}>
            <LogOut className="mr-2 h-4 w-4" />
            <span>Cerrar sesión</span>
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </div>
  );
}