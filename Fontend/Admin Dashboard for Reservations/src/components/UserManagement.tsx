import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Avatar, AvatarFallback } from './ui/avatar';
import { Users, Search, Edit, Trash2, Mail, Phone, Shield, User as UserIcon, Building2 } from 'lucide-react';
import { useAuth } from './auth/AuthProvider';
import { getUsers, getUsersByShelter, deleteUser, filterUsers } from '../Services/authUser';
import { getAllShelters } from '../Services/authShelter';
import type { User, Shelter } from '../types/models';
import { toast } from 'sonner';

export function UserManagement() {
  const { admin } = useAuth();
  const [allUsers, setAllUsers] = useState<User[]>([]);
  const [shelters, setShelters] = useState<Shelter[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedShelter, setSelectedShelter] = useState<string>('');
  const [isLoading, setIsLoading] = useState(true);

  // Cargar shelters al montar el componente
  useEffect(() => {
    loadShelters();
  }, []);

  // Cargar usuarios cuando cambie el shelter seleccionado
  useEffect(() => {
    if (selectedShelter) {
      loadUsers();
    }
  }, [selectedShelter]);

  // Establecer el shelter del admin como predeterminado
  useEffect(() => {
    if (admin?.shelter_id && !selectedShelter) {
      setSelectedShelter(admin.shelter_id.toString());
    }
  }, [admin, selectedShelter]);

  const loadShelters = async () => {
  try {
    const data = await getAllShelters();
    setShelters(data); // ✅ ahora data es Shelter[]
    } catch (error) {
      console.error('Error al cargar shelters:', error);
      toast.error('Error al cargar los refugios');
    }
  };


  const loadUsers = async () => {
    setIsLoading(true);
    try {
      let data: User[];
      if (selectedShelter === 'all') {
        // Cargar todos los usuarios
        data = await getUsers();
      } else {
        // Cargar usuarios por shelter
        data = await getUsersByShelter(parseInt(selectedShelter));
      }
      setAllUsers(data);
    } catch (error) {
      console.error('Error al cargar usuarios:', error);
      toast.error('Error al cargar los usuarios');
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteUser = async (id: number) => {
    if (window.confirm('¿Está seguro de eliminar este usuario?')) {
      try {
        await deleteUser(id);
        toast.success('Usuario eliminado exitosamente');
        loadUsers();
      } catch (error) {
        console.error('Error al eliminar usuario:', error);
        toast.error('Error al eliminar el usuario');
      }
    }
  };

  // Filtrar usuarios por búsqueda
  const filteredUsers = filterUsers(allUsers, { search: searchTerm });

  // Separar en admins y usuarios
  const adminUsers = filteredUsers.filter(user => user.role === 'admin');
  const regularUsers = filteredUsers.filter(user => user.role === 'user');

  const getInitials = (name: string) => {
    return name.split(' ').map(n => n[0]).join('').toUpperCase();
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-MX', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    });
  };

  const UserTable = ({ users, title, icon }: { users: User[], title: string, icon: React.ReactNode }) => (
    <Card className="border-2">
      <CardHeader>
        <CardTitle className="flex items-center gap-3">
          {icon}
          {title}
          <Badge variant="outline" className="ml-auto px-3 py-1">
            {users.length}
          </Badge>
        </CardTitle>
      </CardHeader>
      <CardContent>
        {users.length === 0 ? (
          <div className="text-center py-10 text-muted-foreground">
            No hay {title.toLowerCase()} registrados
          </div>
        ) : (
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Usuario</TableHead>
                  <TableHead>Contacto</TableHead>
                  <TableHead>Edad</TableHead>
                  <TableHead>Nivel Económico</TableHead>
                  <TableHead>Estado</TableHead>
                  <TableHead>Fecha Registro</TableHead>
                  <TableHead>Acciones</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {users.map(user => (
                  <TableRow key={user.id}>
                    <TableCell>
                      <div className="flex items-center gap-3">
                        <Avatar>
                          <AvatarFallback>
                            {getInitials(user.name)}
                          </AvatarFallback>
                        </Avatar>
                        <div>
                          <p>{user.name}</p>
                          <p className="text-xs text-muted-foreground">ID: {user.id}</p>
                        </div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="space-y-1">
                        <div className="flex items-center gap-2 text-sm">
                          <Mail className="h-3 w-3" />
                          {user.email}
                        </div>
                        {user.phone && (
                          <div className="flex items-center gap-2 text-sm">
                            <Phone className="h-3 w-3" />
                            {user.phone}
                          </div>
                        )}
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline">{user.bithday}</Badge>
                    </TableCell>
                    <TableCell>
                      <Badge variant="secondary">{user.economic_level}</Badge>
                    </TableCell>
                    <TableCell>
                      <Badge variant={user.verified ? 'default' : 'secondary'}>
                        {user.verified ? 'Verificado' : 'Sin verificar'}
                      </Badge>
                    </TableCell>
                    <TableCell className="text-sm">
                      {formatDate(user.created_at)}
                    </TableCell>
                    <TableCell>
                      <div className="flex gap-1">
                        <Button variant="ghost" size="icon" title="Editar usuario">
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button 
                          variant="ghost" 
                          size="icon" 
                          title="Eliminar usuario"
                          onClick={() => handleDeleteUser(user.id)}
                        >
                          <Trash2 className="h-4 w-4 text-red-600" />
                        </Button>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        )}
      </CardContent>
    </Card>
  );

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <h2 className="text-2xl">Gestión de Usuarios</h2>
      </div>

      {/* Filtros */}
      <Card className="border-2">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Users className="h-6 w-6" style={{ color: '#06b6d4' }} />
            Filtros de Búsqueda
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-5">
            <div className="space-y-2">
              <Label>Buscar Usuario</Label>
              <div className="relative">
                <Search className="absolute left-3 top-3 h-5 w-5 text-muted-foreground" />
                <Input
                  placeholder="Nombre, email o teléfono..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10 h-11"
                />
              </div>
            </div>

            <div className="space-y-2">
              <Label>Refugio / Shelter</Label>
              <Select 
                value={selectedShelter} 
                onValueChange={setSelectedShelter}
              >
                <SelectTrigger className="h-11">
                  <Building2 className="h-5 w-5 mr-2" style={{ color: '#06b6d4' }} />
                  <SelectValue placeholder="Seleccione un refugio" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">
                    <span className="flex items-center gap-2">
                      Todos los refugios
                    </span>
                  </SelectItem>
                  {shelters.map(shelter => (
                    <SelectItem key={shelter.id} value={shelter.id.toString()}>
                      <span className="flex items-center gap-2">
                        {shelter.name}
                        {admin?.shelter_id === shelter.id && (
                          <Badge variant="outline" className="ml-2">Mi refugio</Badge>
                        )}
                      </span>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          {/* Estadísticas rápidas */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mt-6 pt-6 border-t">
            <div className="text-center">
              <p className="text-muted-foreground">Total Usuarios</p>
              <p className="text-3xl mt-1" style={{ color: '#06b6d4' }}>
                {filteredUsers.length}
              </p>
            </div>
            <div className="text-center">
              <p className="text-muted-foreground">Administradores</p>
              <p className="text-3xl mt-1" style={{ color: '#06b6d4' }}>
                {adminUsers.length}
              </p>
            </div>
            <div className="text-center">
              <p className="text-muted-foreground">Usuarios Regulares</p>
              <p className="text-3xl mt-1" style={{ color: '#06b6d4' }}>
                {regularUsers.length}
              </p>
            </div>
          </div>
        </CardContent>
      </Card>

      {isLoading ? (
        <Card className="border-2">
          <CardContent className="py-20 text-center">
            <p className="text-muted-foreground">Cargando usuarios...</p>
          </CardContent>
        </Card>
      ) : (
        <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">
          {/* Columna de Administradores */}
          <UserTable 
            users={adminUsers} 
            title="Administradores" 
            icon={<Shield className="h-6 w-6" style={{ color: '#06b6d4' }} />}
          />

          {/* Columna de Usuarios */}
          <UserTable 
            users={regularUsers} 
            title="Usuarios" 
            icon={<UserIcon className="h-6 w-6" style={{ color: '#06b6d4' }} />}
          />
        </div>
      )}
    </div>
  );
}