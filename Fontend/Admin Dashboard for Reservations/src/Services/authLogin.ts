// src/services/authService.ts
import axios from 'axios';
import type { EditUserRequest } from '../types/models.ts';

export  const API_URL = 'http://localhost:5086/api'; // Ajusta según tu entorno

export const loginUser = async (email: string, password: string) => {
  try {
    const response = await axios.post(`${API_URL}/Auth/login`, {
      email,
      password
    });
    if (response.data.code === '200') {
      const { token, user } = response.data.data;
      // Guardar en localStorage
      localStorage.setItem("token", token);
      localStorage.setItem("user", JSON.stringify(user));
      return { success: true, token, user };
    }

    return { success: false, error: response.data.message };
  } catch (error: any) {
    return { success: false, error: 'Error de conexión con el servidor' };
  }
};

// Completar registro (admin)
export const completeUserRegistration = async (
  email: string,
  phone: string,
  shelterId: number,
  nivelEconomico: string,
  verificacion: boolean
) => {
  try {
    const response = await axios.post(`${API_URL}/Auth/register-user`, {
      email,
      numero: phone,
      shelterId,
      nivelEconomico,
      verificacion,
    }, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('token')}`,
      },
    });

    return response.data;
  } catch (error: any) {
    console.error("Error al completar registro:", error.response?.data || error.message);
    throw error;
  }
};

//Crear admin
export const createAdmin = async (
  name: string,
  email: string,
  password: string,
  passwordAdmin: string
) => {
  const token = localStorage.getItem("token");
  try {
    const adminData = localStorage.getItem("user");
    console.log("Datos del admin actual desde localStorage:", adminData);
    const parsedAdmin = adminData ? JSON.parse(adminData) : null;
    const emailAdmin = parsedAdmin?.email;

    if (!emailAdmin) throw new Error("No se encontró el correo del administrador actual");

    const response = await axios.post(
      `${API_URL}/Auth/register-admin`,
      {
        Nombre: name,
        Email: email,
        Password: password,
        EmailAdmin: emailAdmin,
        PasswordAdmin: passwordAdmin,
      },
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
          },
      }
    );

    if (response.data.code === "201") {
      return { success: true, data: response.data.data };
    }

    return { success: false, message: response.data.message || "Error al crear admin" };
  } catch (error: any) {
    console.error("Error al registrar admin:", error.response?.data || error.message);
    throw new Error(error.response?.data?.message || "Error inesperado al registrar admin");
  }
};

// Verificar usuario directamente
export const verifyUser = async (id: number, verified: boolean) => {
  try {
    const response = await axios.put(`${API_URL}/Auth/verify/${id}`, { verificacion: verified }, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('token')}`,
      },
    });
    return response.data;
  } catch (error: any) {
    console.error("Error al verificar usuario:", error.response?.data || error.message);
    throw error;
  }
};

// Eliminar usuario corregido
export const deleteUser = async (id: number) => {
  try {
    const response = await axios.delete(`${API_URL}/Auth/delete-user`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('token')}`,
        "Content-Type": "application/json", // ✅ Asegura el tipo correcto
      },
      data: JSON.stringify({ id }), // ✅ Enviar como JSON válido
    });

    if (response.data?.ok && response.data?.code === "200") {
      return { success: true, message: "Usuario eliminado exitosamente" };
    } else {
      throw new Error(response.data?.message || "No se pudo eliminar el usuario");
    }
  } catch (error: any) {
    console.error("Error al eliminar usuario:", error.response?.data || error.message);
    return { success: false, message: error.response?.data?.message || error.message };
  }
};

export const editUser = async (data: EditUserRequest) => {
  try {
    const response = await axios.patch(`${API_URL}/Auth/edit-user`, data, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`
      },
    });

    return response.data;
  } catch (error: any) {
    console.error("Error al editar usuario:", error.response?.data || error.message);
    throw new Error(error.response?.data?.message || "Error al editar usuario");
  }
};