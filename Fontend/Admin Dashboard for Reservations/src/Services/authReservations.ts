import axios from 'axios';

import { API_URL } from './authLogin'

// ==================== INTERFACES ====================

export interface Reservation {
  id: number;
  userId: number;
  bedId: number;
  checkInDate: string;
  checkOutDate: string;
  status: 'pending' | 'confirmed' | 'cancelled' | 'completed';
  confirmationCode?: string;
  notes?: string;
  createdAt?: string;
  
  // Relaciones (si el backend las incluye)
  user?: {
    id: number;
    name: string;
    email: string;
    phone?: string;
  };
  bed?: {
    id: number;
    bedNumber: string;
    roomId: number;
    shelterId: number;
  };
  shelter?: {
    id: number;
    name: string;
  };
  service?: {
    id: number;
    name: string;
  };
}

export interface CreateReservationDto {
  userId: number;
  bedId: number;
  checkInDate: string;
  checkOutDate: string;
  notes?: string;
}

export interface UpdateReservationDto {
  id: number;
  status?: 'pending' | 'confirmed' | 'cancelled' | 'completed';
  checkInDate?: string;
  checkOutDate?: string;
  notes?: string;
}

export interface ReservationFilters {
  search?: string;
  status?: string;
  serviceId?: number;
  shelterId?: number;
  startDate?: string;
  endDate?: string;
}

// ==================== OBTENER TODAS LAS RESERVAS ====================

