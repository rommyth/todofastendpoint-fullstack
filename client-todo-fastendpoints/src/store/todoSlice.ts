import type { Todo, CreateTodo, UpdateTodo } from "../types/todo";

export const TODO_ACTIONS = {
  GET_ALL: "GET_ALL",
  GET_ALL_SUCCESS: "GET_ALL_SUCCESS",
  GET_ALL_ERROR: "GET_ALL_ERROR",
  GET_SINGLE: "GET_SINGLE",
  GET_SINGLE_SUCCESS: "GET_SINGLE_SUCCESS",
  GET_SINGLE_ERROR: "GET_SINGLE_ERROR",
  CREATE: "CREATE",
  CREATE_SUCCESS: "CREATE_SUCCESS",
  CREATE_ERROR: "CREATE_ERROR",
  UPDATE: "UPDATE",
  UPDATE_SUCCESS: "UPDATE_SUCCESS",
  UPDATE_ERROR: "UPDATE_ERROR",
  DELETE: "DELETE",
  DELETE_SUCCESS: "DELETE_SUCCESS",
  DELETE_ERROR: "DELETE_ERROR",
} as const;

interface TodoState {
  todos: Todo[];
  selectedTodo: Todo | null;
  loading: boolean;
  creating: boolean;
  updating: boolean;
  deleting: boolean;
  error: string | null;
}

interface TodoAction {
  type: string;
  payload?: any;
}

const initialState: TodoState = {
  todos: [],
  selectedTodo: null,
  loading: false,
  creating: false,
  updating: false,
  deleting: false,
  error: null,
};

export const todoReducer = (
  state = initialState,
  action: TodoAction,
): TodoState => {
  switch (action.type) {
    case TODO_ACTIONS.GET_ALL:
    case TODO_ACTIONS.GET_SINGLE:
    case TODO_ACTIONS.CREATE:
    case TODO_ACTIONS.UPDATE:
    case TODO_ACTIONS.DELETE:
      return { ...state, loading: true, error: null };

    case TODO_ACTIONS.GET_ALL_SUCCESS:
      return { ...state, loading: false, todos: action.payload };

    case TODO_ACTIONS.GET_SINGLE_SUCCESS:
    case TODO_ACTIONS.CREATE_SUCCESS:
    case TODO_ACTIONS.UPDATE_SUCCESS:
      const updatedTodo = action.payload as Todo;
      if (action.type === TODO_ACTIONS.GET_SINGLE_SUCCESS) {
        return { ...state, loading: false, selectedTodo: updatedTodo };
      }
      return {
        ...state,
        loading: false,
        todos: updateTodoInList(state.todos, updatedTodo),
      };

    case TODO_ACTIONS.GET_ALL_ERROR:
    case TODO_ACTIONS.GET_SINGLE_ERROR:
    case TODO_ACTIONS.CREATE_ERROR:
    case TODO_ACTIONS.UPDATE_ERROR:
    case TODO_ACTIONS.DELETE_ERROR:
      return { ...state, loading: false, error: action.payload };

    case TODO_ACTIONS.DELETE_SUCCESS:
      const deletedId = action.payload as string;
      return {
        ...state,
        loading: false,
        todos: state.todos.filter((todo) => todo.id !== deletedId),
      };

    case "SELECT_TODO":
      return { ...state, loading: false, selectedTodo: action.payload };

    default:
      return state;
  }
};

const updateTodoInList = (todos: Todo[], updatedTodo: Todo): Todo[] => {
  const index = todos.findIndex((todo) => todo.id === updatedTodo.id);
  if (index === -1) {
    return [...todos, updatedTodo];
  }
  return [...todos.slice(0, index), updatedTodo, ...todos.slice(index + 1)];
};

export const todoActions = {
  getAll: () => ({ type: TODO_ACTIONS.GET_ALL }),
  create: (todo: CreateTodo) => ({
    type: TODO_ACTIONS.CREATE,
    payload: todo,
  }),
  update: (id: string, todo: UpdateTodo) => ({
    type: TODO_ACTIONS.UPDATE,
    payload: { id, todo },
  }),
  delete: (id: string) => ({ type: TODO_ACTIONS.DELETE, payload: id }),
  select: (todo: Todo | null) => ({ type: "SELECT_TODO", payload: todo }),
};
