import { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Calendar, Search, Plus, QrCode, Check, X, Clock, AlertCircle } from 'lucide-react';

// Mock data
const reservationData = [
  { 
    id: 1001, 
    patient: 'Juan Pérez', 
    bedNumber: '101',
    service: 'Urgencias',
    checkIn: '2024-01-15',
    checkOut: '2024-01-18',
    status: 'confirmed',
    confirmationMethod: 'qr',
    createdAt: '2024-01-14 10:30'
  },
  { 
    id: 1002, 
    patient: 'María García', 
    bedNumber: '201',
    service: 'Cirugía',
    checkIn: '2024-01-14',
    checkOut: '2024-01-20',
    status: 'pending',
    confirmationMethod: 'pending',
    createdAt: '2024-01-14 09:15'
  },
  { 
    id: 1003, 
    patient: 'Carlos López', 
    bedNumber: '202',
    service: 'Cirugía',
    checkIn: '2024-01-16',
    checkOut: '2024-01-19',
    status: 'pending',
    confirmationMethod: 'pending',
    createdAt: '2024-01-14 14:20'
  },
  { 
    id: 1004, 
    patient: 'Ana Rodríguez', 
    bedNumber: '301',
    service: 'Medicina General',
    checkIn: '2024-01-17',
    checkOut: '2024-01-22',
    status: 'cancelled',
    confirmationMethod: 'manual',
    createdAt: '2024-01-13 16:45'
  }
];

const services = ['Todos', 'Urgencias', 'Cirugía', 'Medicina General', 'Pediatría'];
const statuses = ['Todos', 'pending', 'confirmed', 'cancelled', 'completed'];

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
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedService, setSelectedService] = useState('Todos');
  const [selectedStatus, setSelectedStatus] = useState('Todos');
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);

  const filteredReservations = reservationData.filter(reservation => {
    const matchesSearch = reservation.patient.toLowerCase().includes(searchTerm.toLowerCase()) ||
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
    console.log('Confirming reservation:', id);
  };

  const handleCancelReservation = (id: number) => {
    console.log('Cancelling reservation:', id);
  };

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
                <Label>Paciente</Label>
                <Select>
                  <SelectTrigger>
                    <SelectValue placeholder="Seleccionar paciente" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="1">Juan Pérez</SelectItem>
                    <SelectItem value="2">María García</SelectItem>
                    <SelectItem value="3">Carlos López</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              
              <div className="space-y-2">
                <Label>Servicio</Label>
                <Select>
                  <SelectTrigger>
                    <SelectValue placeholder="Seleccionar servicio" />
                  </SelectTrigger>
                  <SelectContent>
                    {services.slice(1).map(service => (
                      <SelectItem key={service} value={service}>{service}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label>Fecha Ingreso</Label>
                  <Input type="date" />
                </div>
                <div className="space-y-2">
                  <Label>Fecha Salida</Label>
                  <Input type="date" />
                </div>
              </div>
              
              <div className="flex gap-2 pt-4">
                <Button className="flex-1">
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
                  placeholder="Paciente, cama o ID..."
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
                  {services.map(service => (
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
                  {statuses.slice(1).map(status => (
                    <SelectItem key={status} value={status}>
                      {statusLabels[status as keyof typeof statusLabels]}
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
          <CardTitle>Lista de Reservas</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ID</TableHead>
                <TableHead>Paciente</TableHead>
                <TableHead>Cama</TableHead>
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
                  <TableCell>{reservation.patient}</TableCell>
                  <TableCell>
                    <Badge variant="outline">
                      Cama {reservation.bedNumber}
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
              <div className="text-2xl">{reservationData.filter(r => r.status === 'pending').length}</div>
              <p className="text-sm text-muted-foreground">Pendientes de QR</p>
            </div>
            <div className="text-center p-4 border rounded-lg">
              <QrCode className="h-8 w-8 mx-auto mb-2 text-green-600" />
              <div className="text-2xl">{reservationData.filter(r => r.confirmationMethod === 'qr').length}</div>
              <p className="text-sm text-muted-foreground">Confirmadas por QR</p>
            </div>
            <div className="text-center p-4 border rounded-lg">
              <Check className="h-8 w-8 mx-auto mb-2 text-blue-600" />
              <div className="text-2xl">{reservationData.filter(r => r.status === 'confirmed').length}</div>
              <p className="text-sm text-muted-foreground">Total Confirmadas</p>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}