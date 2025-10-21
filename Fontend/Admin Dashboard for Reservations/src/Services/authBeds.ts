import axios from "axios";
import { API_URL } from './authLogin';

// =======================
// Tipos de datos (Types)
// =======================
export interface Bed {
  id: number;
  shelterId: number;
  bedNumber: string;
  isAvailable: boolean;
}

export interface BedCreateDto {
  shelterId: number;
  bedNumber: string;
  isAvailable: boolean;
}

export interface BedPutDto extends BedCreateDto {
  id: number;
}

export interface BedPatchAvailabilityDto {
  id: number;
  isAvailable: boolean;
}

export interface ApiResponse<T> {
  ok: boolean;
  code: string;
  message: string;
  data: T;
  rowsCount?: number;
  currentPage?: number;
  totalPages?: number;
}

// =======================
// Funciones principales
// =======================

// 🔹 Obtener todas las camas (puedes filtrar por refugio o disponibilidad)
export const getBeds = async (
  shelterId?: number,
  available?: boolean
): Promise<Bed[]> => {
  try {
    const params: any = {};
    if (shelterId !== undefined) params.shelterId = shelterId;
    if (available !== undefined) params.available = available;

    const response = await axios.get(`${API_URL}/Beds`, { 
      params,
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`
      }
    });

    const data = response.data.data;
    
    if (Array.isArray(data)) {
      return data as Bed[];
    } else if (data) {
      return [data as Bed];
    } else {
      return [] as Bed[];
    }
  } catch (error: any) {
    console.error("Error al obtener camas:", error);
    return [] as Bed[];
  }
};

// 🔹 Obtener una cama específica por ID
export const getBed = async (id: number): Promise<Bed> => {
  try {
    const response = await axios.get(`${API_URL}/Beds/${id}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`
      }
    });

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al obtener la cama');
    }
  } catch (error: any) {
    console.error("Error al obtener cama:", error);
    throw error.response?.data || error;
  }
};

// 🔹 Crear una nueva cama
export const createBed = async (dto: BedCreateDto): Promise<Bed> => {
  try {
    const response = await axios.post(`${API_URL}/Beds`, dto, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
        'Content-Type': 'application/json'
      }
    });

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al crear la cama');
    }
  } catch (error: any) {
    console.error("Error al crear cama:", error);
    throw error.response?.data || error;
  }
};

// 🔹 Actualizar todos los datos de una cama
export const updateBed = async (dto: BedPutDto): Promise<Bed> => {
  try {
    const response = await axios.put(`${API_URL}/Beds`, dto, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
        'Content-Type': 'application/json'
      }
    });

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al actualizar la cama');
    }
  } catch (error: any) {
    console.error("Error al actualizar cama:", error);
    throw error.response?.data || error;
  }
};

// 🔹 Actualizar solo disponibilidad
export const updateBedAvailability = async (
  dto: BedPatchAvailabilityDto
): Promise<Bed> => {
  try {
    const response = await axios.patch(`${API_URL}/Beds/availability`, dto, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
        'Content-Type': 'application/json'
      }
    });

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al actualizar disponibilidad de cama');
    }
  } catch (error: any) {
    console.error("Error al actualizar disponibilidad de cama:", error);
    throw error.response?.data || error;
  }
};

// 🔹 Eliminar una cama
export const deleteBed = async (id: number): Promise<void> => {
  try {
    const response = await axios.delete(`${API_URL}/Beds/${id}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`
      }
    });

    if (!response.data.ok) {
      throw new Error(response.data.message || 'Error al eliminar la cama');
    }
  } catch (error: any) {
    console.error("Error al eliminar cama:", error);
    throw error.response?.data || error;
  }
};

// =======================
// Funciones auxiliares
// =======================

/**
 * Valida los datos de una cama antes de crear o actualizar
 */
export const validateBedData = (data: Partial<BedCreateDto | BedPutDto>): string[] => {
  const errors: string[] = [];

  if (!data.shelterId || data.shelterId <= 0) {
    errors.push('El ID del refugio es obligatorio y debe ser válido');
  }

  if (!data.bedNumber || data.bedNumber.trim().length === 0) {
    errors.push('El número de cama es obligatorio');
  }

  if (data.bedNumber && data.bedNumber.length > 50) {
    errors.push('El número de cama no puede exceder 50 caracteres');
  }

  if (data.isAvailable === undefined || data.isAvailable === null) {
    errors.push('El estado de disponibilidad es obligatorio');
  }

  return errors;
};

/**
 * Buscar camas por número
 */
export const searchBedsByNumber = (beds: Bed[], searchTerm: string): Bed[] => {
  if (!searchTerm.trim()) return beds;

  const term = searchTerm.toLowerCase();
  return beds.filter(bed => 
    bed.bedNumber.toLowerCase().includes(term) ||
    bed.id.toString().includes(term)
  );
};

/**
 * Ordenar camas por diferentes criterios
 */
export const sortBeds = (beds: Bed[], sortBy: 'number' | 'availability' | 'shelter'): Bed[] => {
  const sorted = [...beds];

  switch (sortBy) {
    case 'number':
      return sorted.sort((a, b) => a.bedNumber.localeCompare(b.bedNumber));
    case 'availability':
      return sorted.sort((a, b) => {
        if (a.isAvailable === b.isAvailable) return 0;
        return a.isAvailable ? -1 : 1;
      });
    case 'shelter':
      return sorted.sort((a, b) => a.shelterId - b.shelterId);
    default:
      return sorted;
  }
};

/**
 * Filtrar camas por disponibilidad
 */
export const filterBedsByAvailability = (beds: Bed[], available: boolean | null): Bed[] => {
  if (available === null) return beds;
  return beds.filter(bed => bed.isAvailable === available);
};