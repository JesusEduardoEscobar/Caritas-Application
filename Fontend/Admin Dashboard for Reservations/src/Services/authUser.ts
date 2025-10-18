import { API_URL } from './authLogin';
import axios from 'axios';

const token = localStorage.getItem('token');

export const getUsers = async () => {
    try{
        const response = await axios.get(`${API_URL}/Users/allUsers`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });
        return response.data;
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

export const getUserByShelter = async (id: number) => {
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

