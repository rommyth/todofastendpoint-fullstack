import { TodoList } from "./components/TodoList";
import { TodoProvider } from "./store/todoStore";

export default function App() {
  return (
    <TodoProvider>
      <TodoList />
    </TodoProvider>
  );
}
