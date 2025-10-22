import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Calendar, Search, Check, X, QrCode, Clock, Plus } from 'lucide-react';
import { 
  getAllReservations, 
  getReservationsByShelter,
  updateReservationStatus,
  deleteReservation,
  filterReservations,
  getStatusLabel,
  getStatusColor,
  formatReservationDate,
  getReservationStats,
  type Reservation,
  type ReservationFilters
} from '../Services/authReservations';
import { getAllServices } from '../Services/authServices';
import { getAllShelters } from '../Services/authShelter';
import type { Service } from '../types/models.ts';
import type { Shelter } from '../types/models.ts';
import { useAuth } from './auth/AuthProvider';
import { toast } from 'sonner';

export function ReservationsManagement() {
  const { admin } = useAuth();
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [services, setServices] = useState<Service[]>([]);
  const [shelters, setShelters] = useState<Shelter[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  // Filtros
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedService, setSelectedService] = useState('all');
  const [selectedStatus, setSelectedStatus] = useState('all');

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setIsLoading(true);
    try {
      await Promise.all([
        loadReservations(),
        loadServices(),
        loadShelters()
      ]);
    } catch (error) {
      console.error('Error al cargar datos:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const loadReservations = async () => {
    try {
      let data: Reservation[];
      
      if (admin?.shelter_id) {
        const shelterId = admin.shelter_id;
        data = await getReservationsByShelter(shelterId);
      } else {
        data = await getAllReservations();
      }
      
      setReservations(data);
    } catch (error: any) {
      console.error('Error al cargar reservas:', error);
      toast.error(error.message || 'Error al cargar las reservas');
    }
  };

  const loadServices = async () => {
    try {
      const data = await getAllServices();
      setServices(data);
    } catch (error) {
      console.error('Error al cargar servicios:', error);
    }
  };

  const loadShelters = async () => {
    try {
      const data = await getAllShelters();
      setShelters(data);
    } catch (error) {
      console.error('Error al cargar refugios:', error);
    }
  };

  const handleStatusChange = async (reservationId: number, newStatus: any) => {
    try {
      await updateReservationStatus(reservationId, newStatus);
      toast.success('Estado actualizado correctamente');
      await loadReservations();
    } catch (error: any) {
      console.error('Error al actualizar estado:', error);
      toast.error(error.message || 'Error al actualizar el estado');
    }
  };

  const handleDeleteReservation = async (id: number) => {
    if (!window.confirm('¿Está seguro de eliminar esta reserva?')) {
      return;
    }

    try {
      await deleteReservation(id);
      toast.success('Reserva eliminada exitosamente');
      await loadReservations();
    } catch (error: any) {
      console.error('Error al eliminar reserva:', error);
      toast.error(error.message || 'Error al eliminar la reserva');
    }
  };

  // Aplicar filtros
  const filters: ReservationFilters = {
    search: searchTerm,
    status: selectedStatus !== 'all' ? selectedStatus : undefined,
    serviceId: selectedService !== 'all' ? parseInt(selectedService) : undefined
  };

  const filteredReservations = filterReservations(reservations, filters);
  const stats = getReservationStats(filteredReservations);

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <h2 className="text-2xl font-bold">Gestión de Reservas</h2>
        <Button className="bg-cyan-600 hover:bg-cyan-700">
          <Plus className="h-4 w-4 mr-2" />
          Nueva Reserva
        </Button>
      </div>

      {/* Filtros */}
      <Card className="border-2">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Calendar className="h-6 w-6 text-cyan-500" />
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
                  <SelectValue placeholder="Todos" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">Todos</SelectItem>
                  {services.map(service => (
                    <SelectItem key={service.id} value={service.id.toString()}>
                      {service.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <Label>Estado</Label>
              <Select value={selectedStatus} onValueChange={setSelectedStatus}>
                <SelectTrigger>
                  <SelectValue placeholder="Todos los estados" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">Todos los estados</SelectItem>
                  <SelectItem value="pending">Pendiente</SelectItem>
                  <SelectItem value="confirmed">Confirmada</SelectItem>
                  <SelectItem value="cancelled">Cancelada</SelectItem>
                  <SelectItem value="completed">Completada</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          {/* Estadísticas */}
          <div className="grid grid-cols-2 md:grid-cols-5 gap-4 mt-6 pt-6 border-t">
            <div className="text-center">
              <p className="text-sm text-muted-foreground">Total</p>
              <p className="text-2xl font-bold text-cyan-500">{stats.total}</p>
            </div>
            <div className="text-center">
              <p className="text-sm text-muted-foreground">Pendientes</p>
              <p className="text-2xl font-bold text-yellow-500">{stats.pending}</p>
            </div>
            <div className="text-center">
              <p className="text-sm text-muted-foreground">Confirmadas</p>
              <p className="text-2xl font-bold text-green-500">{stats.confirmed}</p>
            </div>
            <div className="text-center">
              <p className="text-sm text-muted-foreground">Activas</p>
              <p className="text-2xl font-bold text-blue-500">{stats.active}</p>
            </div>
            <div className="text-center">
              <p className="text-sm text-muted-foreground">Canceladas</p>
              <p className="text-2xl font-bold text-red-500">{stats.cancelled}</p>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Lista de Reservas */}
      <Card className="border-2">
        <CardHeader>
          <CardTitle>
            Lista de Reservas ({filteredReservations.length})
          </CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="text-center py-10">
              <p className="text-muted-foreground">Cargando reservas...</p>
            </div>
          ) : filteredReservations.length === 0 ? (
            <div className="text-center py-10">
              <p className="text-muted-foreground">No hay reservas que mostrar</p>
            </div>
          ) : (
            <div className="overflow-x-auto">
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
                      <TableCell className="font-medium">
                        #{reservation.id}
                      </TableCell>
                      <TableCell>
                        <div>
                          <p className="font-medium">{reservation.user?.name || 'N/A'}</p>
                          <p className="text-xs text-muted-foreground">
                            {reservation.user?.email}
                          </p>
                        </div>
                      </TableCell>
                      <TableCell>
                        {reservation.bed?.bedNumber || 'N/A'}
                      </TableCell>
                      <TableCell>
                        {reservation.service?.name || reservation.shelter?.name || 'N/A'}
                      </TableCell>
                      <TableCell>
                        <div className="text-sm">
                          <p>Entrada: {formatReservationDate(reservation.checkInDate)}</p>
                          <p>Salida: {formatReservationDate(reservation.checkOutDate)}</p>
                        </div>
                      </TableCell>
                      <TableCell>
                        <Badge className={getStatusColor(reservation.status)}>
                          {getStatusLabel(reservation.status)}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        {reservation.status === 'confirmed' && reservation.confirmationCode ? (
                          <div className="flex items-center gap-2">
                            <QrCode className="h-4 w-4 text-green-600" />
                            <span className="text-xs">{reservation.confirmationCode}</span>
                          </div>
                        ) : reservation.status === 'pending' ? (
                          <div className="flex items-center gap-2 text-yellow-600">
                            <Clock className="h-4 w-4" />
                            <span className="text-xs">Pendiente</span>
                          </div>
                        ) : reservation.status === 'cancelled' ? (
                          <span className="text-xs text-red-600">Manual</span>
                        ) : null}
                      </TableCell>
                      <TableCell>
                        <div className="flex gap-1">
                          {reservation.status === 'pending' && (
                            <Button
                              variant="ghost"
                              size="icon"
                              title="Confirmar"
                              onClick={() => handleStatusChange(reservation.id, 'confirmed')}
                            >
                              <Check className="h-4 w-4 text-green-600" />
                            </Button>
                          )}
                          {(reservation.status === 'pending' || reservation.status === 'confirmed') && (
                            <Button
                              variant="ghost"
                              size="icon"
                              title="Cancelar"
                              onClick={() => handleStatusChange(reservation.id, 'cancelled')}
                            >
                              <X className="h-4 w-4 text-red-600" />
                            </Button>
                          )}
                          {reservation.confirmationCode && (
                            <Button
                              variant="ghost"
                              size="icon"
                              title="Ver QR"
                            >
                              <QrCode className="h-4 w-4" />
                            </Button>
                          )}
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
    </div>
  );
}