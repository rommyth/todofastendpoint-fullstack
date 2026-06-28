export interface Todo {
  id: string;
  title: string;
  description: string;
  isCompleted: boolean;
  createdAt: string;
  updatedAt: string;
  userId?: string;
  userName?: string;
}

export type CreateTodo = Omit<Todo, 'id' | 'createdAt' | 'updatedAt' | 'userId' | 'userName'>;

export type UpdateTodo = Partial<Omit<Todo, 'id' | 'createdAt' | 'updatedAt' | 'userId' | 'userName'>>;
