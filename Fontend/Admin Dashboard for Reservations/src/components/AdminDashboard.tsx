import { useState } from 'react';
import { Button } from './ui/button';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { QrCode, Users, Calendar, BarChart3 } from 'lucide-react';
import { UserManagement } from './UserManagement';
import { ReservationsManagement } from './ReservationManagement';
import { DashboardStats } from './DashboardStats';
import { CalendarView } from './CalendarView';
import { AuthHeader } from './auth/AuthHeader';

interface AdminDashboardProps {
  onActivateQRMode: () => void;
  isMobile: boolean;
}

export function AdminDashboard({ onActivateQRMode, isMobile }: AdminDashboardProps) {
  const [activeTab, setActiveTab] = useState('dashboard');

  return (
    <div className="min-h-screen bg-background">
      <header className="border-b bg-card p-4">
        <div className="max-w-7xl mx-auto flex items-center justify-between">
          <h1 className="text-2xl">Posada del Peregrino - Cáritas</h1>
          <div className="flex items-center gap-4">
            {isMobile && (
              <Button onClick={onActivateQRMode} variant="outline">
                <QrCode className="h-4 w-4 mr-2" />
                Modo QR
              </Button>
            )}
            <AuthHeader />
          </div>
        </div>
      </header>

      <div className="max-w-7xl mx-auto p-4">
        <Tabs value={activeTab} onValueChange={setActiveTab} className="space-y-6">
          <TabsList className="flex w-full grid-cols-4">
            <TabsTrigger value="dashboard" className="flex items-center gap-2">
              <BarChart3 className="h-4 w-4" />
              Dashboard
            </TabsTrigger>
            <TabsTrigger value="reservations" className="flex items-center gap-2">
              <Calendar className="h-4 w-4" />
              Reservas
            </TabsTrigger>
            <TabsTrigger value="users" className="flex items-center gap-2">
              <Users className="h-4 w-4" />
              Peregrinos
            </TabsTrigger>
            <TabsTrigger value="calendar" className="flex items-center gap-2">
              <Calendar className="h-4 w-4" />
              Calendario
            </TabsTrigger>
          </TabsList>

          <TabsContent value="dashboard">
            <DashboardStats />
          </TabsContent>

          <TabsContent value="reservations">
            <ReservationsManagement />
          </TabsContent>

          <TabsContent value="users">
            <UserManagement />
          </TabsContent>

          <TabsContent value="calendar">
            <CalendarView />
          </TabsContent>
        </Tabs>
      </div>
    </div>
  );
}