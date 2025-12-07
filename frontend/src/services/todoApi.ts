import axios from 'axios';
import { TodoItem, CreateTodoDto, UpdateTodoDto } from '../types/todo.types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const todoApi = {
  // Get all todos with optional filters...............

  getAll: async (isCompleted?: boolean, isOverdue?: boolean): Promise<TodoItem[]> => {
    const params = new URLSearchParams();
    if (isCompleted !== undefined) params.append('isCompleted', String(isCompleted));
    if (isOverdue !== undefined) params.append('isOverdue', String(isOverdue));
    
    const response = await api.get<TodoItem[]>(`/todos?${params.toString()}`);
    return response.data;
  },

  // Get a single todo by ID.............
  getById: async (id: string): Promise<TodoItem> => {
    const response = await api.get<TodoItem>(`/todos/${id}`);
    return response.data;
  },

  create: async (todo: CreateTodoDto): Promise<TodoItem> => {
    const response = await api.post<TodoItem>('/todos', todo);
    return response.data;
  },

  // Update an existing todo ...........
  update: async (id: string, todo: UpdateTodoDto): Promise<TodoItem> => {
    const response = await api.put<TodoItem>(`/todos/${id}`, todo);
    return response.data;
  },

  markCompleted: async (id: string): Promise<TodoItem> => {
    const response = await api.patch<TodoItem>(`/todos/${id}/complete`);
    return response.data;
  },

  markIncomplete: async (id: string): Promise<TodoItem> => {
    const response = await api.patch<TodoItem>(`/todos/${id}/incomplete`);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/todos/${id}`);
  },
};

// Error handling interceptor........

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      const message = error.response.data?.message || error.response.data?.error || 'An error occurred';
      throw new Error(message);
    } else if (error.request) {
      throw new Error('Unable to connect to server. Please check if the API is running.');
    } else {
      throw new Error(error.message || 'An unexpected error occurred');
    }
  }
);