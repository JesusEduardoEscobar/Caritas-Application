import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { Users, Bed, Calendar, CheckCircle, AlertCircle, Shield, UserCheck } from 'lucide-react';
import { getUsers } from '../Services/authUser';
import { toast } from 'sonner';

// Mock data para servicios
const bedsByService = [
  { service: 'Hospedaje', total: 50, inUse: 31, reserved: 8, available: 11 },
  { service: 'Comida', total: 50, inUse: 38, reserved: 5, available: 7 },
  { service: 'Regaderas', total: 10, inUse: 6, reserved: 2, available: 2 },
  { service: 'Lavandería', total: 8, inUse: 3, reserved: 2, available: 3 },
  { service: 'Enfermería', total: 5, inUse: 2, reserved: 1, available: 2 }
];

export function DashboardStats() {
  const [totalUsers, setTotalUsers] = useState(0);
  const [totalAdmins, setTotalAdmins] = useState(0);
  const [totalRegularUsers, setTotalRegularUsers] = useState(0);
  const [verifiedUsers, setVerifiedUsers] = useState(0);
  const [isLoading, setIsLoading] = useState(true);

  // Mock data para otros stats
  const stats = {
    activeReservations: 23,
    totalBeds: 50,
    occupiedBeds: 31,
    pendingConfirmations: 5
  };

  useEffect(() => {
    loadUserStats();
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

  const occupancyRate = Math.round((stats.occupiedBeds / stats.totalBeds) * 100);

  return (
    <div className="space-y-7">
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-5">
        <Card className="border-2">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-3">
            <CardTitle>Total Usuarios</CardTitle>
            <Users className="h-6 w-6" style={{ color: '#06b6d4' }} />
          </CardHeader>
          <CardContent>
            <div className="text-3xl" style={{ color: '#06b6d4' }}>
              {isLoading ? '...' : totalUsers}
            </div>
            <div className="flex items-center gap-2 mt-2">
              <Badge variant="outline" className="px-2 py-1">
                <Shield className="h-3 w-3 mr-1" />
                {isLoading ? '...' : totalAdmins} Admins
              </Badge>
              <Badge variant="outline" className="px-2 py-1">
                <UserCheck className="h-3 w-3 mr-1" />
                {isLoading ? '...' : totalRegularUsers} Usuarios
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
              {isLoading ? '...' : verifiedUsers}
            </div>
            <p className="text-muted-foreground mt-1">
              {isLoading ? '...' : `${totalUsers - verifiedUsers} sin verificar`}
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

      <Card className="border-2">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <span style={{ color: '#06b6d4' }}>●</span>
            Uso de Servicios de la Posada del Peregrino
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-7">
            {bedsByService.map((service) => {
              const totalUsed = service.inUse + service.reserved;
              const usagePercent = (totalUsed / service.total) * 100;
              
              return (
                <div key={service.service} className="space-y-3">
                  <div className="flex items-center justify-between">
                    <h4>{service.service}</h4>
                    <div className="flex gap-3">
                      <Badge variant="outline" className="px-4 py-1">
                        {totalUsed}/{service.total}
                      </Badge>
                      {service.available === 0 ? (
                        <Badge variant="destructive" className="px-3 py-1">Completo</Badge>
                      ) : service.available <= 3 ? (
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
                  
                  <div className="grid grid-cols-3 gap-5">
                    <div className="space-y-1">
                      <p className="text-muted-foreground">En uso</p>
                      <p className="text-xl text-red-600">{service.inUse}</p>
                    </div>
                    <div className="space-y-1">
                      <p className="text-muted-foreground">Reservadas</p>
                      <p className="text-xl text-yellow-600">{service.reserved}</p>
                    </div>
                    <div className="space-y-1">
                      <p className="text-muted-foreground">Disponibles</p>
                      <p className="text-xl" style={{ color: '#06b6d4' }}>{service.available}</p>
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        </CardContent>
      </Card>

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
