import { API_URL } from './authLogin';
import axios from 'axios';
import type { User, UserFilters, UserCreateDTO, UserUpdateDTO } from '../types/models';

const token = localStorage.getItem('token');

export const getUsers = async () => {
    try{
        const response = await axios.get(`${API_URL}/Users/allUsers`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });
        // Si el backend devuelve { success: true, data: [...] }
        console.log('Respuesta al obtener usuarios:', response.data);
        if (response.data?.data && Array.isArray(response.data.data)) {
        return response.data.data;
        } 
        
        // Si el backend devuelve directamente el array [...]
        if (Array.isArray(response.data)) {
        return response.data;
        }
        console.error('Respuesta inesperada al obtener usuarios:', response.data);
        return [];
    }
    catch(error){
        console.error('Error al obtener usuarios:', error);
        return null;
    }
}

export const getUserByID = async (id: number) => {
    try {
        const response = await axios.get(`${API_URL}/Users/${id}`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });
        return response.data;
    } catch (error) {
        console.error('Error al obtener usuario por ID:', error);
        return null;
    }
}

export const getUsersByShelter = async (id: number): Promise<User[]> => {
  try {
    const response = await axios.get(`${API_URL}/Users/filter-by-shelter/${id}`, {
      headers: { Authorization: `Bearer ${token}` }
    });

    // Igual que en getUsers()
    if (response.data?.data && Array.isArray(response.data.data)) {
      return response.data.data;
    }

    if (Array.isArray(response.data)) {
      return response.data;
    }

    console.error('Respuesta inesperada al obtener usuarios por refugio:', response.data);
    return [];
  } catch (error) {
    console.error('Error al obtener usuarios por refugio:', error);
    return []; // siempre devolver array
  }
};


export const updateUser = async (id: number, data: any) => {
    try {
        const response = await axios.put(`${API_URL}/Auth/update-user/${id}`, data, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });
        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error) && error.response) {
            return error.response.data;
        }
    }
}

// Función auxiliar para filtrar usuarios localmente después de obtenerlos
export const filterUsers = (users: User[], filters: UserFilters): User[] => {
  if (!Array.isArray(users)) {
    console.warn('users no es un array:', users);
    return [];
  }

  return users.filter(user => {
    if (filters.role && user.role !== filters.role) return false;

    if (filters.search) {
      const searchLower = filters.search.toLowerCase();
      const name = user.name?.toLowerCase() || "";
      const email = user.email?.toLowerCase() || "";
      const phone = user.phone?.toLowerCase() || "";

      return (
        name.includes(searchLower) ||
        email.includes(searchLower) ||
        phone.includes(searchLower)
      );
    }

    return true;
  });
};
