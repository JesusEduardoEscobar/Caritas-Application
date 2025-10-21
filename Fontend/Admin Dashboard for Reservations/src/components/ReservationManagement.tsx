import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Calendar, Search, Plus, QrCode, Check, X, Clock, AlertCircle, Loader2 } from 'lucide-react';
import { toast } from 'sonner';

// Importar servicios existentes
import { getBeds } from '../Services/authBeds';
import { getAllShelters } from '../Services/authShelter';
import { getAllServices } from '../Services/authServices';
import { getUsers } from '../Services/authUser';

// Mock data para reservas hasta que tengas el endpoint
const mockReservationData = [
  { 
    id: 1001, 
    pilgrim: 'Peregrino A', 
    bedNumber: '101',
    service: 'Hospedaje',
    checkIn: '2024-01-15',
    checkOut: '2024-01-18',
    status: 'confirmed',
    confirmationMethod: 'qr',
    createdAt: '2024-01-14 10:30'
  },
  { 
    id: 1002, 
    pilgrim: 'Peregrino B', 
    bedNumber: '201',
    service: 'Hospedaje',
    checkIn: '2024-01-14',
    checkOut: '2024-01-20',
    status: 'pending',
    confirmationMethod: 'pending',
    createdAt: '2024-01-14 09:15'
  },
  { 
    id: 1003, 
    pilgrim: 'Peregrino C', 
    bedNumber: '202',
    service: 'Comida',
    checkIn: '2024-01-16',
    checkOut: '2024-01-19',
    status: 'pending',
    confirmationMethod: 'pending',
    createdAt: '2024-01-14 14:20'
  },
  { 
    id: 1004, 
    pilgrim: 'Peregrino D', 
    bedNumber: '1',
    service: 'Regaderas',
    checkIn: '2024-01-17',
    checkOut: '2024-01-22',
    status: 'cancelled',
    confirmationMethod: 'manual',
    createdAt: '2024-01-13 16:45'
  }
];

const statusLabels = {
  pending: 'Pendiente',
  confirmed: 'Confirmada',
  cancelled: 'Cancelada',
  completed: 'Completada'
};

const statusColors = {
  pending: 'secondary',
  confirmed: 'default',
  cancelled: 'destructive',
  completed: 'outline'
} as const;

