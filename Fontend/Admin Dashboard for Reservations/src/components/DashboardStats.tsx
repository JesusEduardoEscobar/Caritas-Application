import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from './ui/alert-dialog';
import { 
  Users, Bed, Calendar, CheckCircle, AlertCircle, Shield, UserCheck, 
  Home, Loader2, MapPin, Phone, Package, Plus, Pencil, Trash2
} from 'lucide-react';
import { getUsers } from '../Services/authUser';
import { getBeds } from '../Services/authBeds';
import { getAllShelters, createShelter, updateShelter, deleteShelter, validateShelterData } from '../Services/authShelter';
import { getAllServices, createService, updateService, deleteService, validateServiceData, availableIcons } from '../Services/authServices';
import { toast } from 'sonner';
import * as LucideIcons from 'lucide-react';
import { Textarea } from "./ui/textarea";

interface ServiceStats {
  service: string;
  serviceId: number;
  total: number;
  occupied: number;
  available: number;
}

export function DashboardStats() {
  const [totalUsers, setTotalUsers] = useState(0);
  const [totalAdmins, setTotalAdmins] = useState(0);
  const [totalRegularUsers, setTotalRegularUsers] = useState(0);
  const [verifiedUsers, setVerifiedUsers] = useState(0);
  const [isLoading, setIsLoading] = useState(true);

  // Estados para beds, shelters y services
  const [beds, setBeds] = useState<any[]>([]);
  const [shelters, setShelters] = useState<any[]>([]);
  const [services, setServices] = useState<any[]>([]);
  const [shelterBedStats, setShelterBedStats] = useState<Record<number, any>>({});
  const [serviceStats, setServiceStats] = useState<ServiceStats[]>([]);

  // Estados para diálogos
  const [isCreateShelterOpen, setIsCreateShelterOpen] = useState(false);
  const [isCreateServiceOpen, setIsCreateServiceOpen] = useState(false);
  const [isEditShelterOpen, setIsEditShelterOpen] = useState(false);
  const [isEditServiceOpen, setIsEditServiceOpen] = useState(false);
  const [isCreating, setIsCreating] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);

  // Estados para edición
  const [editingShelter, setEditingShelter] = useState<any>(null);
  const [editingService, setEditingService] = useState<any>(null);

  // Estados para confirmación de eliminación
  const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false);
  const [itemToDelete, setItemToDelete] = useState<{ type: 'shelter' | 'service', id: number, name: string } | null>(null);

  // Estados para formularios
  const [newShelter, setNewShelter] = useState({
    name: '',
    address: '',
    latitude: '',
    longitude: '',
    phone: '',
    capacity: ''
  });

  const [newService, setNewService] = useState({
    name: '',
    description: '',
    iconKey: ''
  });

  // Mock data para otros stats
  const stats = {
    activeReservations: 23,
    totalBeds: 0,
    occupiedBeds: 0,
    pendingConfirmations: 5
  };

  useEffect(() => {
    loadUserStats();
    loadSheltersAndServices();
  }, []);

  const loadUserStats = async () => {
    try {
      const users = await getUsers();
      setTotalUsers(users.length);
      setTotalAdmins(users.filter((u: any) => u.role === 'admin').length);
      setTotalRegularUsers(users.filter((u: any) => u.role === 'user').length);
      setVerifiedUsers(users.filter((u: any) => u.verified).length);
    } catch (error) {
      console.error('Error al cargar estadísticas de usuarios:', error);
      toast.error('Error al cargar estadísticas de usuarios');
    } finally {
      setIsLoading(false);
    }
  };

  const loadSheltersAndServices = async () => {
    try {
      const [bedsData, sheltersData, servicesData] = await Promise.all([
        getBeds(),
        getAllShelters(),
        getAllServices()
      ]);

      setBeds(bedsData);
      setShelters(sheltersData);
      setServices(servicesData);

      // Calcular estadísticas de camas por shelter
      const statsByShelter: Record<number, any> = {};
      for (const shelter of sheltersData) {
        const shelterBeds = bedsData.filter((bed: any) => bed.shelterId === shelter.id);
        const available = shelterBeds.filter((bed: any) => bed.isAvailable).length;
        const occupied = shelterBeds.length - available;
        const rate = shelterBeds.length > 0 ? Math.round((occupied / shelterBeds.length) * 100) : 0;

        statsByShelter[shelter.id] = {
          total: shelterBeds.length,
          available,
          occupied,
          occupancyRate: rate
        };
      }
      setShelterBedStats(statsByShelter);

      // Calcular estadísticas por servicio
      const statsPerService: ServiceStats[] = servicesData.map((service: any) => {
        const totalBeds = bedsData.length;
        const availableBeds = bedsData.filter((bed: any) => bed.isAvailable).length;
        const occupiedBeds = totalBeds - availableBeds;

        return {
          service: service.name,
          serviceId: service.id,
          total: totalBeds,
          occupied: occupiedBeds,
          available: availableBeds
        };
      });

      setServiceStats(statsPerService);
      stats.totalBeds = bedsData.length;
      stats.occupiedBeds = bedsData.filter((bed: any) => !bed.isAvailable).length;

    } catch (error) {
      console.error('Error al cargar refugios y servicios:', error);
      toast.error('Error al cargar datos');
    }
  };

  const getIconComponent = (iconKey: string) => {
    const iconName = iconKey
      .split('-')
      .map(word => word.charAt(0).toUpperCase() + word.slice(1))
      .join('');
    
    const Icon = (LucideIcons as any)[iconName];
    return Icon || Package;
  };

  // ==================== CREAR REFUGIO ====================
  const handleCreateShelter = async () => {
    const errors = validateShelterData({
      name: newShelter.name,
      address: newShelter.address,
      latitude: parseFloat(newShelter.latitude),
      longitude: parseFloat(newShelter.longitude),
      phone: newShelter.phone,
      capacity: parseInt(newShelter.capacity)
    });

    if (errors.length > 0) {
      errors.forEach(error => toast.error(error));
      return;
    }

    setIsCreating(true);
    try {
      await createShelter({
        name: newShelter.name,
        address: newShelter.address,
        latitude: parseFloat(newShelter.latitude),
        longitude: parseFloat(newShelter.longitude),
        phone: newShelter.phone,
        capacity: parseInt(newShelter.capacity)
      });

      toast.success('Refugio creado correctamente');
      setIsCreateShelterOpen(false);
      setNewShelter({
        name: '',
        address: '',
        latitude: '',
        longitude: '',
        phone: '',
        capacity: ''
      });
      
      loadSheltersAndServices();
    } catch (error: any) {
      toast.error(error.message || 'Error al crear el refugio');
    } finally {
      setIsCreating(false);
    }
  };

  // ==================== EDITAR REFUGIO ====================
  const handleEditShelter = (shelter: any) => {
    setEditingShelter({
      id: shelter.id,
      name: shelter.name,
      address: shelter.address,
      latitude: shelter.latitude.toString(),
      longitude: shelter.longitude.toString(),
      phone: shelter.phone,
      capacity: shelter.capacity.toString()
    });
    setIsEditShelterOpen(true);
  };

  const handleUpdateShelter = async () => {
    if (!editingShelter) return;

    const errors = validateShelterData({
      name: editingShelter.name,
      address: editingShelter.address,
      latitude: parseFloat(editingShelter.latitude),
      longitude: parseFloat(editingShelter.longitude),
      phone: editingShelter.phone,
      capacity: parseInt(editingShelter.capacity)
    });

    if (errors.length > 0) {
      errors.forEach(error => toast.error(error));
      return;
    }

    setIsCreating(true);
    try {
      await updateShelter({
        id: editingShelter.id,
        name: editingShelter.name,
        address: editingShelter.address,
        latitude: parseFloat(editingShelter.latitude),
        longitude: parseFloat(editingShelter.longitude),
        phone: editingShelter.phone,
        capacity: parseInt(editingShelter.capacity)
      });

      toast.success('Refugio actualizado correctamente');
      setIsEditShelterOpen(false);
      setEditingShelter(null);
      loadSheltersAndServices();
    } catch (error: any) {
      toast.error(error.message || 'Error al actualizar el refugio');
    } finally {
      setIsCreating(false);
    }
  };

  // ==================== CREAR SERVICIO ====================
  const handleCreateService = async () => {
    const errors = validateServiceData({
      name: newService.name,
      description: newService.description,
      iconKey: newService.iconKey
    });

    if (errors.length > 0) {
      errors.forEach(error => toast.error(error));
      return;
    }

    setIsCreating(true);
    try {
      await createService({
        name: newService.name,
        description: newService.description,
        iconKey: newService.iconKey
      });

      toast.success('Servicio creado correctamente');
      setIsCreateServiceOpen(false);
      setNewService({
        name: '',
        description: '',
        iconKey: ''
      });
      
      loadSheltersAndServices();
    } catch (error: any) {
      toast.error(error.message || 'Error al crear el servicio');
    } finally {
      setIsCreating(false);
    }
  };

  // ==================== EDITAR SERVICIO ====================
  const handleEditService = (service: any) => {
    setEditingService({
      id: service.id,
      name: service.name,
      description: service.description,
      iconKey: service.iconKey
    });
    setIsEditServiceOpen(true);
  };

  const handleUpdateService = async () => {
    if (!editingService) return;

    const errors = validateServiceData({
      name: editingService.name,
      description: editingService.description,
      iconKey: editingService.iconKey
    });

    if (errors.length > 0) {
      errors.forEach(error => toast.error(error));
      return;
    }

    setIsCreating(true);
    try {
      await updateService({
        id: editingService.id,
        name: editingService.name,
        description: editingService.description,
        iconKey: editingService.iconKey
      });

      toast.success('Servicio actualizado correctamente');
      setIsEditServiceOpen(false);
      setEditingService(null);
      loadSheltersAndServices();
    } catch (error: any) {
      toast.error(error.message || 'Error al actualizar el servicio');
    } finally {
      setIsCreating(false);
    }
  };

  // ==================== ELIMINAR ====================
  const handleDeleteClick = (type: 'shelter' | 'service', id: number, name: string) => {
    setItemToDelete({ type, id, name });
    setDeleteConfirmOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!itemToDelete) return;

    setIsDeleting(true);
    try {
      if (itemToDelete.type === 'shelter') {
        await deleteShelter(itemToDelete.id);
        toast.success('Refugio eliminado correctamente');
      } else {
        await deleteService(itemToDelete.id);
        toast.success('Servicio eliminado correctamente');
      }
      
      setDeleteConfirmOpen(false);
      setItemToDelete(null);
      loadSheltersAndServices();
    } catch (error: any) {
      toast.error(error.message || 'Error al eliminar');
    } finally {
      setIsDeleting(false);
    }
  };

  const occupancyRate = stats.totalBeds > 0 ? Math.round((stats.occupiedBeds / stats.totalBeds) * 100) : 0;

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Loader2 className="h-8 w-8 animate-spin text-cyan-500" />
      </div>
    );
  }

  return (
    <div className="space-y-7">
      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-5">
        <Card className="border-2">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-3">
            <CardTitle>Total Usuarios</CardTitle>
            <Users className="h-6 w-6" style={{ color: '#06b6d4' }} />
          </CardHeader>
          <CardContent>
            <div className="text-3xl" style={{ color: '#06b6d4' }}>
              {totalUsers}
            </div>
            <div className="flex items-center gap-2 mt-2">
              <Badge variant="outline" className="px-2 py-1">
                <Shield className="h-3 w-3 mr-1" />
                {totalAdmins} Admins
              </Badge>
              <Badge variant="outline" className="px-2 py-1">
                <UserCheck className="h-3 w-3 mr-1" />
                {totalRegularUsers} Usuarios
              </Badge>
            </div>
          </CardContent>
        </Card>

        <Card className="border-2">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-3">
            <CardTitle>Usuarios Verificados</CardTitle>
            <CheckCircle className="h-6 w-6" style={{ color: '#06b6d4' }} />
          </CardHeader>
          <CardContent>
            <div className="text-3xl" style={{ color: '#06b6d4' }}>
              {verifiedUsers}
            </div>
            <p className="text-muted-foreground mt-1">
              {totalUsers - verifiedUsers} sin verificar
            </p>
          </CardContent>
        </Card>

        <Card className="border-2">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-3">
            <CardTitle>Reservas Activas</CardTitle>
            <Calendar className="h-6 w-6" style={{ color: '#06b6d4' }} />
          </CardHeader>
          <CardContent>
            <div className="text-3xl" style={{ color: '#06b6d4' }}>{stats.activeReservations}</div>
            <p className="text-muted-foreground mt-1">
              {stats.pendingConfirmations} pendientes de confirmación
            </p>
          </CardContent>
        </Card>

        <Card className="border-2">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-3">
            <CardTitle>Ocupación de Camas</CardTitle>
            <Bed className="h-6 w-6" style={{ color: '#06b6d4' }} />
          </CardHeader>
          <CardContent>
            <div className="text-3xl" style={{ color: '#06b6d4' }}>{stats.occupiedBeds}/{stats.totalBeds}</div>
            <Progress value={occupancyRate} className="mt-3 h-3" />
            <p className="text-muted-foreground mt-2">
              {occupancyRate}% ocupación
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Refugios */}
      <Card className="border-2">
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="flex items-center gap-2">
              <Home className="h-5 w-5" style={{ color: '#06b6d4' }} />
              Refugios Disponibles
            </CardTitle>
            <Dialog open={isCreateShelterOpen} onOpenChange={setIsCreateShelterOpen}>
              <DialogTrigger asChild>
                <Button size="sm">
                  <Plus className="h-4 w-4 mr-2" />
                  Agregar Refugio
                </Button>
              </DialogTrigger>
              <DialogContent className="max-w-md max-h-[90vh] overflow-y-auto">
                <DialogHeader>
                  <DialogTitle>Crear Nuevo Refugio</DialogTitle>
                </DialogHeader>
                <div className="space-y-4">
                  <div className="space-y-2">
                    <Label>Nombre del Refugio *</Label>
                    <Input
                      placeholder="Ej: Refugio San Juan"
                      value={newShelter.name}
                      onChange={(e) => setNewShelter({...newShelter, name: e.target.value})}
                    />
                  </div>

                  <div className="space-y-2">
                    <Label>Dirección *</Label>
                    <Textarea
                      placeholder="Dirección completa del refugio"
                      value={newShelter.address}
                      onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) =>
                          setNewShelter({ ...newShelter, address: e.target.value })
                        }
                        rows={2}
                      />
                  </div>

                  <div className="grid grid-cols-2 gap-4">
                    <div className="space-y-2">
                      <Label>Latitud *</Label>
                      <Input
                        type="number"
                        step="0.000001"
                        placeholder="Ej: 25.686613"
                        value={newShelter.latitude}
                        onChange={(e) => setNewShelter({...newShelter, latitude: e.target.value})}
                      />
                    </div>
                    <div className="space-y-2">
                      <Label>Longitud *</Label>
                      <Input
                        type="number"
                        step="0.000001"
                        placeholder="Ej: -100.316116"
                        value={newShelter.longitude}
                        onChange={(e) => setNewShelter({...newShelter, longitude: e.target.value})}
                      />
                    </div>
                  </div>

                  <div className="space-y-2">
                    <Label>Teléfono *</Label>
                    <Input
                      placeholder="Ej: 81-1234-5678"
                      value={newShelter.phone}
                      onChange={(e) => setNewShelter({...newShelter, phone: e.target.value})}
                    />
                  </div>

                  <div className="space-y-2">
                    <Label>Capacidad (personas) *</Label>
                    <Input
                      type="number"
                      min="1"
                      placeholder="Ej: 50"
                      value={newShelter.capacity}
                      onChange={(e) => setNewShelter({...newShelter, capacity: e.target.value})}
                    />
                  </div>

                  <div className="flex gap-2 pt-4">
                    <Button 
                      className="flex-1" 
                      onClick={handleCreateShelter}
                      disabled={isCreating}
                    >
                      {isCreating ? (
                        <>
                          <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                          Creando...
                        </>
                      ) : (
                        'Crear Refugio'
                      )}
                    </Button>
                    <Button 
                      variant="outline" 
                      onClick={() => setIsCreateShelterOpen(false)}
                      className="flex-1"
                      disabled={isCreating}
                    >
                      Cancelar
                    </Button>
                  </div>
                </div>
              </DialogContent>
            </Dialog>
          </div>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {shelters.length === 0 ? (
              <p className="text-muted-foreground col-span-full text-center py-8">
                No hay refugios registrados
              </p>
            ) : (
              shelters.map(shelter => {
                const shelterStats = shelterBedStats[shelter.id] || { total: 0, available: 0, occupied: 0, occupancyRate: 0 };
                
                return (
                  <Card key={shelter.id} className="border">
                    <CardHeader className="pb-3">
                      <div className="flex items-start justify-between gap-2">
                        <CardTitle className="text-lg line-clamp-1 flex-1">
                          {shelter.name}
                        </CardTitle>
                        <div className="flex gap-1">
                          <Button
                            size="sm"
                            variant="ghost"
                            className="h-8 w-8 p-0"
                            onClick={() => handleEditShelter(shelter)}
                          >
                            <Pencil className="h-4 w-4" />
                          </Button>
                          <Button
                            size="sm"
                            variant="ghost"
                            className="h-8 w-8 p-0 text-destructive hover:text-destructive"
                            onClick={() => handleDeleteClick('shelter', shelter.id, shelter.name)}
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                      </div>
                      {shelterStats.available === 0 ? (
                        <Badge variant="destructive">Lleno</Badge>
                      ) : shelterStats.available <= 3 ? (
                        <Badge variant="secondary">Poco espacio</Badge>
                      ) : (
                        <Badge style={{ backgroundColor: '#06b6d4', color: 'white' }}>
                          Disponible
                        </Badge>
                      )}
                    </CardHeader>
                    <CardContent className="space-y-3">
                      <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <MapPin className="h-4 w-4 flex-shrink-0" />
                        <span className="line-clamp-1">{shelter.address}</span>
                      </div>
                      <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <Phone className="h-4 w-4 flex-shrink-0" />
                        <span>{shelter.phone}</span>
                      </div>
                      
                      <div className="pt-2 space-y-2">
                        <div className="flex justify-between text-sm">
                          <span>Capacidad:</span>
                          <span className="font-medium">{shelter.capacity} personas</span>
                        </div>
                        <div className="flex justify-between text-sm">
                          <span>Camas totales:</span>
                          <span className="font-medium">{shelterStats.total}</span>
                        </div>
                        <div className="flex justify-between text-sm">
                          <span>Disponibles:</span>
                          <span className="font-medium" style={{ color: '#06b6d4' }}>
                            {shelterStats.available}
                          </span>
                        </div>
                        <div className="flex justify-between text-sm">
                          <span>Ocupadas:</span>
                          <span className="font-medium text-red-600">
                            {shelterStats.occupied}
                          </span>
                        </div>
                        <Progress value={shelterStats.occupancyRate} className="h-2" />
                        <p className="text-xs text-muted-foreground text-center">
                          {shelterStats.occupancyRate}% ocupación
                        </p>
                      </div>
                    </CardContent>
                  </Card>
                );
              })
            )}
          </div>
        </CardContent>
      </Card>

      {/* Diálogo Editar Refugio */}
      <Dialog open={isEditShelterOpen} onOpenChange={setIsEditShelterOpen}>
        <DialogContent className="max-w-md max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Editar Refugio</DialogTitle>
          </DialogHeader>
          {editingShelter && (
            <div className="space-y-4">
              <div className="space-y-2">
                <Label>Nombre del Refugio *</Label>
                <Input
                  placeholder="Ej: Refugio San Juan"
                  value={editingShelter.name}
                  onChange={(e) => setEditingShelter({...editingShelter, name: e.target.value})}
                />
              </div>

              <div className="space-y-2">
                <Label>Dirección *</Label>
                <Textarea
                  placeholder="Dirección completa del refugio"
                  value={editingShelter.address}
                  onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) =>
                      setEditingShelter({ ...editingShelter, address: e.target.value })
                    }
                    rows={2}
                  />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label>Latitud *</Label>
                  <Input
                    type="number"
                    step="0.000001"
                    placeholder="Ej: 25.686613"
                    value={editingShelter.latitude}
                    onChange={(e) => setEditingShelter({...editingShelter, latitude: e.target.value})}
                  />
                </div>
                <div className="space-y-2">
                  <Label>Longitud *</Label>
                  <Input
                    type="number"
                    step="0.000001"
                    placeholder="Ej: -100.316116"
                    value={editingShelter.longitude}
                    onChange={(e) => setEditingShelter({...editingShelter, longitude: e.target.value})}
                  />
                </div>
              </div>

              <div className="space-y-2">
                <Label>Teléfono *</Label>
                <Input
                  placeholder="Ej: 81-1234-5678"
                  value={editingShelter.phone}
                  onChange={(e) => setEditingShelter({...editingShelter, phone: e.target.value})}
                />
              </div>

              <div className="space-y-2">
                <Label>Capacidad (personas) *</Label>
                <Input
                  type="number"
                  min="1"
                  placeholder="Ej: 50"
                  value={editingShelter.capacity}
                  onChange={(e) => setEditingShelter({...editingShelter, capacity: e.target.value})}
                />
              </div>

              <div className="flex gap-2 pt-4">
                <Button 
                  className="flex-1" 
                  onClick={handleUpdateShelter}
                  disabled={isCreating}
                >
                  {isCreating ? (
                    <>
                      <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                      Actualizando...
                    </>
                  ) : (
                    'Actualizar Refugio'
                  )}
                </Button>
                <Button 
                  variant="outline" 
                  onClick={() => setIsEditShelterOpen(false)}
                  className="flex-1"
                  disabled={isCreating}
                >
                  Cancelar
                </Button>
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>

      {/* Servicios */}
      <Card className="border-2">
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="flex items-center gap-2">
              <Package className="h-5 w-5" style={{ color: '#06b6d4' }} />
              Servicios Disponibles
            </CardTitle>
            <Dialog open={isCreateServiceOpen} onOpenChange={setIsCreateServiceOpen}>
              <DialogTrigger asChild>
                <Button size="sm">
                  <Plus className="h-4 w-4 mr-2" />
                  Agregar Servicio
                </Button>
              </DialogTrigger>
              <DialogContent className="max-w-md max-h-[90vh] overflow-y-auto">
                <DialogHeader>
                  <DialogTitle>Crear Nuevo Servicio</DialogTitle>
                </DialogHeader>
                <div className="space-y-4">
                  <div className="space-y-2">
                    <Label>Nombre del Servicio *</Label>
                    <Input
                      placeholder="Ej: Hospedaje, Comida, Lavandería"
                      value={newService.name}
                      onChange={(e) => setNewService({...newService, name: e.target.value})}
                      maxLength={100}
                    />
                  </div>

                  <div className="space-y-2">
                    <Label>Descripción *</Label>
                    <Textarea
                      placeholder="Describe el servicio que se ofrece"
                      value={newService.description}
                      onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) => setNewService({...newService, description: e.target.value})}
                      rows={3}
                      maxLength={500}
                    />
                    <p className="text-xs text-muted-foreground">
                      {newService.description.length}/500 caracteres
                    </p>
                  </div>

                  <div className="space-y-2">
                    <Label>Icono *</Label>
                    <Select value={newService.iconKey} onValueChange={(v: string) => setNewService({...newService, iconKey: v})}>
                      <SelectTrigger>
                        <SelectValue placeholder="Seleccionar icono" />
                      </SelectTrigger>
                      <SelectContent className="max-h-[300px]">
                        {availableIcons.map(icon => {
                          const IconComponent = getIconComponent(icon.key);
                          return (
                            <SelectItem key={icon.key} value={icon.key}>
                              <div className="flex items-center gap-2">
                                <IconComponent className="h-4 w-4" />
                                <span>{icon.label}</span>
                              </div>
                            </SelectItem>
                          );
                        })}
                      </SelectContent>
                    </Select>
                  </div>

                  {newService.iconKey && (
                    <div className="p-4 border rounded-lg bg-muted/50">
                      <p className="text-sm text-muted-foreground mb-2">Vista previa:</p>
                      <div className="flex items-center gap-3">
                        {(() => {
                          const IconComponent = getIconComponent(newService.iconKey);
                          return <IconComponent className="h-8 w-8" style={{ color: '#06b6d4' }} />;
                        })()}
                        <div>
                          <p className="font-medium">{newService.name || 'Nombre del servicio'}</p>
                          <p className="text-sm text-muted-foreground line-clamp-2">
                            {newService.description || 'Descripción del servicio'}
                          </p>
                        </div>
                      </div>
                    </div>
                  )}

                  <div className="flex gap-2 pt-4">
                    <Button 
                      className="flex-1" 
                      onClick={handleCreateService}
                      disabled={isCreating}
                    >
                      {isCreating ? (
                        <>
                          <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                          Creando...
                        </>
                      ) : (
                        'Crear Servicio'
                      )}
                    </Button>
                    <Button 
                      variant="outline" 
                      onClick={() => setIsCreateServiceOpen(false)}
                      className="flex-1"
                      disabled={isCreating}
                    >
                      Cancelar
                    </Button>
                  </div>
                </div>
              </DialogContent>
            </Dialog>
          </div>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
            {services.length === 0 ? (
              <p className="text-muted-foreground col-span-full text-center py-8">
                No hay servicios registrados
              </p>
            ) : (
              services.map(service => {
                const IconComponent = getIconComponent(service.iconKey);
                
                return (
                  <Card key={service.id} className="border hover:border-cyan-500 transition-colors relative group">
                    <div className="absolute top-2 right-2 flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity z-10">
                      <Button
                        size="sm"
                        variant="ghost"
                        className="h-8 w-8 p-0 bg-background shadow-sm"
                        onClick={() => handleEditService(service)}
                      >
                        <Pencil className="h-4 w-4" />
                      </Button>
                      <Button
                        size="sm"
                        variant="ghost"
                        className="h-8 w-8 p-0 text-destructive hover:text-destructive bg-background shadow-sm"
                        onClick={() => handleDeleteClick('service', service.id, service.name)}
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                    <CardContent className="pt-6">
                      <div className="flex flex-col items-center text-center space-y-3">
                        <div className="p-3 rounded-full bg-cyan-50">
                          <IconComponent className="h-8 w-8" style={{ color: '#06b6d4' }} />
                        </div>
                        <h3 className="font-semibold">{service.name}</h3>
                        <p className="text-sm text-muted-foreground line-clamp-2">
                          {service.description}
                        </p>
                      </div>
                    </CardContent>
                  </Card>
                );
              })
            )}
          </div>
        </CardContent>
      </Card>

      {/* Diálogo Editar Servicio */}
      <Dialog open={isEditServiceOpen} onOpenChange={setIsEditServiceOpen}>
        <DialogContent className="max-w-md max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Editar Servicio</DialogTitle>
          </DialogHeader>
          {editingService && (
            <div className="space-y-4">
              <div className="space-y-2">
                <Label>Nombre del Servicio *</Label>
                <Input
                  placeholder="Ej: Hospedaje, Comida, Lavandería"
                  value={editingService.name}
                  onChange={(e) => setEditingService({...editingService, name: e.target.value})}
                  maxLength={100}
                />
              </div>

              <div className="space-y-2">
                <Label>Descripción *</Label>
                <Textarea
                  placeholder="Describe el servicio que se ofrece"
                  value={editingService.description}
                  onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) => setEditingService({...editingService, description: e.target.value})}
                  rows={3}
                  maxLength={500}
                />
                <p className="text-xs text-muted-foreground">
                  {editingService.description.length}/500 caracteres
                </p>
              </div>

              <div className="space-y-2">
                <Label>Icono *</Label>
                <Select value={editingService.iconKey} onValueChange={(v: string) => setEditingService({...editingService, iconKey: v})}>
                  <SelectTrigger>
                    <SelectValue placeholder="Seleccionar icono" />
                  </SelectTrigger>
                  <SelectContent className="max-h-[300px]">
                    {availableIcons.map(icon => {
                      const IconComponent = getIconComponent(icon.key);
                      return (
                        <SelectItem key={icon.key} value={icon.key}>
                          <div className="flex items-center gap-2">
                            <IconComponent className="h-4 w-4" />
                            <span>{icon.label}</span>
                          </div>
                        </SelectItem>
                      );
                    })}
                  </SelectContent>
                </Select>
              </div>

              {editingService.iconKey && (
                <div className="p-4 border rounded-lg bg-muted/50">
                  <p className="text-sm text-muted-foreground mb-2">Vista previa:</p>
                  <div className="flex items-center gap-3">
                    {(() => {
                      const IconComponent = getIconComponent(editingService.iconKey);
                      return <IconComponent className="h-8 w-8" style={{ color: '#06b6d4' }} />;
                    })()}
                    <div>
                      <p className="font-medium">{editingService.name || 'Nombre del servicio'}</p>
                      <p className="text-sm text-muted-foreground line-clamp-2">
                        {editingService.description || 'Descripción del servicio'}
                      </p>
                    </div>
                  </div>
                </div>
              )}

              <div className="flex gap-2 pt-4">
                <Button 
                  className="flex-1" 
                  onClick={handleUpdateService}
                  disabled={isCreating}
                >
                  {isCreating ? (
                    <>
                      <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                      Actualizando...
                    </>
                  ) : (
                    'Actualizar Servicio'
                  )}
                </Button>
                <Button 
                  variant="outline" 
                  onClick={() => setIsEditServiceOpen(false)}
                  className="flex-1"
                  disabled={isCreating}
                >
                  Cancelar
                </Button>
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>

      {/* Diálogo de Confirmación de Eliminación */}
      <AlertDialog open={deleteConfirmOpen} onOpenChange={setDeleteConfirmOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>¿Estás seguro?</AlertDialogTitle>
            <AlertDialogDescription>
              Esta acción no se puede deshacer. Esto eliminará permanentemente{' '}
              {itemToDelete?.type === 'shelter' ? 'el refugio' : 'el servicio'}{' '}
              <strong>{itemToDelete?.name}</strong>
              {itemToDelete?.type === 'shelter' && ' y todos sus datos asociados'}.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={isDeleting}>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleConfirmDelete}
              disabled={isDeleting}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {isDeleting ? (
                <>
                  <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                  Eliminando...
                </>
              ) : (
                'Eliminar'
              )}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Uso de Servicios */}
      <Card className="border-2">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <span style={{ color: '#06b6d4' }}>●</span>
            Uso de Servicios de la Posada del Peregrino
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-7">
            {serviceStats.length === 0 ? (
              <p className="text-muted-foreground text-center py-8">
                No hay estadísticas de servicios disponibles
              </p>
            ) : (
              serviceStats.map((serviceStat) => {
                const usagePercent = serviceStat.total > 0 
                  ? Math.round((serviceStat.occupied / serviceStat.total) * 100) 
                  : 0;
                
                return (
                  <div key={serviceStat.serviceId} className="space-y-3">
                    <div className="flex items-center justify-between">
                      <h4 className="font-semibold">{serviceStat.service}</h4>
                      <div className="flex gap-3">
                        <Badge variant="outline" className="px-4 py-1">
                          {serviceStat.occupied}/{serviceStat.total}
                        </Badge>
                        {serviceStat.available === 0 ? (
                          <Badge variant="destructive" className="px-3 py-1">Completo</Badge>
                        ) : serviceStat.available <= 3 ? (
                          <Badge variant="secondary" className="px-3 py-1">
                            <AlertCircle className="h-4 w-4 mr-1" />
                            Poca capacidad
                          </Badge>
                        ) : (
                          <Badge className="px-3 py-1" style={{ backgroundColor: '#06b6d4', color: 'white' }}>
                            <CheckCircle className="h-4 w-4 mr-1" />
                            Disponible
                          </Badge>
                        )}
                      </div>
                    </div>
                    
                    <Progress 
                      value={usagePercent} 
                      className="h-4"
                    />
                    
                    <div className="grid grid-cols-2 gap-5">
                      <div className="space-y-1">
                        <p className="text-sm text-muted-foreground">Ocupados</p>
                        <p className="text-2xl font-bold text-red-600">{serviceStat.occupied}</p>
                      </div>
                      <div className="space-y-1">
                        <p className="text-sm text-muted-foreground">Disponibles</p>
                        <p className="text-2xl font-bold" style={{ color: '#06b6d4' }}>
                          {serviceStat.available}
                        </p>
                      </div>
                    </div>
                  </div>
                );
              })
            )}
          </div>
        </CardContent>
      </Card>

      {/* Alertas */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-5">
        <Card className="border-2">
          <CardHeader>
            <CardTitle>Confirmaciones Pendientes</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {Array.from({ length: stats.pendingConfirmations }, (_, i) => (
                <div key={i} className="flex items-center justify-between p-4 border-2 rounded-lg">
                  <div>
                    <p>Reserva #{1001 + i}</p>
                    <p className="text-muted-foreground">
                      Peregrino {i + 1}
                    </p>
                  </div>
                  <Badge variant="outline" className="px-3 py-1">Pendiente QR</Badge>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
        <Card className="border-2">
          <CardHeader>
            <CardTitle>Próximos Vencimientos de Hospedaje</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <div className="flex items-center justify-between p-4 border-2 rounded-lg">
                <div>
                  <p>Cama 12 - Hospedaje</p>
                  <p className="text-muted-foreground">
                    Vence en 2 horas
                  </p>
                </div>
                <Badge variant="secondary" className="px-3 py-1">2h</Badge>
              </div>
              <div className="flex items-center justify-between p-4 border-2 rounded-lg">
                <div>
                  <p>Cama 25 - Hospedaje</p>
                  <p className="text-muted-foreground">
                    Vence mañana
                  </p>
                </div>
                <Badge className="px-3 py-1" style={{ backgroundColor: '#06b6d4', color: 'white' }}>1d</Badge>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}