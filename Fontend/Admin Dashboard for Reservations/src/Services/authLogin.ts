// src/services/authService.ts
import axios from 'axios';

export  const API_URL = 'http://localhost:5086/api'; // Ajusta según tu entorno

export const loginUser = async (email: string, password: string) => {
  try {
    const response = await axios.post(`${API_URL}/Auth/login`, {
      email,
      password
    });
    console.log(response.data);
    if (response.data.code === '200') {
      const { token, user } = response.data.data;
      console.log(user);
      return { success: true, token, user };
    }

    return { success: false, error: response.data.message };
  } catch (error: any) {
    return { success: false, error: 'Error de conexión con el servidor' };
  }
};

export const registerUser = async (name: string, email: string, password: string) => {
  try {
    const response = await axios.post(`${API_URL}/Auth/register`, { name, email, password });
    const { token, user } = response.data.data;

    localStorage.setItem('token', token);
    localStorage.setItem('user', JSON.stringify(user));
    if (response.data.code === '201') {
      return { success: true, token, user };
    }
  } catch (error: any) {
    console.error('Error al registrar usuario:', error.response?.data?.message || error.message);
    throw new Error(error.response?.data?.message || 'Error inesperado en el registro');
  }
}

export const registerAdmin = async (name: string, email: string, password: string, passwordAdmin: string) => {
  try {
    const adminData = localStorage.getItem('user');
    const parsedAdmin = adminData ? JSON.parse(adminData) : null;
    const emailAdmin = parsedAdmin?.email;

    if (!emailAdmin) {
      throw new Error('No se encontró el correo del administrador actual');
    }

    const response = await axios.post(`${API_URL}/Auth/register-admin`, {
      name,
      email,
      password,
      emailAdmin,
      passwordAdmin,
    });

    const { token, user } = response.data.data;

    localStorage.setItem('token', token);
    localStorage.setItem('user', JSON.stringify(user));

    if (response.data.code === '201') {
      return { success: true, token, user };
    }
  } catch (error: any) {
    console.error('Error al registrar admin:', error.response?.data?.message || error.message);
    throw new Error(error.response?.data?.message || 'Error inesperado en el registro de admin');
  }
};