export function ReservationManagement() {
  const [reservations, setReservations] = useState(mockReservationData);
  const [beds, setBeds] = useState<any[]>([]);
  const [shelters, setShelters] = useState<any[]>([]);
  const [services, setServices] = useState<any[]>([]);
  const [users, setUsers] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  const [searchTerm, setSearchTerm] = useState('');
  const [selectedService, setSelectedService] = useState('Todos');
  const [selectedStatus, setSelectedStatus] = useState('Todos');
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);

  const [newReservation, setNewReservation] = useState({
    userId: '',
    shelterId: '',
    serviceId: '',
    bedId: '',
    checkIn: '',
    checkOut: ''
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setIsLoading(true);
    try {
      const [bedsData, sheltersData, servicesData, usersData] = await Promise.all([
        getBeds(),
        getAllShelters(),
        getAllServices(),
        getUsers()
      ]);

      setBeds(bedsData);
      setShelters(sheltersData);
      setServices(servicesData);
      setUsers(usersData);

      toast.success('Datos cargados correctamente');
    } catch (error) {
      console.error('Error al cargar datos:', error);
      toast.error('Error al cargar los datos');
    } finally {
      setIsLoading(false);
    }
  };

  const filteredReservations = reservations.filter(reservation => {
    const matchesSearch = reservation.pilgrim.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         reservation.bedNumber.includes(searchTerm) ||
                         reservation.id.toString().includes(searchTerm);
    const matchesService = selectedService === 'Todos' || reservation.service === selectedService;
    const matchesStatus = selectedStatus === 'Todos' || reservation.status === selectedStatus;
    
    return matchesSearch && matchesService && matchesStatus;
  });

  const getStatusBadge = (status: string) => {
    return (
      <Badge variant={statusColors[status as keyof typeof statusColors]}>
        {statusLabels[status as keyof typeof statusLabels]}
      </Badge>
    );
  };

  const getConfirmationIcon = (method: string, status: string) => {
    if (status === 'confirmed') {
      return method === 'qr' ? (
        <QrCode className="h-4 w-4 text-green-600" />
      ) : (
        <Check className="h-4 w-4 text-green-600" />
      );
    }
    if (status === 'pending') {
      return <Clock className="h-4 w-4 text-amber-600" />;
    }
    if (status === 'cancelled') {
      return <X className="h-4 w-4 text-red-600" />;
    }
    return <AlertCircle className="h-4 w-4 text-gray-600" />;
  };

  const handleConfirmReservation = (id: number) => {
    toast.success(`Reserva #${id} confirmada`);
    setReservations(prev => prev.map(r => 
      r.id === id ? { ...r, status: 'confirmed' as const, confirmationMethod: 'manual' as const } : r
    ));
  };

  const handleCancelReservation = (id: number) => {
    toast.success(`Reserva #${id} cancelada`);
    setReservations(prev => prev.map(r => 
      r.id === id ? { ...r, status: 'cancelled' as const } : r
    ));
  };

  const handleCreateReservation = async () => {
    toast.success('Reserva creada correctamente');
    setIsCreateDialogOpen(false);
    setNewReservation({
      userId: '',
      shelterId: '',
      serviceId: '',
      bedId: '',
      checkIn: '',
      checkOut: ''
    });
  };

  const availableBeds = beds.filter(bed => 
    bed.isAvailable && bed.shelterId.toString() === newReservation.shelterId
  );

  const uniqueServices = ['Todos', ...new Set(services.map(s => s.name))];

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Loader2 className="h-8 w-8 animate-spin text-cyan-500" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <h2 className="text-2xl">Gestión de Reservas</h2>
        <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
          <DialogTrigger asChild>
            <Button>
              <Plus className="h-4 w-4 mr-2" />
              Nueva Reserva
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Crear Nueva Reserva</DialogTitle>
            </DialogHeader>
            <div className="space-y-4">
              <div className="space-y-2">
                <Label>Usuario/Peregrino</Label>
                <Select value={newReservation.userId} onValueChange={(v: string) => setNewReservation({...newReservation, userId: v})}>
                  <SelectTrigger>
                    <SelectValue placeholder="Seleccionar usuario" />
                  </SelectTrigger>
                  <SelectContent>
                    {users.map(user => (
                      <SelectItem key={user.id} value={user.id.toString()}>
                        {user.name || user.email}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label>Refugio</Label>
                <Select value={newReservation.shelterId} onValueChange={(v: string) => setNewReservation({...newReservation, shelterId: v, bedId: ''})}>
                  <SelectTrigger>
                    <SelectValue placeholder="Seleccionar refugio" />
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
              
              <div className="space-y-2">
                <Label>Servicio</Label>
                <Select value={newReservation.serviceId} onValueChange={(v: string) => setNewReservation({...newReservation, serviceId: v})}>
                  <SelectTrigger>
                    <SelectValue placeholder="Seleccionar servicio" />
                  </SelectTrigger>
                  <SelectContent>
                    {services.map(service => (
                      <SelectItem key={service.id} value={service.id.toString()}>
                        {service.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              {newReservation.shelterId && (
                <div className="space-y-2">
                  <Label>Cama disponible</Label>
                  <Select value={newReservation.bedId} onValueChange={(v: string) => setNewReservation({...newReservation, bedId: v})}>
                    <SelectTrigger>
                      <SelectValue placeholder="Seleccionar cama" />
                    </SelectTrigger>
                    <SelectContent>
                      {availableBeds.length > 0 ? (
                        availableBeds.map(bed => (
                          <SelectItem key={bed.id} value={bed.id.toString()}>
                            {bed.bedNumber}
                          </SelectItem>
                        ))
                      ) : (
                        <SelectItem value="none" disabled>No hay camas disponibles</SelectItem>
                      )}
                    </SelectContent>
                  </Select>
                </div>
              )}
              
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label>Fecha Ingreso</Label>
                  <Input 
                    type="date" 
                    value={newReservation.checkIn}
                    onChange={(e) => setNewReservation({...newReservation, checkIn: e.target.value})}
                  />
                </div>
                <div className="space-y-2">
                  <Label>Fecha Salida</Label>
                  <Input 
                    type="date" 
                    value={newReservation.checkOut}
                    onChange={(e) => setNewReservation({...newReservation, checkOut: e.target.value})}
                  />
                </div>
              </div>
              
              <div className="flex gap-2 pt-4">
                <Button className="flex-1" onClick={handleCreateReservation}>
                  Crear Reserva
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
            <Calendar className="h-5 w-5" />
            Filtros de Reserva
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="space-y-2">
              <Label>Buscar</Label>
              <div className="relative">
                <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
                <Input
                  placeholder="Peregrino, cama o ID..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10"
                />
              </div>
            </div>
            <div className="space-y-2">
              <Label>Servicio</Label>
              <Select value={selectedService} onValueChange={setSelectedService}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {uniqueServices.map(service => (
                    <SelectItem key={service} value={service}>{service}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>Estado</Label>
              <Select value={selectedStatus} onValueChange={setSelectedStatus}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Todos">Todos los estados</SelectItem>
                  {Object.entries(statusLabels).map(([key, label]) => (
                    <SelectItem key={key} value={key}>
                      {label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Lista de Reservas ({filteredReservations.length})</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ID</TableHead>
                <TableHead>Peregrino</TableHead>
                <TableHead>Recurso</TableHead>
                <TableHead>Servicio</TableHead>
                <TableHead>Fechas</TableHead>
                <TableHead>Estado</TableHead>
                <TableHead>Confirmación</TableHead>
                <TableHead>Acciones</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredReservations.map(reservation => (
                <TableRow key={reservation.id}>
                  <TableCell>#{reservation.id}</TableCell>
                  <TableCell>{reservation.pilgrim}</TableCell>
                  <TableCell>
                    <Badge variant="outline">
                      {reservation.service === 'Hospedaje' ? `Cama ${reservation.bedNumber}` : `#${reservation.bedNumber}`}
                    </Badge>
                  </TableCell>
                  <TableCell>{reservation.service}</TableCell>
                  <TableCell>
                    <div className="text-sm">
                      <div>Entrada: {reservation.checkIn}</div>
                      <div>Salida: {reservation.checkOut}</div>
                    </div>
                  </TableCell>
                  <TableCell>
                    {getStatusBadge(reservation.status)}
                  </TableCell>
                  <TableCell>
                    <div className="flex items-center gap-2">
                      {getConfirmationIcon(reservation.confirmationMethod, reservation.status)}
                      <span className="text-xs">
                        {reservation.confirmationMethod === 'qr' ? 'QR' : 
                         reservation.confirmationMethod === 'manual' ? 'Manual' : 'Pendiente'}
                      </span>
                    </div>
                  </TableCell>
                  <TableCell>
                    <div className="flex gap-1">
                      {reservation.status === 'pending' && (
                        <>
                          <Button 
                            variant="ghost" 
                            size="sm"
                            onClick={() => handleConfirmReservation(reservation.id)}
                          >
                            <Check className="h-4 w-4" />
                          </Button>
                          <Button 
                            variant="ghost" 
                            size="sm"
                            onClick={() => handleCancelReservation(reservation.id)}
                          >
                            <X className="h-4 w-4" />
                          </Button>
                        </>
                      )}
                      {reservation.status === 'pending' && (
                        <Button variant="ghost" size="sm">
                          <QrCode className="h-4 w-4" />
                        </Button>
                      )}
                    </div>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Resumen de Confirmaciones QR</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="text-center p-4 border rounded-lg">
              <Clock className="h-8 w-8 mx-auto mb-2 text-amber-600" />
              <div className="text-2xl">{reservations.filter(r => r.status === 'pending').length}</div>
              <p className="text-sm text-muted-foreground">Pendientes de QR</p>
            </div>
            <div className="text-center p-4 border rounded-lg">
              <QrCode className="h-8 w-8 mx-auto mb-2 text-green-600" />
              <div className="text-2xl">{reservations.filter(r => r.confirmationMethod === 'qr').length}</div>
              <p className="text-sm text-muted-foreground">Confirmadas por QR</p>
            </div>
            <div className="text-center p-4 border rounded-lg">
              <Check className="h-8 w-8 mx-auto mb-2 text-blue-600" />
              <div className="text-2xl">{reservations.filter(r => r.status === 'confirmed').length}</div>
              <p className="text-sm text-muted-foreground">Total Confirmadas</p>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}