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
        return response.data.data || response.data || [];
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

export const getUsersByShelter = async (id: number) => {
    try {
        const response = await axios.get(`${API_URL}/Users/filter-by-shelter/${id}`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });
        return response.data;
    } catch (error) {
        console.error('Error al obtener usuarios por refugio:', error);
        return null;
    }
}

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

export const deleteUser = async (id: number) => {
    try {
        const response = await axios.delete(`${API_URL}/Auth/delete-user/${id}`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });
        return response.data;
        } catch (error) {
        if (axios.isAxiosError(error) && error.response) {
        }
    }
}

// Función auxiliar para filtrar usuarios localmente después de obtenerlos
export const filterUsers = (users: User[], filters: UserFilters): User[] => {
  return users.filter(user => {
    if (filters.role && user.role !== filters.role) return false;
    if (filters.search) {
      const searchLower = filters.search.toLowerCase();
      return (
        user.name.toLowerCase().includes(searchLower) ||
        user.email.toLowerCase().includes(searchLower) ||
        (user.phone && user.phone.toLowerCase().includes(searchLower))
      );
    }
    return true;
  });
};