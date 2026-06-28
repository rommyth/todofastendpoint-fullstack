import client from './client';
import type { Todo, CreateTodo, UpdateTodo } from '../types/todo';

const TODOS_URL = '/api/todos';

export const todosApi = {
  getAll: async () => {
    const response = await client.get<Todo[]>(TODOS_URL);
    return response.data;
  },

  getById: async (id: string) => {
    const response = await client.get<Todo>(`${TODOS_URL}/${id}`);
    return response.data;
  },

  create: async (todo: CreateTodo) => {
    const response = await client.post<Todo>(TODOS_URL, todo);
    return response.data;
  },

  update: async (id: string, todo: UpdateTodo) => {
    const response = await client.put<Todo>(`${TODOS_URL}/${id}`, todo);
    return response.data;
  },

  delete: async (id: string) => {
    await client.delete(`${TODOS_URL}/${id}`);
  },
};
