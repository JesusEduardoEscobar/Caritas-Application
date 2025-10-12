import { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { ChevronLeft, ChevronRight, Calendar, Filter } from 'lucide-react';

// Mock data for calendar events
const calendarEvents = [
  { id: 1, date: '2024-01-15', bedNumber: '101', pilgrim: 'Peregrino A', service: 'Hospedaje', duration: 3, status: 'occupied' },
  { id: 2, date: '2024-01-14', bedNumber: '201', pilgrim: 'Peregrino B', service: 'Hospedaje', duration: 6, status: 'occupied' },
  { id: 3, date: '2024-01-16', bedNumber: '202', pilgrim: 'Peregrino C', service: 'Hospedaje', duration: 3, status: 'reserved' },
  { id: 4, date: '2024-01-17', bedNumber: '301', pilgrim: 'Peregrino D', service: 'Hospedaje', duration: 5, status: 'reserved' },
];

const services = ['Todos', 'Hospedaje', 'Comida', 'Regaderas', 'Lavandería', 'Enfermería'];

const months = [
  'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
  'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
];

const weekDays = ['Dom', 'Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb'];

export function CalendarView() {
  const [currentDate, setCurrentDate] = useState(new Date(2024, 0, 15)); // January 15, 2024
  const [selectedService, setSelectedService] = useState('Todos');
  const [viewMode, setViewMode] = useState<'month' | 'week'>('month');

  const getDaysInMonth = (date: Date) => {
    const year = date.getFullYear();
    const month = date.getMonth();
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    const firstDay = new Date(year, month, 1).getDay();
    
    const days = [];
    
    // Add empty cells for days before the first day of the month
    for (let i = 0; i < firstDay; i++) {
      days.push(null);
    }
    
    // Add all days of the month
    for (let day = 1; day <= daysInMonth; day++) {
      days.push(new Date(year, month, day));
    }
    
    return days;
  };

  const getEventsForDate = (date: Date | null) => {
    if (!date) return [];
    
    const dateStr = date.toISOString().split('T')[0];
    return calendarEvents.filter(event => {
      const eventDate = new Date(event.date);
      const endDate = new Date(eventDate);
      endDate.setDate(endDate.getDate() + event.duration);
      
      const matchesService = selectedService === 'Todos' || event.service === selectedService;
      const isInDateRange = date >= eventDate && date < endDate;
      
      return matchesService && isInDateRange;
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
      occupied: 'bg-red-100 border-red-300 text-red-800',
      reserved: 'bg-yellow-100 border-yellow-300 text-yellow-800',
      available: 'bg-green-100 border-green-300 text-green-800'
    };
    return colors[status as keyof typeof colors] || 'bg-gray-100 border-gray-300 text-gray-800';
  };

  const days = getDaysInMonth(currentDate);

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <h2 className="text-2xl">Calendario de Ocupación</h2>
        <div className="flex gap-2">
          <Button
            variant={viewMode === 'month' ? 'default' : 'outline'}
            onClick={() => setViewMode('month')}
          >
            Mes
          </Button>
          <Button
            variant={viewMode === 'week' ? 'default' : 'outline'}
            onClick={() => setViewMode('week')}
          >
            Semana
          </Button>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Filter className="h-5 w-5" />
            Filtros
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex gap-4">
            <div className="space-y-2">
              <label className="text-sm">Servicio</label>
              <Select value={selectedService} onValueChange={setSelectedService}>
                <SelectTrigger className="w-48">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {services.map(service => (
                    <SelectItem key={service} value={service}>{service}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="flex items-center gap-2">
              <Calendar className="h-5 w-5" />
              {months[currentDate.getMonth()]} {currentDate.getFullYear()}
            </CardTitle>
            <div className="flex gap-2">
              <Button variant="outline" size="icon" onClick={() => navigateMonth('prev')}>
                <ChevronLeft className="h-4 w-4" />
              </Button>
              <Button variant="outline" size="icon" onClick={() => navigateMonth('next')}>
                <ChevronRight className="h-4 w-4" />
              </Button>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {/* Calendar Grid */}
            <div className="grid grid-cols-7 gap-1">
              {/* Week headers */}
              {weekDays.map(day => (
                <div key={day} className="p-2 text-center font-medium text-sm text-muted-foreground">
                  {day}
                </div>
              ))}
              
              {/* Calendar days */}
              {days.map((day, index) => {
                const events = getEventsForDate(day);
                const isToday = day && day.toDateString() === new Date().toDateString();
                
                return (
                  <div
                    key={index}
                    className={`min-h-24 p-1 border rounded-lg ${
                      day ? 'bg-background hover:bg-accent cursor-pointer' : 'bg-muted'
                    } ${isToday ? 'ring-2 ring-primary' : ''}`}
                  >
                    {day && (
                      <div className="space-y-1">
                        <div className={`text-sm ${isToday ? 'font-bold text-primary' : ''}`}>
                          {day.getDate()}
                        </div>
                        <div className="space-y-1">
                          {events.slice(0, 2).map(event => (
                            <div
                              key={event.id}
                              className={`text-xs p-1 rounded border ${getStatusColor(event.status)}`}
                            >
                              <div className="truncate">
                                {event.service === 'Hospedaje' ? `Cama ${event.bedNumber}` : event.service}
                              </div>
                              <div className="truncate text-xs opacity-75">
                                {event.pilgrim}
                              </div>
                            </div>
                          ))}
                          {events.length > 2 && (
                            <div className="text-xs text-muted-foreground text-center">
                              +{events.length - 2} más
                            </div>
                          )}
                        </div>
                      </div>
                    )}
                  </div>
                );
              })}
            </div>

            {/* Legend */}
            <div className="flex flex-wrap gap-4 pt-4 border-t">
              <div className="flex items-center gap-2">
                <div className="w-4 h-4 bg-red-100 border border-red-300 rounded"></div>
                <span className="text-sm">En uso</span>
              </div>
              <div className="flex items-center gap-2">
                <div className="w-4 h-4 bg-yellow-100 border border-yellow-300 rounded"></div>
                <span className="text-sm">Reservado</span>
              </div>
              <div className="flex items-center gap-2">
                <div className="w-4 h-4 bg-green-100 border border-green-300 rounded"></div>
                <span className="text-sm">Disponible</span>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Upcoming Events */}
      <Card>
        <CardHeader>
          <CardTitle>Próximos Eventos</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-3">
            {calendarEvents
              .filter(event => selectedService === 'Todos' || event.service === selectedService)
              .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
              .slice(0, 5)
              .map(event => {
                const startDate = new Date(event.date);
                const endDate = new Date(startDate);
                endDate.setDate(endDate.getDate() + event.duration);
                
                return (
                  <div key={event.id} className="flex items-center justify-between p-3 border rounded-lg">
                    <div className="flex-1">
                      <div className="flex items-center gap-2">
                        <Badge variant="outline">
                          {event.service === 'Hospedaje' ? `Cama ${event.bedNumber}` : `${event.service} #${event.bedNumber}`}
                        </Badge>
                        <Badge variant="secondary">{event.service}</Badge>
                      </div>
                      <p className="mt-1">{event.pilgrim}</p>
                      <p className="text-sm text-muted-foreground">
                        {startDate.toLocaleDateString()} - {endDate.toLocaleDateString()} 
                        ({event.duration} días)
                      </p>
                    </div>
                    <Badge variant={event.status === 'occupied' ? 'destructive' : 'secondary'}>
                      {event.status === 'occupied' ? 'En uso' : 'Reservado'}
                    </Badge>
                  </div>
                );
              })}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}