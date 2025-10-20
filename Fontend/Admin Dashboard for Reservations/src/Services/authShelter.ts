import { API_URL } from './authLogin';
import axios from 'axios';
import type { Shelter, ShelterCreateDto, ShelterUpdateDto } from '../types/models.ts';

const token = localStorage.getItem('token');

export const getAllShelters = async (): Promise<Shelter[]> => {
  try {
    const response = await axios.get(`${API_URL}/Shelters`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    const data = response.data.data;

    // Asegúrate de que sea un arreglo
    if (Array.isArray(data)) {
      return data as Shelter[];
    } else if (data) {
      // Si la API devolviera un solo objeto
      return [data as Shelter];
    } else {
      return [] as Shelter[];
    }

  } catch (error) {
    console.error('Error al obtener refugios:', error);
    return [] as Shelter[];
  }
};


export const getShelterById = async (id: number): Promise<Shelter> => {
  try {
    const response = await axios.get(`${API_URL}/Shelter/${id}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`
      }
    });

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al obtener el refugio');
    }
  } catch (error: any) {
    console.error(`Error al obtener refugio ${id}:`, error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Error al obtener el refugio');
  }
};

// ==================== CREAR REFUGIO ====================

export const createShelter = async (shelterData: ShelterCreateDto): Promise<Shelter> => {
  try {
    const response = await axios.post(`${API_URL}/Shelter`, shelterData, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
        'Content-Type': 'application/json'
      }
    });

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al crear el refugio');
    }
  } catch (error: any) {
    console.error('Error al crear refugio:', error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Error al crear el refugio');
  }
};

// ==================== ACTUALIZAR REFUGIO ====================

export const updateShelter = async (shelterData: ShelterUpdateDto): Promise<Shelter> => {
  try {
    const response = await axios.put(`${API_URL}/Shelter`, shelterData, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
        'Content-Type': 'application/json'
      }
    });

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al actualizar el refugio');
    }
  } catch (error: any) {
    console.error('Error al actualizar refugio:', error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Error al actualizar el refugio');
  }
};

// ==================== ELIMINAR REFUGIO ====================

export const deleteShelter = async (id: number): Promise<void> => {
  try {
    const response = await axios.delete(`${API_URL}/Shelter/${id}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`
      }
    });

    if (!response.data.ok) {
      throw new Error(response.data.message || 'Error al eliminar el refugio');
    }
  } catch (error: any) {
    console.error(`Error al eliminar refugio ${id}:`, error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Error al eliminar el refugio');
  }
};

// ==================== FUNCIONES AUXILIARES ====================

/**
 * Valida los datos de un refugio antes de crear o actualizar
 */
export const validateShelterData = (data: Partial<ShelterCreateDto | ShelterUpdateDto>): string[] => {
  const errors: string[] = [];

  if (!data.name || data.name.trim().length === 0) {
    errors.push('El nombre del refugio es obligatorio');
  }

  if (!data.address || data.address.trim().length === 0) {
    errors.push('La dirección es obligatoria');
  }

  if (data.latitude === undefined || data.latitude < -90 || data.latitude > 90) {
    errors.push('La latitud debe estar entre -90 y 90');
  }

  if (data.longitude === undefined || data.longitude < -180 || data.longitude > 180) {
    errors.push('La longitud debe estar entre -180 y 180');
  }

  if (!data.phone || data.phone.trim().length === 0) {
    errors.push('El teléfono es obligatorio');
  }

  if (data.capacity === undefined || data.capacity < 0) {
    errors.push('La capacidad debe ser un número positivo');
  }

  return errors;
};

/**
 * Formatea un refugio para mostrar en la UI
 */
export const formatShelterForDisplay = (shelter: Shelter) => {
  return {
    ...shelter,
    formattedCapacity: `${shelter.capacity} personas`,
    formattedCoordinates: `${shelter.latitude.toFixed(6)}, ${shelter.longitude.toFixed(6)}`,
    formattedDate: shelter.createdAt 
      ? new Date(shelter.createdAt).toLocaleDateString('es-MX', {
          year: 'numeric',
          month: 'long',
          day: 'numeric'
        })
      : 'No disponible'
  };
};

/**
 * Busca refugios por nombre o dirección
 */
export const searchShelters = (shelters: Shelter[], searchTerm: string): Shelter[] => {
  if (!searchTerm.trim()) return shelters;

  const term = searchTerm.toLowerCase();
  return shelters.filter(shelter => 
    shelter.name.toLowerCase().includes(term) ||
    shelter.address.toLowerCase().includes(term) ||
    shelter.phone.includes(term)
  );
};

/**
 * Ordena refugios por diferentes criterios
 */
export const sortShelters = (shelters: Shelter[], sortBy: 'name' | 'capacity' | 'date'): Shelter[] => {
  const sorted = [...shelters];

  switch (sortBy) {
    case 'name':
      return sorted.sort((a, b) => a.name.localeCompare(b.name));
    case 'capacity':
      return sorted.sort((a, b) => b.capacity - a.capacity);
    case 'date':
      return sorted.sort((a, b) => {
        if (!a.createdAt || !b.createdAt) return 0;
        return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
      });
    default:
      return sorted;
  }
};

/**
 * Calcula la distancia entre dos puntos geográficos (en km)
 * Usa la fórmula de Haversine
 */
export const calculateDistance = (
  lat1: number,
  lon1: number,
  lat2: number,
  lon2: number
): number => {
  const R = 6371; // Radio de la Tierra en km
  const dLat = (lat2 - lat1) * Math.PI / 180;
  const dLon = (lon2 - lon1) * Math.PI / 180;
  
  const a = 
    Math.sin(dLat / 2) * Math.sin(dLat / 2) +
    Math.cos(lat1 * Math.PI / 180) * Math.cos(lat2 * Math.PI / 180) *
    Math.sin(dLon / 2) * Math.sin(dLon / 2);
  
  const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
  const distance = R * c;
  
  return Math.round(distance * 100) / 100; // Redondear a 2 decimales
};

/**
 * Encuentra el refugio más cercano a unas coordenadas dadas
 */
export const findNearestShelter = (
  shelters: Shelter[],
  userLat: number,
  userLon: number
): { shelter: Shelter; distance: number } | null => {
  if (shelters.length === 0) return null;

  let nearest = shelters[0];
  let minDistance = calculateDistance(userLat, userLon, nearest.latitude, nearest.longitude);

  for (const shelter of shelters) {
    const distance = calculateDistance(userLat, userLon, shelter.latitude, shelter.longitude);
    if (distance < minDistance) {
      minDistance = distance;
      nearest = shelter;
    }
  }

  return { shelter: nearest, distance: minDistance };
};

/**
 * Obtiene refugios ordenados por distancia a una ubicación
 */
export const getSheltersByDistance = (
  shelters: Shelter[],
  userLat: number,
  userLon: number
): Array<Shelter & { distance: number }> => {
  return shelters
    .map(shelter => ({
      ...shelter,
      distance: calculateDistance(userLat, userLon, shelter.latitude, shelter.longitude)
    }))
    .sort((a, b) => a.distance - b.distance);
};