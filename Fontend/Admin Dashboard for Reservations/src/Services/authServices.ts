import axios from 'axios';
import { API_URL } from './authLogin';
import type { Service, ServiceCreateDto, ServiceUpdateDto } from '../types/models.ts';

// ==================== OBTENER TODOS LOS SERVICIOS ====================

export const getAllServices = async (): Promise<Service[]> => {
  try {
    const response = await axios.get(`${API_URL}/Services`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`
      }
    });

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al obtener servicios');
    }
  } catch (error: any) {
    console.error('Error al obtener servicios:', error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Error al obtener los servicios');
  }
};

// ==================== OBTENER UN SERVICIO POR ID ====================

export const getServiceById = async (id: number): Promise<Service> => {
  try {
    const response = await axios.get(`${API_URL}/Service/${id}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`
      }
    });

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al obtener el servicio');
    }
  } catch (error: any) {
    console.error(`Error al obtener servicio ${id}:`, error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Error al obtener el servicio');
  }
};

// ==================== CREAR SERVICIO ====================

export const createService = async (serviceData: ServiceCreateDto): Promise<Service> => {
  try {
    const response = await axios.post(`${API_URL}/Service`, serviceData, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
        'Content-Type': 'application/json'
      }
    });

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al crear el servicio');
    }
  } catch (error: any) {
    console.error('Error al crear servicio:', error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Error al crear el servicio');
  }
};

// ==================== ACTUALIZAR SERVICIO ====================

export const updateService = async (serviceData: ServiceUpdateDto): Promise<Service> => {
  try {
    const response = await axios.put(`${API_URL}/Services`, serviceData, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
        'Content-Type': 'application/json'
      }
    });

    if (response.data.ok) {
      return response.data.data;
    } else {
      throw new Error(response.data.message || 'Error al actualizar el servicio');
    }
  } catch (error: any) {
    console.error('Error al actualizar servicio:', error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Error al actualizar el servicio');
  }
};

// ==================== ELIMINAR SERVICIO ====================

export const deleteService = async (id: number): Promise<void> => {
  try {
    const response = await axios.delete(`${API_URL}/Service/${id}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`
      }
    });

    if (!response.data.ok) {
      throw new Error(response.data.message || 'Error al eliminar el servicio');
    }
  } catch (error: any) {
    console.error(`Error al eliminar servicio ${id}:`, error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Error al eliminar el servicio');
  }
};

// ==================== FUNCIONES AUXILIARES ====================

/**
 * Valida los datos de un servicio antes de crear o actualizar
 */
export const validateServiceData = (data: Partial<ServiceCreateDto | ServiceUpdateDto>): string[] => {
  const errors: string[] = [];

  if (!data.name || data.name.trim().length === 0) {
    errors.push('El nombre del servicio es obligatorio');
  }

  if (data.name && data.name.length > 100) {
    errors.push('El nombre del servicio no puede exceder 100 caracteres');
  }

  if (!data.description || data.description.trim().length === 0) {
    errors.push('La descripción es obligatoria');
  }

  if (data.description && data.description.length > 500) {
    errors.push('La descripción no puede exceder 500 caracteres');
  }

  if (!data.iconKey || data.iconKey.trim().length === 0) {
    errors.push('El icono es obligatorio');
  }

  return errors;
};

/**
 * Busca servicios por nombre o descripción
 */
export const searchServices = (services: Service[], searchTerm: string): Service[] => {
  if (!searchTerm.trim()) return services;

  const term = searchTerm.toLowerCase();
  return services.filter(service => 
    service.name.toLowerCase().includes(term) ||
    service.description.toLowerCase().includes(term)
  );
};

/**
 * Ordena servicios alfabéticamente por nombre
 */
export const sortServicesByName = (services: Service[], ascending: boolean = true): Service[] => {
  const sorted = [...services];
  return sorted.sort((a, b) => {
    const comparison = a.name.localeCompare(b.name);
    return ascending ? comparison : -comparison;
  });
};

/**
 * Agrupa servicios por categoría (si el iconKey contiene la categoría)
 * Por ejemplo: "health-medical", "food-kitchen", etc.
 */
export const groupServicesByCategory = (services: Service[]): Record<string, Service[]> => {
  const grouped: Record<string, Service[]> = {};

  services.forEach(service => {
    // Extrae la categoría del iconKey (primera parte antes del guión)
    const category = service.iconKey.split('-')[0] || 'other';
    
    if (!grouped[category]) {
      grouped[category] = [];
    }
    
    grouped[category].push(service);
  });

  return grouped;
};

/**
 * Iconos populares de Lucide React que se pueden usar
 */
export const availableIcons = [
  // Salud y Medicina
  { key: 'heart-pulse', label: 'Salud / Primeros Auxilios', category: 'health' },
  { key: 'pill', label: 'Medicamentos', category: 'health' },
  { key: 'stethoscope', label: 'Atención Médica', category: 'health' },
  { key: 'syringe', label: 'Vacunación', category: 'health' },
  
  // Alimentación
  { key: 'utensils', label: 'Comida / Alimentación', category: 'food' },
  { key: 'coffee', label: 'Bebidas Calientes', category: 'food' },
  { key: 'apple', label: 'Frutas / Alimentos Frescos', category: 'food' },
  { key: 'soup', label: 'Comida Caliente', category: 'food' },
  
  // Refugio y Alojamiento
  { key: 'home', label: 'Alojamiento / Vivienda', category: 'shelter' },
  { key: 'bed', label: 'Cama / Descanso', category: 'shelter' },
  { key: 'hotel', label: 'Hospedaje Temporal', category: 'shelter' },
  
  // Ropa y Vestuario
  { key: 'shirt', label: 'Ropa / Vestuario', category: 'clothing' },
  { key: 'baby', label: 'Artículos para Bebés', category: 'clothing' },
  
  // Higiene y Cuidado Personal
  { key: 'bath', label: 'Higiene / Baños', category: 'hygiene' },
  { key: 'shower', label: 'Duchas', category: 'hygiene' },
  { key: 'droplet', label: 'Agua Potable', category: 'hygiene' },
  
  // Educación y Capacitación
  { key: 'book-open', label: 'Educación', category: 'education' },
  { key: 'graduation-cap', label: 'Capacitación', category: 'education' },
  { key: 'library', label: 'Biblioteca', category: 'education' },
  
  // Comunicación
  { key: 'phone', label: 'Teléfono / Comunicación', category: 'communication' },
  { key: 'wifi', label: 'Internet / WiFi', category: 'communication' },
  { key: 'mail', label: 'Correo / Mensajería', category: 'communication' },
  
  // Transporte
  { key: 'bus', label: 'Transporte Público', category: 'transport' },
  { key: 'car', label: 'Transporte Privado', category: 'transport' },
  
  // Seguridad y Legal
  { key: 'shield', label: 'Seguridad', category: 'security' },
  { key: 'scale', label: 'Asesoría Legal', category: 'security' },
  { key: 'lock', label: 'Protección', category: 'security' },
  
  // Apoyo Psicológico
  { key: 'heart', label: 'Apoyo Emocional', category: 'support' },
  { key: 'users', label: 'Grupos de Apoyo', category: 'support' },
  { key: 'message-circle', label: 'Consejería', category: 'support' },
  
  // Trabajo y Empleo
  { key: 'briefcase', label: 'Empleo / Trabajo', category: 'employment' },
  { key: 'tool', label: 'Capacitación Laboral', category: 'employment' },
  
  // Otros
  { key: 'plus-circle', label: 'Servicios Adicionales', category: 'other' },
  { key: 'info', label: 'Información', category: 'other' },
  { key: 'help-circle', label: 'Ayuda', category: 'other' },
];

/**
 * Obtiene el nombre legible de un icono por su key
 */
export const getIconLabel = (iconKey: string): string => {
  const icon = availableIcons.find(i => i.key === iconKey);
  return icon?.label || iconKey;
};

/**
 * Obtiene la categoría de un icono
 */
export const getIconCategory = (iconKey: string): string => {
  const icon = availableIcons.find(i => i.key === iconKey);
  return icon?.category || 'other';
};

/**
 * Filtra iconos por categoría
 */
export const getIconsByCategory = (category: string) => {
  return availableIcons.filter(icon => icon.category === category);
};

/**
 * Obtiene todas las categorías únicas de iconos
 */
export const getIconCategories = (): string[] => {
  const categories = new Set(availableIcons.map(icon => icon.category));
  return Array.from(categories);
};

/**
 * Valida si un iconKey existe en la lista de iconos disponibles
 */
export const isValidIcon = (iconKey: string): boolean => {
  return availableIcons.some(icon => icon.key === iconKey);
};

/**
 * Formatea un servicio para mostrar en la UI
 */
export const formatServiceForDisplay = (service: Service) => {
  return {
    ...service,
    iconLabel: getIconLabel(service.iconKey),
    category: getIconCategory(service.iconKey),
    shortDescription: service.description.length > 100 
      ? `${service.description.substring(0, 100)}...` 
      : service.description
  };
};

/**
 * Obtiene estadísticas de servicios por categoría
 */
export const getServiceStats = (services: Service[]) => {
  const grouped = groupServicesByCategory(services);
  
  return Object.entries(grouped).map(([category, items]) => ({
    category,
    count: items.length,
    services: items
  })).sort((a, b) => b.count - a.count);
};

/**
 * Exporta servicios a formato CSV
 */
export const exportServicesToCSV = (services: Service[]): string => {
  const headers = ['ID', 'Nombre', 'Descripción', 'Icono'];
  const rows = services.map(service => [
    service.id,
    `"${service.name.replace(/"/g, '""')}"`,
    `"${service.description.replace(/"/g, '""')}"`,
    service.iconKey
  ]);

  const csv = [
    headers.join(','),
    ...rows.map(row => row.join(','))
  ].join('\n');

  return csv;
};

/**
 * Descarga servicios como archivo CSV
 */
export const downloadServicesCSV = (services: Service[], filename: string = 'servicios.csv') => {
  const csv = exportServicesToCSV(services);
  const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
  const link = document.createElement('a');
  const url = URL.createObjectURL(blob);
  
  link.setAttribute('href', url);
  link.setAttribute('download', filename);
  link.style.visibility = 'hidden';
  
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};