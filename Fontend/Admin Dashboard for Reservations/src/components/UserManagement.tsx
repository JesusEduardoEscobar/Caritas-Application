import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Avatar, AvatarFallback } from './ui/avatar';
import { Users, Search, Edit, Trash2, Mail, Phone, Shield, User, Building2 } from 'lucide-react';
import { useAuth } from './auth/AuthProvider';
import { getUsers, getUsersByShelter, filterUsers } from '../Services/authUser';
import { deleteUser, completeUserRegistration, createAdmin, editUser, createUser } from '../Services/authLogin';
import { getAllShelters } from '../Services/authShelter';
import type { User as UserType, Shelter } from '../types/models';
import { toast } from 'sonner';

export function UserManagement() {
  const { admin } = useAuth();
  const [allUsers, setAllUsers] = useState<UserType[]>([]);
  const [shelters, setShelters] = useState<Shelter[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedShelter, setSelectedShelter] = useState<string>('');
  const [isLoading, setIsLoading] = useState(true);
  const [showUserForm, setShowUserForm] = useState(false);
  const [showAdminForm, setShowAdminForm] = useState(false);

  // Estados para completar registro de usuario
  const [email, setEmail] = useState("");
  const [numero, setNumero] = useState("");
  const [shelterId, setShelterId] = useState("");
  const [nivel, setNivel] = useState("Medio");
  const [verificado, setVerificado] = useState(true);

  // Estados para editar usuario
  const [showEditForm, setShowEditForm] = useState(false);
  const [editingUserId, setEditingUserId] = useState<number | null>(null);
  const [editName, setEditName] = useState("");
  const [editPhone, setEditPhone] = useState("");
  const [editVerified, setEditVerified] = useState(false);
  const [editEconomicLevel, setEditEconomicLevel] = useState("Medio");
  const [editShelterId, setEditShelterId] = useState("");

  // Estados para crear admin
  const [adminName, setAdminName] = useState("");
  const [adminEmail, setAdminEmail] = useState("");
  const [adminPassword, setAdminPassword] = useState("");
  const [adminConfirm, setAdminConfirm] = useState("");
  const [adminPasswordAdmin, setAdminPasswordAdmin] = useState("");

  // Estados para creaer usuario
  const [showRegisterUserForm, setShowRegisterUserForm] = useState(false);
  const [registerName, setRegisterName] = useState("");
  const [registerEmail, setRegisterEmail] = useState("");
  const [registerPassword, setRegisterPassword] = useState("");
  const [registerConfirmPassword, setRegisterConfirmPassword] = useState("");
  const [registerFechaDeNacimiento, setRegisterFechaDeNacimiento] = useState("");
  const [registerShelterId, setRegisterShelterId] = useState("");
  const [registerNumero, setRegisterNumero] = useState("");
  const [registerNivel, setRegisterNivel] = useState("Medium");
  const [registerVerificado, setRegisterVerificado] = useState(false);


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

  useEffect(() => {
    // Si el admin tiene un shelter asignado, lo usamos
    if (admin?.shelter_id) {
      setSelectedShelter(admin.shelter_id.toString());
    } else {
      // Si no, mostramos todos los usuarios
      setSelectedShelter('all');
    }
  }, [admin]);

  const loadShelters = async () => {
    try {
      const data = await getAllShelters();
      setShelters(data);
    } catch (error) {
      console.error('Error al cargar shelters:', error);
      toast.error('Error al cargar los refugios');
    }
  };

  const loadUsers = async () => {
    setIsLoading(true);
    try {
      let data: UserType[];
      if (selectedShelter === 'all') {
        data = await getUsers();
      } else {
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
    if (!window.confirm('¿Está seguro de eliminar este usuario? Esta acción no se puede deshacer.')) {
      return;
    }
    
    try {
      await deleteUser(id);
      toast.success('Usuario eliminado exitosamente');
      await loadUsers();
    } catch (error: any) {
      console.error('Error al eliminar usuario:', error);
      toast.error(error.message || 'Error al eliminar el usuario');
    }
  };

  const handleRegisterUser = async () => {
    if (!email.trim()) {
      toast.error("El correo es obligatorio");
      return;
    }
    if (!shelterId) {
      toast.error("Debe seleccionar un refugio");
      return;
    }

    try {
      await completeUserRegistration(email, numero, parseInt(shelterId), nivel, verificado);
      toast.success("Usuario completado correctamente");
      
      setEmail("");
      setNumero("");
      setShelterId("");
      setNivel("Medio");
      setVerificado(true);
      setShowUserForm(false);
      
      await loadUsers();
    } catch (err: any) {
      console.error("Error al completar usuario:", err);
      toast.error(err.message || "Error al completar el registro del usuario");
    }
  };

  // --- FUNCIÓN PARA EDITAR USUARIO ---
  const handleEditUser = async () => {
    if (!editingUserId) {
      toast.error("No hay usuario seleccionado para editar");
      return;
    }

    if (!editName.trim()) {
      toast.error("El nombre no puede estar vacío");
      return;
    }

    try {
      const updatedUser = {
        id: editingUserId,
        nombre: editName.trim(),
        numero: editPhone?.trim() || undefined,
        shelterId: editShelterId ? parseInt(editShelterId) : undefined,
        verificado: editVerified,
        nivelEconomico: editEconomicLevel,
      };


      const response = await editUser(updatedUser);

      if (!response?.ok) {
        throw new Error(response?.message || "Error al editar usuario");
      }

      toast.success("Usuario editado correctamente");

      setShowEditForm(false);
      setEditingUserId(null);
      setEditName("");
      setEditPhone("");
      setEditingUserId(null);
      setEditVerified(false);
      setEditEconomicLevel("Medio");

      await loadUsers();
    } catch (err: any) {
      console.error("Error al editar usuario:", err);
      toast.error(err.message || "Ocurrió un error al editar el usuario");
    }
  };

  // --- FUNCIÓN PARA ABRIR EL FORMULARIO DE EDICIÓN ---
  const openEditForm = (user: UserType) => {
    setEditingUserId(user.id);
    setEditName(user.name || "");
    setEditPhone(user.phone || "");
    setEditVerified(Boolean(user.verified));
    setEditEconomicLevel(user.economicLevel || "Medio");
    setEditShelterId(user.shelterId ? user.shelterId.toString() : ""); // 👈 Nuevo
    setShowEditForm(true);
  };

  const handleRegisterAdmin = async () => {
    if (!adminName.trim()) {
      toast.error("El nombre es obligatorio");
      return;
    }
    if (!adminEmail.trim()) {
      toast.error("El correo es obligatorio");
      return;
    }
    if (!adminPassword) {
      toast.error("La contraseña es obligatoria");
      return;
    }
    if (adminPassword.length < 6) {
      toast.error("La contraseña debe tener al menos 6 caracteres");
      return;
    }
    if (adminPassword !== adminConfirm) {
      toast.error("Las contraseñas no coinciden");
      return;
    }
    if (!adminPasswordAdmin) {
      toast.error("Debe ingresar su contraseña actual para autorizar");
      return;
    }

    try {
      await createAdmin(adminName, adminEmail, adminPassword, adminPasswordAdmin);
      toast.success("Administrador creado correctamente");
      
      setAdminName("");
      setAdminEmail("");
      setAdminPassword("");
      setAdminConfirm("");
      setAdminPasswordAdmin("");
      setShowAdminForm(false);
      
      await loadUsers();
    } catch (err: any) {
      console.error("Error al crear admin:", err);
      toast.error(err.message || "Error al crear el administrador");
    }
  };

  const handleCreateUser = async () => {
    if (!registerEmail.trim()) {
      toast.error("El correo es obligatorio");
      return;
    }
    if (registerPassword.length < 6) {
      toast.error("La contraseña debe tener al menos 6 caracteres");
      return;
    }
    if (registerPassword !== registerConfirmPassword) {
      toast.error("Las contraseñas no coinciden");
      return;
    }
    try {
      await createUser(registerName, registerEmail, registerPassword, registerConfirmPassword, registerNumero, registerFechaDeNacimiento, parseInt(registerShelterId), registerNivel, registerVerificado);
      toast.success("Usuario creado correctamente");
      setRegisterName("");
      setRegisterEmail("");
      setRegisterPassword("");
      setRegisterConfirmPassword("");
      setRegisterNumero("");
      setRegisterNivel("Medio");
      setRegisterVerificado(false);
      setShowRegisterUserForm(false);
      await loadUsers();
    } catch (err: any) {
      console.error("Error al crear usuario:", err);
      toast.error(err.message || "Error al crear el usuario");
    }
  }

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

  const UserTable = ({
  users,
  title,
  icon,
}: {
  users: UserType[];
  title: string;
  icon: React.ReactNode;
}) => {
  const isUser = title.toLowerCase().includes('usuario');

  return (
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
                  {isUser && <TableHead>Fecha de nacimiento</TableHead> }
                  {isUser && <TableHead>Nivel Económico</TableHead>  }
                  <TableHead>Refugio</TableHead>
                  <TableHead>Estado</TableHead>
                  <TableHead>Acciones</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {users.map(user => (
                  <TableRow key={user.id}>
                    <TableCell>
                      <div className="flex items-center gap-3">
                        <Avatar>
                          <AvatarFallback>{getInitials(user.name)}</AvatarFallback>
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
                    {isUser && (
                      <>
                      <TableCell>
                      <Badge variant="outline">
                        {user.dateOfBirth
                        ? new Date(user.dateOfBirth).toLocaleDateString("es-MX")
                        : '-'}
                      </Badge>
                    </TableCell>
                   
                      <TableCell>
                        <Badge variant="secondary">{user.economicLevel}</Badge>
                      </TableCell>
                      </>
                    )}
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <Building2 className="h-4 w-4" />
                        <span>{shelters.find(s => s.id === user.shelterId)?.name || 'N/A'}</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge variant={user.verified ? 'default' : 'secondary'}>
                        {user.verified ? 'Verificado' : 'Sin verificar'}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      {isUser && (
                        <div className="flex gap-1">
                        <Button
                          variant="ghost"
                          size="icon"
                          title="Editar usuario"
                          onClick={() => openEditForm(user)}
                        >
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
                      )}
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
};


  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <h2 className="text-2xl">Gestión de Usuarios</h2>
      </div>

      {/* BOTONES PRINCIPALES */}
      <div className="flex gap-4 mb-4">
        <Button onClick={() => {setShowUserForm(!showUserForm);
                               setShowAdminForm(false);
                               setShowRegisterUserForm(false)} }>
          Completar Registro Usuario
        </Button>
        <Button onClick={() => {setShowAdminForm(!showAdminForm);
                                setShowUserForm(false);
                                setShowRegisterUserForm(false)}}>
          Registrar Admin
        </Button>
      <Button onClick={() => {setShowRegisterUserForm(!showRegisterUserForm);
                              setShowUserForm(false);
                              setShowAdminForm(false)}}>
          Crear Usuario
        </Button>
      </div>

      {/* FORMULARIO COMPLETAR USUARIO */}
      {showUserForm && (
        <Card className="border-2">
          <CardHeader>
            <CardTitle>Completar Registro de Usuario</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <div className="flex gap-2">
                <div className="flex-1">
                  <Label>Correo del Usuario</Label>
                  <Input 
                    placeholder="Correo del usuario" 
                    value={email} 
                    onChange={(e) => setEmail(e.target.value)} 
                    className="mt-1"
                  />
                </div>
                <Button 
                  type="button"
                  onClick={() => {
                    const userFound = allUsers.find(u => 
                      u.email.trim().toLowerCase() === email.trim().toLowerCase()
                    );
                    if (userFound) {
                      toast.success("Usuario encontrado");
                      setNumero(userFound.phone || "");
                      setShelterId(userFound.shelter_id ? userFound.shelter_id.toString() : "");
                      setNivel(userFound.economicLevel || "Medio");
                      setVerificado(userFound.verified ?? true);
                    } else {
                      toast.error("No se encontró un usuario con ese correo");
                    }
                  }}
                  className="self-end"
                >
                  Buscar
                </Button>
              </div>

              {/* Mostrar información del usuario encontrado */}
              {numero && (
                <div className="bg-blue-50 p-4 rounded-lg border border-blue-200 space-y-2">
                  <p className="text-sm font-medium text-blue-900">✓ Usuario encontrado</p>
                  <div className="space-y-1">
                    <p className="text-sm text-blue-700">
                      <span className="font-medium">Nombre:</span> {allUsers.find(u => u.email === email)?.name || "Usuario"}
                    </p>
                    <p className="text-sm text-blue-700">
                      <span className="font-medium">Teléfono:</span> {numero}
                    </p>
                  </div>
                </div>
              )}

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <Label>Refugio <span className="text-red-500">*</span></Label>
                  <Select value={shelterId} onValueChange={setShelterId}>
                    <SelectTrigger className="mt-1">
                      <SelectValue placeholder="Seleccione un refugio" />
                    </SelectTrigger>
                    <SelectContent>
                      {shelters.map(shelter => (
                        <SelectItem key={shelter.id} value={shelter.id.toString()}>
                          {shelter.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <div>
                <Label>Nivel Económico</Label>
                <Select value={nivel} onValueChange={setNivel}>
                  <SelectTrigger className="mt-1">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Bajo">Bajo</SelectItem>
                    <SelectItem value="Medio">Medio</SelectItem>
                    <SelectItem value="Alto">Alto</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div className="flex items-center gap-2">
                <input 
                  type="checkbox" 
                  id="verificado"
                  checked={verificado} 
                  onChange={(e) => setVerificado(e.target.checked)}
                  className="h-4 w-4"
                /> 
                <Label htmlFor="verificado" className="cursor-pointer">
                  Usuario Verificado
                </Label>
              </div>

              <div className="flex gap-2 pt-4">
                <Button 
                  onClick={handleRegisterUser}
                  disabled={!email || !shelterId}
                >
                  Guardar Cambios
                </Button>
                <Button 
                  variant="outline"
                  onClick={() => {
                    setShowUserForm(false);
                    setEmail("");
                    setNumero("");
                    setShelterId("");
                    setNivel("Medio");
                    setVerificado(true);
                  }}
                >
                  Cancelar
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>
      )}

      {/* FORMULARIO EDITAR USUARIO */}
      {showEditForm && (
        <Card className="border-2 border-amber-500">
          <CardHeader>
            <CardTitle>Editar Usuario</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <div>
                <Label>Nombre</Label>
                <Input 
                  placeholder="Nombre del usuario" 
                  value={editName} 
                  onChange={(e) => setEditName(e.target.value)} 
                  className="mt-1"
                />
              </div>

              <div>
                <Label>Teléfono</Label>
                <Input 
                  placeholder="Número de teléfono (puede incluir +52)" 
                  value={editPhone} 
                  onChange={(e) => setEditPhone(e.target.value)} 
                  className="mt-1"
                />
              </div>

              <div>
                <Label>Refugio / Shelter</Label>
                <Select value={editShelterId} onValueChange={setEditShelterId}>
                  <SelectTrigger className="mt-1">
                    <SelectValue placeholder="Seleccione un refugio" />
                  </SelectTrigger>
                    <SelectContent>
                      {shelters.map(shelter => (
                        <SelectItem key={shelter.id} value={shelter.id.toString()}>
                          {shelter.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                </Select>
              </div>

              <div>
                <Label>Nivel Económico</Label>
                <Select value={editEconomicLevel} onValueChange={setEditEconomicLevel}>
                  <SelectTrigger className="mt-1">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Bajo">Bajo</SelectItem>
                    <SelectItem value="Medio">Medio</SelectItem>
                    <SelectItem value="Alto">Alto</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div className="flex items-center gap-2">
                <input 
                  type="checkbox" 
                  id="editVerified"
                  checked={editVerified} 
                  onChange={(e) => setEditVerified(e.target.checked)}
                  className="h-4 w-4"
                /> 
                <Label htmlFor="editVerified" className="cursor-pointer">
                  Usuario Verificado
                </Label>
              </div>

              <div className="flex gap-2 pt-4">
                <Button 
                  onClick={handleEditUser}
                  disabled={!editName.trim()}
                >
                  Guardar Cambios
                </Button>
                <Button 
                  variant="outline"
                  onClick={() => {
                    setShowEditForm(false);
                    setEditingUserId(null);
                    setEditName("");
                    setEditPhone("");
                    setEditVerified(false);
                    setEditEconomicLevel("Medio");
                  }}
                >
                  Cancelar
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>
      )}

      {/* FORMULARIO CREAR ADMIN */}
      {showAdminForm && (
        <Card className="border-2">
          <CardHeader>
            <CardTitle>Registrar Nuevo Administrador</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <div>
                <Label>Nombre Completo</Label>
                <Input 
                  placeholder="Nombre del administrador" 
                  value={adminName} 
                  onChange={(e) => setAdminName(e.target.value)} 
                  className="mt-1"
                />
              </div>
              <div>
                <Label>Correo Electrónico</Label>
                <Input 
                  placeholder="correo@ejemplo.com" 
                  type="email"
                  value={adminEmail} 
                  onChange={(e) => setAdminEmail(e.target.value)} 
                  className="mt-1"
                />
              </div>
              <div>
                <Label>Contraseña</Label>
                <Input 
                  placeholder="Contraseña del nuevo admin" 
                  type="password" 
                  value={adminPassword} 
                  onChange={(e) => setAdminPassword(e.target.value)} 
                  className="mt-1"
                />
              </div>
              <div>
                <Label>Confirmar Contraseña</Label>
                <Input 
                  placeholder="Confirmar contraseña" 
                  type="password" 
                  value={adminConfirm} 
                  onChange={(e) => setAdminConfirm(e.target.value)} 
                  className="mt-1"
                />
              </div>
              <div>
                <Label>Tu Contraseña (Admin Actual)</Label>
                <Input 
                  placeholder="Tu contraseña para autorizar" 
                  type="password" 
                  value={adminPasswordAdmin} 
                  onChange={(e) => setAdminPasswordAdmin(e.target.value)} 
                  className="mt-1"
                />
              </div>

              <div className="flex gap-2 pt-4">
                <Button 
                  onClick={handleRegisterAdmin}
                  disabled={!adminName || !adminEmail || !adminPassword || !adminConfirm || !adminPasswordAdmin}
                >
                  Crear Admin
                </Button>
                <Button 
                  variant="outline"
                  onClick={() => {
                    setShowAdminForm(false);
                    setAdminName("");
                    setAdminEmail("");
                    setAdminPassword("");
                    setAdminConfirm("");
                    setAdminPasswordAdmin("");
                  }}
                >
                  Cancelar
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>
      )}

      {/* FORMULARIO CREAR USUARIO */}
      {showRegisterUserForm && (
        <Card className="border-2">
          <CardHeader>
            <CardTitle>Registrar Nuevo Usuario</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <div>
                <Label>Nombre Completo</Label>
                <Input
                  placeholder="Nombre del usuario"
                  value={registerName}
                  onChange={(e) => setRegisterName(e.target.value)}
                  className="mt-1"
                />
              </div>
              <div>
                <Label>Correo Electrónico</Label>
                <Input
                  placeholder="Correo del usuario"
                  type="email"
                  value={registerEmail}
                  onChange={(e) => setRegisterEmail(e.target.value)}
                  className="mt-1"
                /> 
              </div>
              <div>
                <Label>Contraseña</Label>
                <Input
                  placeholder="Contraseña del usuario"
                  type="password"
                  value={registerPassword}
                  onChange={(e) => setRegisterPassword(e.target.value)}
                  className="mt-1"
                />
              </div>
              <div>
                <Label>Confirmar Contraseña</Label>
                <Input
                  placeholder="Confirmar contraseña"
                  type="password"
                  value={registerConfirmPassword}
                  onChange={(e) => setRegisterConfirmPassword(e.target.value)}
                  className="mt-1"
                />
              </div>
              <div>
                <Label>Número de Teléfono</Label>
                <Input
                  placeholder="Número de teléfono (puede incluir +52)"
                  value={registerNumero}
                  onChange={(e) => setRegisterNumero(e.target.value)}
                  className="mt-1"
                />
              </div>
              <div>
                <Label>Fecha de Nacimiento <span className="text-red-500">*</span></Label>
                <Input
                  type="date"
                  value={registerFechaDeNacimiento}
                  onChange={(e) => setRegisterFechaDeNacimiento(e.target.value)}
                  className="mt-1"
                  max={new Date().toISOString().split('T')[0]}
                />
              </div>
              <div>
                <Label>Refugio / Shelter</Label>
                <Select value={registerShelterId} onValueChange={setRegisterShelterId}>
                  <SelectTrigger className="mt-1">
                    <SelectValue placeholder="Seleccione un refugio" />
                  </SelectTrigger>
                  <SelectContent>
                    {shelters.map(shelter => (
                      <SelectItem key={shelter.id} value={shelter.id.toString()}>
                        {shelter.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label>Nivel Económico</Label>
                <Select value={registerNivel} onValueChange={setRegisterNivel}>
                  <SelectTrigger className="mt-1">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Bajo">Bajo</SelectItem>
                    <SelectItem value="Medio">Medio</SelectItem>
                    <SelectItem value="Alto">Alto</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div className="flex items-center gap-2">
                <input
                  type="checkbox"
                  id="registerVerificado"
                  checked={registerVerificado}
                  onChange={(e) => setRegisterVerificado(e.target.checked)}
                  className="h-4 w-4"
                />
                <Label htmlFor="registerVerificado" className="cursor-pointer">
                  Usuario Verificado
                </Label>
              </div>
              <div className="flex gap-2 pt-4">
                <Button
                  onClick={handleCreateUser}
                  disabled={!registerEmail || !registerPassword || !registerConfirmPassword || !registerShelterId}
                >
                  Crear Usuario
                </Button>
                <Button
                  variant="outline"
                  onClick={() => {
                    setShowRegisterUserForm(false);
                    setRegisterName("");
                    setRegisterEmail("");
                    setRegisterPassword("");
                    setRegisterConfirmPassword("");
                    setRegisterNumero("");
                    setRegisterNivel("Medio");
                    setRegisterVerificado(false);
                  }}
                >                  
                Cancelar
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>
      )}

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
                <Search className="absolute left-3 top-3 h-4 w-5 text-muted-foreground mr-2" />
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
            icon={<User className="h-6 w-6" style={{ color: '#06b6d4' }} />}
          />
        </div>
      )}
    </div>
  );
}