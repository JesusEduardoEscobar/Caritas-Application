import { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Avatar, AvatarFallback } from './ui/avatar';
import { Users, Plus, Search, Edit, Trash2, Mail, Phone } from 'lucide-react';

// Mock data
const userData = [
  { 
    id: 1, 
    name: 'Juan Pérez', 
    email: 'juan.perez@email.com', 
    phone: '+1234567890', 
    economi: 'Paciente', 
    status: 'Activo',
    createdAt: '2024-01-10',
    reservations: 3
  },
  { 
    id: 2, 
    name: 'María García', 
    email: 'maria.garcia@email.com', 
    phone: '+1234567891', 
    economi: 'Paciente', 
    status: 'Activo',
    createdAt: '2024-01-08',
    reservations: 1
  },
  { 
    id: 3, 
    name: 'Dr. Carlos López', 
    email: 'carlos.lopez@hospital.com', 
    phone: '+1234567892', 
    economi: 'Médico', 
    status: 'Activo',
    createdAt: '2024-01-05',
    reservations: 0
  },
  { 
    id: 4, 
    name: 'Ana Rodríguez', 
    email: 'ana.rodriguez@email.com', 
    phone: '+1234567893', 
    economi: 'Paciente', 
    status: 'Inactivo',
    createdAt: '2024-01-03',
    reservations: 0
  }
];

const economics = ['Ninguno', 'Basico', 'Urgente', 'Especial'];

export function UserManagement() {
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedRole, setSelectedRole] = useState('Todos');
  const [selectedStatus, setSelectedStatus] = useState('Todos');
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [newUser, setNewUser] = useState({
    name: '',
    email: '',
    phone: '',
    economi: 'Ninguno',
  });

  const filteredUsers = userData.filter(user => {
    const matchesSearch = user.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         user.email.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesRole = selectedRole === 'Todos' || user.economi === selectedRole;
    const matchesStatus = selectedStatus === 'Todos' || user.status === selectedStatus;
    
    return matchesSearch && matchesRole && matchesStatus;
  });

  const handleCreateUser = () => {
    // In real implementation, this would make an API call
    console.log('Creating user:', newUser);
    setIsCreateDialogOpen(false);
    setNewUser({
      name: '',
      email: '',
      phone: '',
      economi: 'Basico'
    });
  };


  const getRoleBadge = (economi: string) => {
    const econimicLevel = {
      'Ninguno': 'outline',
      'Basico': 'default',
      'Urgente': 'destructive',
      'Especial': 'secondary'
    } as const;
    
    return (
      <Badge variant={econimicLevel[economi as keyof typeof econimicLevel]}>
        {economi}
      </Badge>
    );
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <h2 className="text-2xl">Gestión de Usuarios</h2>
        <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
          <DialogTrigger asChild>
            <Button>
              <Plus className="h-4 w-4 mr-2" />
              Crear Usuario
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Crear Nuevo Usuario</DialogTitle>
            </DialogHeader>
            <div className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="name">Nombre Completo</Label>
                <Input
                  id="name"
                  value={newUser.name}
                  onChange={(e) => setNewUser(prev => ({ ...prev, name: e.target.value }))}
                  placeholder="Ingrese el nombre completo"
                />
              </div>
              
              <div className="space-y-2">
                <Label htmlFor="email">Correo Electrónico</Label>
                <Input
                  id="email"
                  type="email"
                  value={newUser.email}
                  onChange={(e) => setNewUser(prev => ({ ...prev, email: e.target.value }))}
                  placeholder="usuario@email.com"
                />
              </div>
              
              <div className="space-y-2">
                <Label htmlFor="phone">Teléfono</Label>
                <Input
                  id="phone"
                  value={newUser.phone}
                  onChange={(e) => setNewUser(prev => ({ ...prev, phone: e.target.value }))}
                  placeholder="+1234567890"
                />
              </div>
              
              

              <div className='space-y-2'>
                <Label htmlFor="economi">Necesidad economica</Label>
                <Select value={newUser.economi} onValueChange={(value: string) => setNewUser(prev => ({ ...prev, economi: value }))}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    {economics.map(economi => (
                      <SelectItem key={economi} value={economi}>{economi}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              
              <div className="flex gap-2 pt-4">
                <Button onClick={handleCreateUser} className="flex-1">
                  Crear Usuario
                </Button>
                <Button 
                  variant="outline" 
                  onClick={() => setIsCreateDialogOpen(false)}
                  className="flex-1"
                >
                  Cancelar
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Users className="h-5 w-5" />
            Filtros de Usuario
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="space-y-2">
              <Label>Buscar</Label>
              <div className="relative">
                <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
                <Input
                  placeholder="Nombre o email..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10"
                />
              </div>
            </div>

            <div className="space-y-2">
              <Label>Necesidad economica</Label>
              <Select value={selectedRole} onValueChange={setSelectedRole}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Todos">Todos los nivels de necesidad</SelectItem>
                  {economics.map(economi => (
                    <SelectItem key={economi} value={economi}>{economi}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Lista de Usuarios</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Usuario</TableHead>
                <TableHead>Contacto</TableHead>
                <TableHead>Necesidad economica</TableHead>
                <TableHead>Reservas</TableHead>
                <TableHead>Fecha Registro</TableHead>
                <TableHead>Acciones</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredUsers.map(user => (
                <TableRow key={user.id}>
                  <TableCell>
                    <div className="flex items-center gap-3">
                      <Avatar>
                        <AvatarFallback>
                          {user.name.split(' ').map(n => n[0]).join('')}
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
                      <div className="flex items-center gap-2 text-sm">
                        <Phone className="h-3 w-3" />
                        {user.phone}
                      </div>
                    </div>
                  </TableCell>
                  <TableCell>
                    {getRoleBadge(user.economi)}
                  </TableCell>
                  <TableCell>
                    <Badge variant="outline">
                      {user.reservations}
                    </Badge>
                  </TableCell>
                  <TableCell className="text-sm">
                    {user.createdAt}
                  </TableCell>
                  <TableCell>
                    <div className="flex gap-1">
                      <Button variant="ghost" size="icon">
                        <Edit className="h-4 w-4" />
                      </Button>
                      <Button variant="ghost" size="icon">
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
    </div>
  );
}