export const getAllReservations = async (): Promise<Reservation[]> => {
  try {
    const response = await axios.get(`${API_URL}/Reservation`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`
      }
    });

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al obtener reservas');
    }
  } catch (error: any) {
    console.error('Error al obtener reservas:', error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Error al obtener las reservas');
  }
};

// ==================== OBTENER RESERVAS POR REFUGIO ====================

export const getReservationsByShelter = async (shelterId: number): Promise<Reservation[]> => {
  try {
    const response = await axios.get(`${API_URL}/Reservation/shelter/${shelterId}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`
      }
    });

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al obtener reservas');
    }
  } catch (error: any) {
    console.error('Error al obtener reservas por refugio:', error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Error al obtener las reservas del refugio');
  }
};

// ==================== OBTENER UNA RESERVA POR ID ====================

export const getReservationById = async (id: number): Promise<Reservation> => {
  try {
    const response = await axios.get(`${API_URL}/Reservation/${id}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`
      }
    });

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al obtener la reserva');
    }
  } catch (error: any) {
    console.error(`Error al obtener reserva ${id}:`, error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Error al obtener la reserva');
  }
};

// ==================== CREAR RESERVA ====================

export const createReservation = async (reservationData: CreateReservationDto): Promise<Reservation> => {
  try {
    const response = await axios.post(`${API_URL}/Reservation`, reservationData, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
        'Content-Type': 'application/json'
      }
    });

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al crear la reserva');
    }
  } catch (error: any) {
    console.error('Error al crear reserva:', error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Error al crear la reserva');
  }
};

// ==================== ACTUALIZAR RESERVA ====================

export const updateReservation = async (reservationData: UpdateReservationDto): Promise<Reservation> => {
  try {
    const response = await axios.put(`${API_URL}/Reservation`, reservationData, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
        'Content-Type': 'application/json'
      }
    });

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al actualizar la reserva');
    }
  } catch (error: any) {
    console.error('Error al actualizar reserva:', error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Error al actualizar la reserva');
  }
};

// ==================== CAMBIAR ESTADO DE RESERVA ====================

export const updateReservationStatus = async (
  id: number, 
  status: 'pending' | 'confirmed' | 'cancelled' | 'completed'
): Promise<Reservation> => {
  try {
    const response = await axios.patch(
      `${API_URL}/Reservation/${id}/status`, 
      { status },
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
          'Content-Type': 'application/json'
        }
      }
    );

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al actualizar el estado');
    }
  } catch (error: any) {
    console.error('Error al actualizar estado:', error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Error al actualizar el estado de la reserva');
  }
};

// ==================== ELIMINAR RESERVA ====================

export const deleteReservation = async (id: number): Promise<void> => {
  try {
    const response = await axios.delete(`${API_URL}/Reservation/${id}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`
      }
    });

    if (!response.data.ok) {
      throw new Error(response.data.message || 'Error al eliminar la reserva');
    }
  } catch (error: any) {
    console.error(`Error al eliminar reserva ${id}:`, error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Error al eliminar la reserva');
  }
};

// ==================== FUNCIONES AUXILIARES ====================

/**
 * Filtra reservas según múltiples criterios
 */
export const filterReservations = (
  reservations: Reservation[], 
  filters: ReservationFilters
): Reservation[] => {
  let filtered = [...reservations];

  // Filtro de búsqueda (ID, nombre de usuario, código de confirmación)
  if (filters.search) {
    const searchLower = filters.search.toLowerCase();
    filtered = filtered.filter(r => 
      r.id.toString().includes(searchLower) ||
      r.user?.name.toLowerCase().includes(searchLower) ||
      r.confirmationCode?.toLowerCase().includes(searchLower) ||
      r.bed?.bedNumber.toLowerCase().includes(searchLower)
    );
  }

  // Filtro de estado
  if (filters.status && filters.status !== 'all') {
    filtered = filtered.filter(r => r.status === filters.status);
  }

  // Filtro de servicio
  if (filters.serviceId) {
    filtered = filtered.filter(r => r.service?.id === filters.serviceId);
  }

  // Filtro de refugio
  if (filters.shelterId) {
    filtered = filtered.filter(r => r.shelter?.id === filters.shelterId);
  }

  // Filtro de fechas
  if (filters.startDate) {
    filtered = filtered.filter(r => 
      new Date(r.checkInDate) >= new Date(filters.startDate!)
    );
  }

  if (filters.endDate) {
    filtered = filtered.filter(r => 
      new Date(r.checkOutDate) <= new Date(filters.endDate!)
    );
  }

  return filtered;
};

/**
 * Obtiene el nombre legible del estado
 */
export const getStatusLabel = (status: string): string => {
  const labels: Record<string, string> = {
    'pending': 'Pendiente',
    'confirmed': 'Confirmada',
    'cancelled': 'Cancelada',
    'completed': 'Completada'
  };
  return labels[status] || status;
};

/**
 * Obtiene el color del badge según el estado
 */
export const getStatusColor = (status: string): string => {
  const colors: Record<string, string> = {
    'pending': 'bg-yellow-100 text-yellow-800 border-yellow-300',
    'confirmed': 'bg-green-100 text-green-800 border-green-300',
    'cancelled': 'bg-red-100 text-red-800 border-red-300',
    'completed': 'bg-blue-100 text-blue-800 border-blue-300'
  };
  return colors[status] || 'bg-gray-100 text-gray-800';
};

/**
 * Formatea fechas para mostrar
 */
export const formatReservationDate = (dateString: string): string => {
  const date = new Date(dateString);
  return date.toLocaleDateString('es-MX', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit'
  });
};

/**
 * Calcula la duración de la reserva en días
 */
export const calculateDuration = (checkIn: string, checkOut: string): number => {
  const start = new Date(checkIn);
  const end = new Date(checkOut);
  const diff = end.getTime() - start.getTime();
  return Math.ceil(diff / (1000 * 60 * 60 * 24));
};

/**
 * Valida si una reserva está activa
 */
export const isActiveReservation = (reservation: Reservation): boolean => {
  const now = new Date();
  const checkIn = new Date(reservation.checkInDate);
  const checkOut = new Date(reservation.checkOutDate);
  
  return reservation.status === 'confirmed' && 
         now >= checkIn && 
         now <= checkOut;
};

/**
 * Obtiene estadísticas de reservas
 */
export const getReservationStats = (reservations: Reservation[]) => {
  const total = reservations.length;
  const pending = reservations.filter(r => r.status === 'pending').length;
  const confirmed = reservations.filter(r => r.status === 'confirmed').length;
  const cancelled = reservations.filter(r => r.status === 'cancelled').length;
  const completed = reservations.filter(r => r.status === 'completed').length;
  const active = reservations.filter(isActiveReservation).length;

  return {
    total,
    pending,
    confirmed,
    cancelled,
    completed,
    active
  };
};

/**
 * Ordena reservas
 */
export const sortReservations = (
  reservations: Reservation[], 
  sortBy: 'date' | 'status' | 'user' = 'date',
  order: 'asc' | 'desc' = 'desc'
): Reservation[] => {
  const sorted = [...reservations];

  sorted.sort((a, b) => {
    let comparison = 0;

    switch (sortBy) {
      case 'date':
        comparison = new Date(b.checkInDate).getTime() - new Date(a.checkInDate).getTime();
        break;
      case 'status':
        comparison = a.status.localeCompare(b.status);
        break;
      case 'user':
        comparison = (a.user?.name || '').localeCompare(b.user?.name || '');
        break;
    }

    return order === 'asc' ? comparison : -comparison;
  });

  return sorted;
};