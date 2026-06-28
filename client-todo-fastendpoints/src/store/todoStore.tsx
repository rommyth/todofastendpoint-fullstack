import { createContext, useContext, type ReactNode, useState, useEffect, useCallback } from 'react';
import { todosApi } from '../api/todos';
import type { Todo, UpdateTodo } from '../types/todo';

interface TodoState {
  todos: Todo[];
  selectedTodo: Todo | null;
  loading: boolean;
  error: string | null;
}

interface TodoContextValue {
  state: TodoState;
  fetchTodos: () => Promise<void>;
  fetchTodo: (id: string) => Promise<void>;
  createTodo: (title: string, description: string) => Promise<void>;
  updateTodo: (id: string, updates: UpdateTodo) => Promise<void>;
  deleteTodo: (id: string) => Promise<void>;
  clearSelected: () => void;
}

const TodoContext = createContext<TodoContextValue | undefined>(undefined);

export const TodoProvider = ({ children }: { children: ReactNode }) => {
  const [state, setState] = useState<TodoState>({
    todos: [],
    selectedTodo: null,
    loading: false,
    error: null,
  });

  const setLoading = () => setState(s => ({ ...s, loading: true, error: null }));
  const setSuccess = (updates: Partial<TodoState>) => setState(s => ({ ...s, loading: false, ...updates }));
  const setError = (error: string) => setState(s => ({ ...s, loading: false, error }));

  const fetchTodos = useCallback(async () => {
    setLoading();
    try {
      const todos = await todosApi.getAll();
      setSuccess({ todos });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch todos');
    }
  }, []);

  const fetchTodo = useCallback(async (id: string) => {
    setLoading();
    try {
      const todo = await todosApi.getById(id);
      setSuccess({ selectedTodo: todo });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch todo');
    }
  }, []);

  const createTodo = useCallback(async (title: string, description: string) => {
    setLoading();
    try {
      await todosApi.create({ title, description, isCompleted: false });
      await fetchTodos();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create todo');
    }
  }, [fetchTodos]);

  const updateTodo = useCallback(async (id: string, updates: UpdateTodo) => {
    setLoading();
    try {
      await todosApi.update(id, updates);
      await fetchTodos();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update todo');
    }
  }, [fetchTodos]);

  const deleteTodo = useCallback(async (id: string) => {
    setLoading();
    try {
      await todosApi.delete(id);
      await fetchTodos();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete todo');
    }
  }, [fetchTodos]);

  const clearSelected = useCallback(() => {
    setState(s => ({ ...s, selectedTodo: null }));
  }, []);

  useEffect(() => { fetchTodos(); }, [fetchTodos]);

  return (
    <TodoContext.Provider
      value={{ state, fetchTodos, fetchTodo, createTodo, updateTodo, deleteTodo, clearSelected }}
    >
      {children}
    </TodoContext.Provider>
  );
};

export const useTodo = () => {
  const context = useContext(TodoContext);
  if (!context) throw new Error('useTodo must be used within TodoProvider');
  return context;
};
