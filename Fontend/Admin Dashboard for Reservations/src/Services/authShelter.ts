import { API_URL } from './authService';
import axios from 'axios';

const token = localStorage.getItem('token');

export const getShelters = async () => {
    try {
        const response = await axios.get(`${API_URL}/Shelters`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });
        return response.data;
    } catch (error) {
        console.error('Error al obtener refugios:', error);
        return null;
    }
}

export const getSheltersById = async (id: number) => {
    try {
        const reponse = await axios.get(`${API_URL}/Shelters/${id}`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });
        return reponse.data;
    } catch (error) {
        console.error('Error al obtener refugio por ID:', error);
        return null;
    }
}

export const PostShelter = async (name: string, address: string, latittude: number, 
    longitude: number, phone: number, capacity: number, description: string) => {
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
        return null;
    }
}