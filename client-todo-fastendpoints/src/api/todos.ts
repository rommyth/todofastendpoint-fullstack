import axios from 'axios';
import type { Todo } from '../types/todo';

const API_BASE_URL = 'http://localhost:5030/api';
const TODOS_URL = `${API_BASE_URL}/todos`;

export const todosApi = {
  getAll: async () => {
    const response = await axios.get<Todo[]>(TODOS_URL);
    return response.data;
  },

  getById: async (id: string) => {
    const response = await axios.get<Todo>(`${TODOS_URL}/${id}`);
    return response.data;
  },

  create: async (todo: Omit<Todo, 'id' | 'createdAt' | 'updatedAt'>) => {
    const response = await axios.post<Todo>(TODOS_URL, todo);
    return response.data;
  },

  update: async (id: string, todo: Partial<Todo>) => {
    const response = await axios.put<Todo>(`${TODOS_URL}/${id}`, todo);
    return response.data;
  },

  delete: async (id: string) => {
    await axios.delete(`${TODOS_URL}/${id}`);
  },
};
