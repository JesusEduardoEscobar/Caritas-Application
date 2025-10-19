import { API_URL } from './authLogin';
import axios from 'axios';
import type { Shelter } from '../types/models.ts';

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

export const getSheltersById = async (id: number) : Promise<Shelter>=> {
    try {
        const reponse = await axios.get(`${API_URL}/Shelters/${id}`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });
        return reponse.data;
    } catch (error) {
        console.error('Error al obtener refugio por ID:', error);
        throw error;
    }
}

export const PostShelter = async (name: string, address: string, latittude: number, 
    longitude: number, phone: number, capacity: number, description: string) : Promise<Shelter>=> {
    try {
        const response = await axios.post(`${API_URL}/Shelters`, 
        { name, address, latittude, longitude, phone, capacity, description },
        {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });
        return response.data;
    } catch (error) {
        console.error('Error al crear refugio:', error);
        throw error;
    }
}