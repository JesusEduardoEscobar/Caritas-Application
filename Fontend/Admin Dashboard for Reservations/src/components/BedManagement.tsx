import { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Input } from './ui/input';
import { Bed, Filter, Search, Plus, Edit, Trash2 } from 'lucide-react';

// Mock data
const bedData = [
  { id: 1, number: '101', service: 'Hospedaje', status: 'occupied', checkIn: '2024-01-15', checkOut: '2024-01-18' },
  { id: 2, number: '102', service: 'Hospedaje', status: 'available', checkIn: null, checkOut: null },
  { id: 3, number: '103', service: 'Hospedaje', status: 'maintenance', checkIn: null, checkOut: null },
  { id: 4, number: '201', service: 'Hospedaje', status: 'occupied', checkIn: '2024-01-14', checkOut: '2024-01-20' },
  { id: 5, number: '202', service: 'Hospedaje', status: 'reserved', checkIn: '2024-01-16', checkOut: '2024-01-19' },
  { id: 6, number: '301', service: 'Hospedaje', status: 'available', checkIn: null, checkOut: null },
  { id: 7, number: '401', service: 'Hospedaje', status: 'available', checkIn: null, checkOut: null },
  { id: 8, number: '1', service: 'Regaderas', status: 'available', checkIn: null, checkOut: null },
  { id: 9, number: '2', service: 'Regaderas', status: 'occupied', checkIn: '2024-01-15', checkOut: '2024-01-15' },
];

const services = ['Todos', 'Hospedaje', 'Comida', 'Regaderas', 'Lavandería', 'Enfermería'];
const statuses = ['Todos', 'available', 'occupied', 'reserved', 'maintenance'];

const statusLabels = {
  available: 'Disponible',
  occupied: 'Ocupada',
  reserved: 'Reservada',
  maintenance: 'Mantenimiento'
};

const statusColors = {
  available: 'default',
  occupied: 'destructive',
  reserved: 'secondary',
  maintenance: 'outline'
} as const;

export function BedManagement() {
  const [selectedService, setSelectedService] = useState('Todos');
  const [selectedStatus, setSelectedStatus] = useState('Todos');
  const [searchTerm, setSearchTerm] = useState('');

  const filteredBeds = bedData.filter(bed => {
    const matchesService = selectedService === 'Todos' || bed.service === selectedService;
    const matchesStatus = selectedStatus === 'Todos' || bed.status === selectedStatus;
    const matchesSearch = bed.number.toLowerCase().includes(searchTerm.toLowerCase());
    
    return matchesService && matchesStatus && matchesSearch;
  });

  const getStatusBadge = (status: string) => {
    return (
      <Badge variant={statusColors[status as keyof typeof statusColors]}>
        {statusLabels[status as keyof typeof statusLabels]}
      </Badge>
    );
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <h2 className="text-2xl">Gestión de Recursos y Camas</h2>
        <Button>
          <Plus className="h-4 w-4 mr-2" />
          Agregar Recurso
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Filter className="h-5 w-5" />
            Filtros
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="space-y-2">
              <label className="text-sm">Servicio</label>
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
              <label className="text-sm">Estado</label>
              <Select value={selectedStatus} onValueChange={setSelectedStatus}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {statuses.map(status => (
                    <SelectItem key={status} value={status}>
                      {status === 'Todos' ? 'Todos' : statusLabels[status as keyof typeof statusLabels]}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <label className="text-sm">Buscar</label>
              <div className="relative">
                <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
                <Input
                  placeholder="Número de cama o recurso..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10"
                />
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {filteredBeds.map(bed => (
          <Card key={bed.id} className="relative">
            <CardHeader className="pb-3">
              <div className="flex items-center justify-between">
                <CardTitle className="flex items-center gap-2">
                  <Bed className="h-5 w-5" />
                  Cama {bed.number}
                </CardTitle>
                <div className="flex gap-1">
                  <Button variant="ghost" size="icon">
                    <Edit className="h-4 w-4" />
                  </Button>
                  <Button variant="ghost" size="icon">
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            </CardHeader>
            <CardContent className="space-y-3">
              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Servicio:</span>
                <Badge variant="outline">{bed.service}</Badge>
              </div>
              
              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Estado:</span>
                {getStatusBadge(bed.status)}
              </div>

              {(bed.status === 'occupied' || bed.status === 'reserved') && bed.checkIn && (
                <div className="space-y-1 pt-2 border-t">
                  <div className="flex items-center justify-between text-xs">
                    <span>Ingreso:</span>
                    <span>{bed.checkIn}</span>
                  </div>
                  <div className="flex items-center justify-between text-xs">
                    <span>Salida prevista:</span>
                    <span>{bed.checkOut}</span>
                  </div>
                </div>
              )}

              {bed.status === 'available' && (
                <Button className="w-full" size="sm">
                  Marcar como Ocupado
                </Button>
              )}

              {bed.status === 'maintenance' && (
                <Button className="w-full" size="sm" variant="outline">
                  Finalizar Mantenimiento
                </Button>
              )}
            </CardContent>
          </Card>
        ))}
      </div>

      {filteredBeds.length === 0 && (
        <Card>
          <CardContent className="text-center py-8">
            <p className="text-muted-foreground">
              No se encontraron recursos con los filtros aplicados
            </p>
          </CardContent>
        </Card>
      )}
    </div>
  );
}