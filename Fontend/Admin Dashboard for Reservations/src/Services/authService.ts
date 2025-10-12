// src/services/authService.ts
import axios from 'axios';

const API_URL = 'http://localhost:5086/api'; // Ajusta según tu entorno

export const loginUser = async (email: string, password: string) => {
  try {
    const response = await axios.post(`${API_URL}/Users/login`, { email, password });

    if (response.data.code === '200') {
      const { token, user } = response.data.data;
      return { success: true, token, user };
    }

    return { success: false, error: response.data.message };
  } catch (error: any) {
    return { success: false, error: 'Error de conexión con el servidor' };
  }
};
