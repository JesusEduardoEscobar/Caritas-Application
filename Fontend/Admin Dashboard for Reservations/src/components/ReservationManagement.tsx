import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Calendar, Search, Check, X, QrCode, Clock, Plus, Loader2 } from 'lucide-react';
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
import { getBeds } from '../Services/authBeds';
import { getUsers } from '../Services/authUser';
import type { Service } from '../types/models.ts';
import type { Shelter } from '../types/models.ts';
import { useAuth } from './auth/AuthProvider';
import { toast } from 'sonner';

// Interfaz extendida para reservas enriquecidas
interface EnrichedReservation extends Reservation {
  enrichedUser?: any;
  enrichedBed?: any;
  enrichedShelter?: any;
  enrichedService?: any;
}

export function ReservationsManagement() {
  const { admin } = useAuth();
  const [reservations, setReservations] = useState<EnrichedReservation[]>([]);
  const [services, setServices] = useState<Service[]>([]);
  const [shelters, setShelters] = useState<Shelter[]>([]);
  const [beds, setBeds] = useState<any[]>([]);
  const [users, setUsers] = useState<any[]>([]);
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
      // Cargar todos los datos en paralelo
      const [usersData, bedsData, servicesData, sheltersData] = await Promise.all([
        getUsers(),
        getBeds(),
        getAllServices(),
        getAllShelters()
      ]);

      setUsers(usersData);
      setBeds(bedsData);
      setServices(servicesData);
      setShelters(sheltersData);

      // Después cargar y enriquecer las reservas
      await loadAndEnrichReservations(usersData, bedsData, servicesData, sheltersData);
    } catch (error) {
      console.error('Error al cargar datos:', error);
      toast.error('Error al cargar los datos');
    } finally {
      setIsLoading(false);
    }
  };

  const loadAndEnrichReservations = async (
    usersData: any[],
    bedsData: any[],
    servicesData: any[],
    sheltersData: any[]
  ) => {
    try {
      let reservationsData: Reservation[];
      
      if (admin?.shelter_id) {
        reservationsData = await getReservationsByShelter(admin.shelter_id);
      } else {
        reservationsData = await getAllReservations();
      }

      console.log('Reservas originales:', reservationsData);

      // Enriquecer cada reserva con datos completos
      const enrichedReservations: EnrichedReservation[] = reservationsData.map(reservation => {
        // Buscar usuario
        const enrichedUser = reservation.user || usersData.find(u => u.id === reservation.userId);
        
        // Buscar cama
        const enrichedBed = reservation.bed || bedsData.find(b => b.id === reservation.bedId);
        
        // Buscar shelter (desde la cama o directamente)
        let enrichedShelter = reservation.shelter;
        if (!enrichedShelter && enrichedBed?.shelterId) {
          enrichedShelter = sheltersData.find(s => s.id === enrichedBed.shelterId);
        }
        
        // Buscar servicio (desde la cama o shelter)
        let enrichedService = reservation.service;
        if (!enrichedService && enrichedBed?.serviceId) {
          enrichedService = servicesData.find(s => s.id === enrichedBed.serviceId);
        }

        return {
          ...reservation,
          enrichedUser,
          enrichedBed,
          enrichedShelter,
          enrichedService
        };
      });

      console.log('Reservas enriquecidas:', enrichedReservations);
      setReservations(enrichedReservations);
    } catch (error: any) {
      console.error('Error al cargar reservas:', error);
      toast.error(error.message || 'Error al cargar las reservas');
    }
  };

  const handleStatusChange = async (reservationId: number, newStatus: any) => {
    try {
      await updateReservationStatus(reservationId, newStatus);
      toast.success('Estado actualizado correctamente');
      await loadAndEnrichReservations(users, beds, services, shelters);
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
      await loadAndEnrichReservations(users, beds, services, shelters);
    } catch (error: any) {
      console.error('Error al eliminar reserva:', error);
      toast.error(error.message || 'Error al eliminar la reserva');
    }
  };

  // Funciones helper mejoradas
  const getUserName = (reservation: EnrichedReservation) => {
    return reservation.enrichedUser?.name || 
           reservation.user?.name || 
           'Usuario desconocido';
  };

  const getUserEmail = (reservation: EnrichedReservation) => {
    return reservation.enrichedUser?.email || 
           reservation.user?.email || 
           '';
  };

  const getBedNumber = (reservation: EnrichedReservation) => {
    const bedNumber = reservation.enrichedBed?.bedNumber || 
                     reservation.bed?.bedNumber;
    return bedNumber ? `Cama ${bedNumber}` : 'Sin asignar';
  };

  const getServiceName = (reservation: EnrichedReservation) => {
    return reservation.enrichedService?.name || 
           reservation.service?.name || 
           reservation.enrichedShelter?.name ||
           reservation.shelter?.name || 
           'No especificado';
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
            <div className="flex items-center justify-center py-10">
              <Loader2 className="h-8 w-8 animate-spin text-cyan-500" />
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
                    <TableHead className="w-[80px]">ID</TableHead>
                    <TableHead className="min-w-[180px]">Peregrino</TableHead>
                    <TableHead className="min-w-[120px]">Recurso</TableHead>
                    <TableHead className="min-w-[150px]">Servicio/Refugio</TableHead>
                    <TableHead className="min-w-[180px]">Fechas</TableHead>
                    <TableHead className="min-w-[120px]">Estado</TableHead>
                    <TableHead className="min-w-[140px]">Confirmación</TableHead>
                    <TableHead className="min-w-[120px]">Acciones</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredReservations.map(reservation => (
                    <TableRow key={reservation.id}>
                      <TableCell className="font-medium">
                        #{reservation.id}
                      </TableCell>
                      <TableCell>
                        <div className="min-w-[160px]">
                          <p className="font-medium">{getUserName(reservation)}</p>
                          {getUserEmail(reservation) && (
                            <p className="text-xs text-muted-foreground truncate">
                              {getUserEmail(reservation)}
                            </p>
                          )}
                        </div>
                      </TableCell>
                      <TableCell>
                        <span className="text-sm font-medium">{getBedNumber(reservation)}</span>
                      </TableCell>
                      <TableCell>
                        <span className="text-sm">{getServiceName(reservation)}</span>
                      </TableCell>
                      <TableCell>
                        <div className="text-sm space-y-1">
                          <div className="flex items-center gap-1">
                            <span className="text-xs text-muted-foreground w-12">Entrada:</span>
                            <span className="font-medium">{formatReservationDate(reservation.checkInDate)}</span>
                          </div>
                          <div className="flex items-center gap-1">
                            <span className="text-xs text-muted-foreground w-12">Salida:</span>
                            <span className="font-medium">{formatReservationDate(reservation.checkOutDate)}</span>
                          </div>
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
                            <QrCode className="h-4 w-4 text-green-600 flex-shrink-0" />
                            <span className="text-xs font-mono">{reservation.confirmationCode}</span>
                          </div>
                        ) : reservation.status === 'pending' ? (
                          <div className="flex items-center gap-2 text-yellow-600">
                            <Clock className="h-4 w-4 flex-shrink-0" />
                            <span className="text-xs">Pendiente</span>
                          </div>
                        ) : reservation.status === 'cancelled' ? (
                          <span className="text-xs text-red-600">Cancelada</span>
                        ) : reservation.status === 'completed' ? (
                          <span className="text-xs text-blue-600">Completada</span>
                        ) : (
                          <span className="text-xs text-muted-foreground">-</span>
                        )}
                      </TableCell>
                      <TableCell>
                        <div className="flex gap-1">
                          {reservation.status === 'pending' && (
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-8 w-8"
                              title="Confirmar reserva"
                              onClick={() => handleStatusChange(reservation.id, 'confirmed')}
                            >
                              <Check className="h-4 w-4 text-green-600" />
                            </Button>
                          )}
                          {(reservation.status === 'pending' || reservation.status === 'confirmed') && (
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-8 w-8"
                              title="Cancelar reserva"
                              onClick={() => handleStatusChange(reservation.id, 'cancelled')}
                            >
                              <X className="h-4 w-4 text-red-600" />
                            </Button>
                          )}
                          {reservation.confirmationCode && (
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-8 w-8"
                              title="Ver código QR"
                            >
                              <QrCode className="h-4 w-4 text-cyan-600" />
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