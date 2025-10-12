import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { Users, Bed, Calendar, CheckCircle, Clock, AlertCircle } from 'lucide-react';

// Mock data
const stats = {
  totalUsers: 156,
  activeReservations: 23,
  totalBeds: 50,
  occupiedBeds: 31,
  pendingConfirmations: 5,
  todayCheckIns: 8,
  todayCheckOuts: 12
};

const bedsByService = [
  { service: 'Hospedaje', total: 50, inUse: 31, reserved: 8, available: 11 },
  { service: 'Comida', total: 50, inUse: 38, reserved: 5, available: 7 },
  { service: 'Regaderas', total: 10, inUse: 6, reserved: 2, available: 2 },
  { service: 'Lavandería', total: 8, inUse: 3, reserved: 2, available: 3 },
  { service: 'Enfermería', total: 5, inUse: 2, reserved: 1, available: 2 }
];

export function DashboardStats() {
  const occupancyRate = Math.round((stats.occupiedBeds / stats.totalBeds) * 100);

  return (
    <div className="space-y-6">
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm">Total Usuarios</CardTitle>
            <Users className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl">{stats.totalUsers}</div>
            <p className="text-xs text-muted-foreground">
              +12 desde el mes pasado
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm">Reservas Activas</CardTitle>
            <Calendar className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl">{stats.activeReservations}</div>
            <p className="text-xs text-muted-foreground">
              {stats.pendingConfirmations} pendientes de confirmación
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm">Ocupación de Camas</CardTitle>
            <Bed className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl">{stats.occupiedBeds}/{stats.totalBeds}</div>
            <Progress value={occupancyRate} className="mt-2" />
            <p className="text-xs text-muted-foreground mt-2">
              {occupancyRate}% ocupación
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm">Actividad Hoy</CardTitle>
            <Clock className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="space-y-1">
              <div className="flex justify-between text-sm">
                <span>Ingresos:</span>
                <span className="text-green-600">{stats.todayCheckIns}</span>
              </div>
              <div className="flex justify-between text-sm">
                <span>Salidas:</span>
                <span className="text-blue-600">{stats.todayCheckOuts}</span>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Uso de Servicios de la Posada del Peregrino</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-6">
            {bedsByService.map((service) => {
              const totalUsed = service.inUse + service.reserved;
              const usagePercent = (totalUsed / service.total) * 100;
              
              return (
                <div key={service.service} className="space-y-3">
                  <div className="flex items-center justify-between">
                    <h4 className="font-medium">{service.service}</h4>
                    <div className="flex gap-2">
                      <Badge variant="outline" className="text-base px-3">
                        {totalUsed}/{service.total}
                      </Badge>
                      {service.available === 0 ? (
                        <Badge variant="destructive">Completo</Badge>
                      ) : service.available <= 3 ? (
                        <Badge variant="secondary">
                          <AlertCircle className="h-3 w-3 mr-1" />
                          Poca capacidad
                        </Badge>
                      ) : (
                        <Badge variant="default">
                          <CheckCircle className="h-3 w-3 mr-1" />
                          Disponible
                        </Badge>
                      )}
                    </div>
                  </div>
                  
                  <Progress 
                    value={usagePercent} 
                    className="h-3"
                  />
                  
                  <div className="grid grid-cols-3 gap-4">
                    <div className="space-y-1">
                      <p className="text-xs text-muted-foreground">En uso</p>
                      <p className="text-lg font-medium text-red-600">{service.inUse}</p>
                    </div>
                    <div className="space-y-1">
                      <p className="text-xs text-muted-foreground">Reservadas</p>
                      <p className="text-lg font-medium text-yellow-600">{service.reserved}</p>
                    </div>
                    <div className="space-y-1">
                      <p className="text-xs text-muted-foreground">Disponibles</p>
                      <p className="text-lg font-medium text-green-600">{service.available}</p>
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        </CardContent>
      </Card>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Card>
          <CardHeader>
            <CardTitle>Confirmaciones Pendientes</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {Array.from({ length: stats.pendingConfirmations }, (_, i) => (
                <div key={i} className="flex items-center justify-between p-3 border rounded-lg">
                  <div>
                    <p>Reserva #{1001 + i}</p>
                    <p className="text-xs text-muted-foreground">
                      Peregrino {i + 1}
                    </p>
                  </div>
                  <Badge variant="outline">Pendiente QR</Badge>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Próximos Vencimientos de Hospedaje</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              <div className="flex items-center justify-between p-3 border rounded-lg">
                <div>
                  <p>Cama 12 - Hospedaje</p>
                  <p className="text-xs text-muted-foreground">
                    Vence en 2 horas
                  </p>
                </div>
                <Badge variant="secondary">2h</Badge>
              </div>
              <div className="flex items-center justify-between p-3 border rounded-lg">
                <div>
                  <p>Cama 25 - Hospedaje</p>
                  <p className="text-xs text-muted-foreground">
                    Vence mañana
                  </p>
                </div>
                <Badge variant="outline">1d</Badge>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}