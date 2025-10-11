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
  { service: 'Lavanderia', total: 15, occupied: 12, available: 3 },
  { service: 'Cocina', total: 20, occupied: 15, available: 5 },
  { service: 'Ducha', total: 10, occupied: 4, available: 6 },
  { service: 'Doctor', total: 5, occupied: 0, available: 5 }
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
          <CardTitle>Camas por Servicio</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {bedsByService.map((service) => (
              <div key={service.service} className="space-y-2">
                <div className="flex items-center justify-between">
                  <h4 className="font-medium">{service.service}</h4>
                  <div className="flex gap-2">
                    <Badge variant="outline">
                      {service.occupied}/{service.total}
                    </Badge>
                    {service.available === 0 ? (
                      <Badge variant="destructive">Completo</Badge>
                    ) : service.available <= 2 ? (
                      <Badge variant="secondary">
                        <AlertCircle className="h-3 w-3 mr-1" />
                        Pocas disponibles
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
                  value={(service.occupied / service.total) * 100} 
                  className="h-2"
                />
                <div className="flex justify-between text-xs text-muted-foreground">
                  <span>Ocupadas: {service.occupied}</span>
                  <span>Disponibles: {service.available}</span>
                </div>
              </div>
            ))}
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
                      Usuario: Paciente {i + 1}
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
            <CardTitle>Próximos Vencimientos</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              <div className="flex items-center justify-between p-3 border rounded-lg">
                <div>
                  <p>Cama 12 - Urgencias</p>
                  <p className="text-xs text-muted-foreground">
                    Vence en 2 horas
                  </p>
                </div>
                <Badge variant="secondary">2h</Badge>
              </div>
              <div className="flex items-center justify-between p-3 border rounded-lg">
                <div>
                  <p>Cama 25 - Cirugía</p>
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