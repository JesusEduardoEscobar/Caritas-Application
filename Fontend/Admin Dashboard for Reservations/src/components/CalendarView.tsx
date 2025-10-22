import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from './ui/dialog';
import { ChevronLeft, ChevronRight, Calendar, Filter, CheckCircle2, XCircle } from 'lucide-react';
import { getAllReservations, getReservationsByShelter, updateReservationStatus, updateReservation, type Reservation } from '../Services/authReservations';
import { getAllServices } from '../Services/authServices';
import type { Service } from '../types/models.ts';
import { useAuth } from './auth/AuthProvider';
import { toast } from 'sonner';
import { Label } from "./ui/label"
import { Input } from "./ui/input"

const months = [
  'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
  'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
];

const weekDays = ['Dom', 'Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb'];

export function CalendarView() {
  const { admin } = useAuth();
  const [currentDate, setCurrentDate] = useState(new Date());
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [services, setServices] = useState<Service[]>([]);
  const [selectedService, setSelectedService] = useState('all');
  const [viewMode, setViewMode] = useState<'month' | 'week'>('month');
  const [isLoading, setIsLoading] = useState(true);
  
  // Modal para cerrar reserva
  const [showCloseModal, setShowCloseModal] = useState(false);
  const [selectedReservation, setSelectedReservation] = useState<Reservation | null>(null);
  const [closeDate, setCloseDate] = useState('');

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setIsLoading(true);
    try {
      await Promise.all([
        loadReservations(),
        loadServices()
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
      toast.error('Error al cargar las reservas');
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

  const getDaysInMonth = (date: Date) => {
    const year = date.getFullYear();
    const month = date.getMonth();
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    const firstDay = new Date(year, month, 1).getDay();
    
    const days = [];
    
    for (let i = 0; i < firstDay; i++) {
      days.push(null);
    }
    
    for (let day = 1; day <= daysInMonth; day++) {
      days.push(new Date(year, month, day));
    }
    
    return days;
  };

  const getEventsForDate = (date: Date | null) => {
    if (!date) return [];
    
    return reservations.filter(reservation => {
      const checkIn = new Date(reservation.checkInDate);
      const checkOut = new Date(reservation.checkOutDate);
      
      // Normalizar fechas para comparación (solo día)
      const dateOnly = new Date(date.getFullYear(), date.getMonth(), date.getDate());
      const checkInOnly = new Date(checkIn.getFullYear(), checkIn.getMonth(), checkIn.getDate());
      const checkOutOnly = new Date(checkOut.getFullYear(), checkOut.getMonth(), checkOut.getDate());
      
      const matchesService = selectedService === 'all' || 
        (reservation.service?.id === parseInt(selectedService));
      
      const isInDateRange = dateOnly >= checkInOnly && dateOnly <= checkOutOnly;
      
      return matchesService && isInDateRange && reservation.status !== 'cancelled';
    });
  };

  const navigateMonth = (direction: 'prev' | 'next') => {
    const newDate = new Date(currentDate);
    if (direction === 'prev') {
      newDate.setMonth(newDate.getMonth() - 1);
    } else {
      newDate.setMonth(newDate.getMonth() + 1);
    }
    setCurrentDate(newDate);
  };

  const getStatusColor = (status: string) => {
    const colors = {
      confirmed: 'bg-green-100 border-green-300 text-green-800',
      pending: 'bg-yellow-100 border-yellow-300 text-yellow-800',
      completed: 'bg-blue-100 border-blue-300 text-blue-800',
      cancelled: 'bg-red-100 border-red-300 text-red-800'
    };
    return colors[status as keyof typeof colors] || 'bg-gray-100 border-gray-300 text-gray-800';
  };

  const getStatusLabel = (status: string) => {
    const labels: Record<string, string> = {
      pending: 'Pendiente',
      confirmed: 'En uso',
      completed: 'Completada',
      cancelled: 'Cancelada'
    };
    return labels[status] || status;
  };

  const openCloseModal = (reservation: Reservation) => {
    setSelectedReservation(reservation);
    setCloseDate(new Date().toISOString().split('T')[0]);
    setShowCloseModal(true);
  };

  const handleCloseReservation = async () => {
    if (!selectedReservation) return;

    try {
      // Actualizar la fecha de checkout y marcar como completada
      await updateReservation({
        id: selectedReservation.id,
        checkOutDate: closeDate,
        status: 'completed'
      });

      toast.success('Reserva cerrada correctamente');
      setShowCloseModal(false);
      setSelectedReservation(null);
      await loadReservations();
    } catch (error: any) {
      console.error('Error al cerrar reserva:', error);
      toast.error(error.message || 'Error al cerrar la reserva');
    }
  };

  const handleCompleteNow = async () => {
    if (!selectedReservation) return;

    try {
      await updateReservationStatus(selectedReservation.id, 'completed');
      toast.success('Reserva completada');
      setShowCloseModal(false);
      setSelectedReservation(null);
      await loadReservations();
    } catch (error: any) {
      console.error('Error al completar reserva:', error);
      toast.error(error.message || 'Error al completar la reserva');
    }
  };

  const days = getDaysInMonth(currentDate);

  // Calcular estadísticas
  const stats = {
    total: reservations.filter(r => r.status !== 'cancelled').length,
    active: reservations.filter(r => {
      const now = new Date();
      const checkIn = new Date(r.checkInDate);
      const checkOut = new Date(r.checkOutDate);
      return r.status === 'confirmed' && now >= checkIn && now <= checkOut;
    }).length,
    pending: reservations.filter(r => r.status === 'pending').length,
    completed: reservations.filter(r => r.status === 'completed').length
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <h2 className="text-2xl font-bold">Calendario de Ocupación</h2>
        <div className="flex gap-2">
          <Button
            variant={viewMode === 'month' ? 'default' : 'outline'}
            onClick={() => setViewMode('month')}
            className={viewMode === 'month' ? 'bg-cyan-600' : ''}
          >
            Mes
          </Button>
          <Button
            variant={viewMode === 'week' ? 'default' : 'outline'}
            onClick={() => setViewMode('week')}
            className={viewMode === 'week' ? 'bg-cyan-600' : ''}
          >
            Semana
          </Button>
        </div>
      </div>

      {/* Estadísticas */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        <Card>
          <CardContent className="pt-6">
            <div className="text-center">
              <p className="text-sm text-muted-foreground">Total Reservas</p>
              <p className="text-3xl font-bold text-cyan-500">{stats.total}</p>
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="pt-6">
            <div className="text-center">
              <p className="text-sm text-muted-foreground">Activas</p>
              <p className="text-3xl font-bold text-green-500">{stats.active}</p>
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="pt-6">
            <div className="text-center">
              <p className="text-sm text-muted-foreground">Pendientes</p>
              <p className="text-3xl font-bold text-yellow-500">{stats.pending}</p>
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="pt-6">
            <div className="text-center">
              <p className="text-sm text-muted-foreground">Completadas</p>
              <p className="text-3xl font-bold text-blue-500">{stats.completed}</p>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Filtros */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Filter className="h-5 w-5 text-cyan-500" />
            Filtros
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-2">
            <label className="text-sm font-medium">Servicio</label>
            <Select value={selectedService} onValueChange={setSelectedService}>
              <SelectTrigger className="w-full md:w-64">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Todos los servicios</SelectItem>
                {services.map(service => (
                  <SelectItem key={service.id} value={service.id.toString()}>
                    {service.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </CardContent>
      </Card>

      {/* Calendario */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="flex items-center gap-2">
              <Calendar className="h-5 w-5 text-cyan-500" />
              {months[currentDate.getMonth()]} {currentDate.getFullYear()}
            </CardTitle>
            <div className="flex gap-2">
              <Button variant="outline" size="icon" onClick={() => navigateMonth('prev')}>
                <ChevronLeft className="h-4 w-4" />
              </Button>
              <Button 
                variant="outline" 
                onClick={() => setCurrentDate(new Date())}
                className="hidden sm:flex"
              >
                Hoy
              </Button>
              <Button variant="outline" size="icon" onClick={() => navigateMonth('next')}>
                <ChevronRight className="h-4 w-4" />
              </Button>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="text-center py-10">
              <p className="text-muted-foreground">Cargando calendario...</p>
            </div>
          ) : (
            <div className="space-y-4">
              <div className="grid grid-cols-7 gap-1">
                {weekDays.map(day => (
                  <div key={day} className="p-2 text-center font-medium text-sm text-muted-foreground">
                    {day}
                  </div>
                ))}
                
                {days.map((day, index) => {
                  const events = getEventsForDate(day);
                  const isToday = day && day.toDateString() === new Date().toDateString();
                  
                  return (
                    <div
                      key={index}
                      className={`min-h-28 p-2 border rounded-lg ${
                        day ? 'bg-background hover:bg-accent/50 cursor-pointer transition-colors' : 'bg-muted'
                      } ${isToday ? 'ring-2 ring-cyan-500' : ''}`}
                    >
                      {day && (
                        <div className="space-y-1">
                          <div className={`text-sm font-medium ${isToday ? 'text-cyan-600' : ''}`}>
                            {day.getDate()}
                          </div>
                          <div className="space-y-1">
                            {events.slice(0, 3).map(event => (
                              <div
                                key={event.id}
                                className={`text-xs p-1.5 rounded border ${getStatusColor(event.status)} cursor-pointer hover:shadow-md transition-shadow`}
                                onClick={() => openCloseModal(event)}
                                title={`${event.user?.name} - ${event.bed?.bedNumber || 'N/A'}`}
                              >
                                <div className="truncate font-medium">
                                  {event.bed?.bedNumber || event.service?.name || 'Reserva'}
                                </div>
                                <div className="truncate text-xs opacity-80">
                                  {event.user?.name || 'Usuario'}
                                </div>
                              </div>
                            ))}
                            {events.length > 3 && (
                              <div className="text-xs text-center text-muted-foreground font-medium">
                                +{events.length - 3} más
                              </div>
                            )}
                          </div>
                        </div>
                      )}
                    </div>
                  );
                })}
              </div>

              {/* Leyenda */}
              <div className="flex flex-wrap gap-4 pt-4 border-t">
                <div className="flex items-center gap-2">
                  <div className="w-4 h-4 bg-green-100 border border-green-300 rounded"></div>
                  <span className="text-sm">Confirmada / En uso</span>
                </div>
                <div className="flex items-center gap-2">
                  <div className="w-4 h-4 bg-yellow-100 border border-yellow-300 rounded"></div>
                  <span className="text-sm">Pendiente</span>
                </div>
                <div className="flex items-center gap-2">
                  <div className="w-4 h-4 bg-blue-100 border border-blue-300 rounded"></div>
                  <span className="text-sm">Completada</span>
                </div>
              </div>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Próximas Reservas */}
      <Card>
        <CardHeader>
          <CardTitle>Próximas Reservas</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-3">
            {reservations
              .filter(r => {
                const checkIn = new Date(r.checkInDate);
                const now = new Date();
                return checkIn >= now && r.status !== 'cancelled' && r.status !== 'completed';
              })
              .sort((a, b) => new Date(a.checkInDate).getTime() - new Date(b.checkInDate).getTime())
              .slice(0, 5)
              .map(reservation => {
                const checkIn = new Date(reservation.checkInDate);
                const checkOut = new Date(reservation.checkOutDate);
                const duration = Math.ceil((checkOut.getTime() - checkIn.getTime()) / (1000 * 60 * 60 * 24));
                
                return (
                  <div key={reservation.id} className="flex items-center justify-between p-4 border rounded-lg hover:bg-accent/50 transition-colors">
                    <div className="flex-1">
                      <div className="flex items-center gap-2 flex-wrap">
                        <Badge variant="outline">
                          {reservation.bed?.bedNumber || reservation.service?.name || `#${reservation.id}`}
                        </Badge>
                        <Badge variant="secondary">
                          {reservation.service?.name || 'Servicio'}
                        </Badge>
                      </div>
                      <p className="mt-2 font-medium">{reservation.user?.name || 'Usuario'}</p>
                      <p className="text-sm text-muted-foreground">
                        {checkIn.toLocaleDateString('es-MX')} - {checkOut.toLocaleDateString('es-MX')} 
                        <span className="ml-2">({duration} días)</span>
                      </p>
                    </div>
                    <div className="flex items-center gap-2">
                      <Badge className={getStatusColor(reservation.status)}>
                        {getStatusLabel(reservation.status)}
                      </Badge>
                      {(reservation.status === 'confirmed' || reservation.status === 'pending') && (
                        <Button
                          size="sm"
                          variant="outline"
                          onClick={() => openCloseModal(reservation)}
                        >
                          Cerrar
                        </Button>
                      )}
                    </div>
                  </div>
                );
              })}
            {reservations.filter(r => {
              const checkIn = new Date(r.checkInDate);
              return checkIn >= new Date() && r.status !== 'cancelled';
            }).length === 0 && (
              <p className="text-center text-muted-foreground py-4">
                No hay próximas reservas
              </p>
            )}
          </div>
        </CardContent>
      </Card>

      {/* Modal para cerrar reserva */}
      <Dialog open={showCloseModal} onOpenChange={setShowCloseModal}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Cerrar Reserva</DialogTitle>
            <DialogDescription>
              {selectedReservation && (
                <div className="mt-2 space-y-2">
                  <p><strong>Usuario:</strong> {selectedReservation.user?.name}</p>
                  <p><strong>Cama/Recurso:</strong> {selectedReservation.bed?.bedNumber || 'N/A'}</p>
                  <p><strong>Check-in:</strong> {new Date(selectedReservation.checkInDate).toLocaleDateString('es-MX')}</p>
                  <p><strong>Check-out programado:</strong> {new Date(selectedReservation.checkOutDate).toLocaleDateString('es-MX')}</p>
                </div>
              )}
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <Label>Fecha de cierre</Label>
              <Input
                type="date"
                value={closeDate}
                onChange={(e) => setCloseDate(e.target.value)}
                max={new Date().toISOString().split('T')[0]}
              />
              <p className="text-xs text-muted-foreground">
                Selecciona la fecha en que terminó la reserva
              </p>
            </div>
          </div>

          <DialogFooter className="flex-col sm:flex-row gap-2">
            <Button
              variant="outline"
              onClick={() => setShowCloseModal(false)}
            >
              Cancelar
            </Button>
            <Button
              variant="outline"
              className="bg-blue-50 hover:bg-blue-100"
              onClick={handleCompleteNow}
            >
              <CheckCircle2 className="h-4 w-4 mr-2" />
              Completar Hoy
            </Button>
            <Button
              className="bg-cyan-600 hover:bg-cyan-700"
              onClick={handleCloseReservation}
              disabled={!closeDate}
            >
              <XCircle className="h-4 w-4 mr-2" />
              Cerrar Reserva
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}