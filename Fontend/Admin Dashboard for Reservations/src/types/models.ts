/**
 * TIPOS DEL BACKEND - Posada del Peregrino
 * 
 * IMPORTANTE: Estos tipos deben coincidir exactamente con los modelos del backend.
 * Cuando el backend cambie, actualiza este archivo.
 * 
 * Para generar automáticamente desde Swagger/OpenAPI:
 * npx openapi-typescript http://tu-api.com/swagger.json -o types/api.d.ts
 */

// ============================================
// USUARIOS
// ============================================

export interface User {
  id: number;
  name: string;
  dateOfBirth: number;
  email: string;
  password?: string; // No se devuelve en las respuestas GET
  phone: string | null;
  role: 'user' | 'admin';
  shelter_id: number;
  shelterId?: number | null;
  economicLevel: string;
  verified: boolean;
}

export type UserRole = User['role'];

export interface UserCreateDTO {
  name: string;
  age: number;
  email: string;
  password: string;
  phone?: string;
  role: UserRole;
  shelter_id: number;
  economic_level: string;
}

export interface UserUpdateDTO {
  name?: string;
  age?: number;
  email?: string;
  password?: string;
  phone?: string;
  role?: UserRole;
  shelter_id?: number;
  economic_level?: string;
  verified?: boolean;
}

export interface EditUserRequest {
  id: number;
  nombre?: string;
  numero?: string;
  shelterId?: number;
  verificado?: boolean;
  nivelEconomico?: string;
}

export interface CreateUserRequest {
  name: string;
  email: string;
  password: string;
  confirmPassword: string;
  numero: string;
  fechaDeNacimiento: string; // formato: "YYYY-MM-DD"
  shelterId: number;
  nivelEconomico: string;
  verificacion: boolean;
}

// ============================================
// ADMIN
// ============================================

export interface Admin {
  id: string;
  email: string;
  name: string;
  role: 'admin';
  shelter_id: number;
  createdAt: string;
}

export interface AuthContextType {
  admin: Admin | null;
  token: string | null;
  login: (email: string, password: string) => Promise<{ success: boolean; error?: string }>;
  logout: () => void;
  isLoading: boolean;
  isAuthenticated: boolean;
}

// ============================================
// SHELTERS (REFUGIOS)
// ============================================

export interface Shelter {
  id: number;
  name: string;
  address: string;
  latitude: number;
  longitude: number;
  phone: string;
  capacity: number;
  description: string;
  createdAt: string;
  occupancy: number;
}

export interface ShelterCreateDto {
  name: string;
  address: string;
  latitude: number;
  longitude: number;
  phone: string;
  capacity: number;
  description?: string;
}

export interface ShelterUpdateDto {
  id: number;
  name: string;
  address: string;
  latitude: number;
  longitude: number;
  phone: string;
  capacity: number;
  description?: string;
}

// ============================================
// RESERVAS
// ============================================

export interface Reservation {
  id: number;
  user_id: number;
  shelter_id: number;
  resource_id: number;
  start_date: string;
  end_date: string;
  status: ReservationStatus;
  qr_code?: string;
  created_at: string;
  updated_at: string;
}

export type ReservationStatus = 'pending' | 'confirmed' | 'cancelled' | 'completed';

export interface ReservationCreateDTO {
  user_id: number;
  shelter_id: number;
  resource_id: number;
  start_date: string;
  end_date: string;
}

// ============================================
// RECURSOS (SERVICIOS)
// ============================================

export interface Resource {
  id: number;
  shelter_id: number;
  type: ResourceType;
  name: string;
  capacity: number;
  available: number;
  description?: string;
  created_at: string;
}

export type ResourceType = 'hospedaje' | 'comida' | 'regaderas' | 'lavanderia' | 'enfermeria';

// ============================================
// AUTENTICACIÓN
// ============================================

export interface LoginDTO {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface JWTPayload {
  id: string | number;
  email: string;
  name: string;
  role: UserRole;
  shelter_id: number;
  exp: number;
  iat?: number;
}

// ============================================
// RESPUESTAS DE LA API
// ============================================

export interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  error?: string;
  message?: string;
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// ============================================
// FILTROS
// ============================================

export interface UserFilters {
  shelterId?: number;
  role?: UserRole;
  search?: string;
  verified?: boolean;
}

export interface ReservationFilters {
  shelterId?: number;
  userId?: number;
  status?: ReservationStatus;
  startDate?: string;
  endDate?: string;
}

export interface ResourceFilters {
  shelterId?: number;
  type?: ResourceType;
  availableOnly?: boolean;
}


// ============================================
// SERVICIOS
// ============================================
export interface Service {
  id: number;
  name: string;
  description: string;
  iconKey: string;
}

export interface ServiceCreateDto {
  name: string;
  description: string;
  iconKey: string;
}

export interface ServiceUpdateDto {
  id: number;
  name: string;
  description: string;
  iconKey: string;
}