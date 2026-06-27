import { useTodo } from '../store/todoStore';
import type { Todo } from '../types/todo';
import { useState, type FormEvent } from 'react';

export const TodoList = () => {
  const { state, fetchTodo, createTodo, updateTodo, deleteTodo, clearSelected } = useTodo();
  const [editingTodo, setEditingTodo] = useState<Todo | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);

  const handleCreateSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const form = e.target as HTMLFormElement;
    const titleInput = form.elements.namedItem('title') as HTMLInputElement;
    const descriptionInput = form.elements.namedItem('description') as HTMLInputElement;
    await createTodo(titleInput.value, descriptionInput.value);
    form.reset();
  };

  const handleUpdateSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!editingTodo) return;
    const form = e.target as HTMLFormElement;
    const titleInput = form.elements.namedItem('title') as HTMLInputElement;
    const descriptionInput = form.elements.namedItem('description') as HTMLInputElement;
    const isCompletedInput = form.elements.namedItem('isCompleted') as HTMLInputElement;
    await updateTodo(editingTodo.id, {
      title: titleInput.value,
      description: descriptionInput.value,
      isCompleted: isCompletedInput.checked,
    });
    setEditingTodo(null);
  };

  const handleViewDetails = async (id: string) => {
    await fetchTodo(id);
    setIsModalOpen(true);
  };

  const getStatusColor = (isCompleted: boolean) => {
    return isCompleted
      ? 'bg-green-500/20 text-green-400 border-green-500/30'
      : 'bg-yellow-500/20 text-yellow-400 border-yellow-500/30';
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric', month: 'short', day: 'numeric',
      hour: '2-digit', minute: '2-digit',
    });
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-[#0f0f13] via-[#16171d] to-[#1a1b23] text-white p-8">
      <div className="max-w-6xl mx-auto">
        <header className="mb-12 text-center">
          <h1 className="text-5xl font-bold mb-3 bg-gradient-to-r from-[#c084fc] to-[#e879f9] bg-clip-text text-transparent">
            Todo App
          </h1>
          <p className="text-gray-400 text-lg">Manage your tasks with elegance</p>
        </header>

        <div className="bg-[#1f2028]/80 backdrop-blur-sm rounded-2xl border border-[#2e303a] shadow-xl p-8 mb-8">
          <h2 className="text-2xl font-semibold mb-6 flex items-center gap-2">
            <span className="w-2 h-2 bg-[#c084fc] rounded-full animate-pulse" />
            Create New Todo
          </h2>
          <form onSubmit={handleCreateSubmit} className="flex flex-col md:flex-row gap-4">
            <div className="flex-1">
              <input
                type="text"
                name="title"
                placeholder="Todo title..."
                className="w-full px-4 py-3 bg-[#16171d] border border-[#2e303a] rounded-lg focus:outline-none focus:border-[#c084fc] focus:ring-1 focus:ring-[#c084fc] text-white placeholder-gray-500 transition-all"
                required
              />
            </div>
            <div className="flex-1">
              <input
                type="text"
                name="description"
                placeholder="Description..."
                className="w-full px-4 py-3 bg-[#16171d] border border-[#2e303a] rounded-lg focus:outline-none focus:border-[#c084fc] focus:ring-1 focus:ring-[#c084fc] text-white placeholder-gray-500 transition-all"
                required
              />
            </div>
            <button
              type="submit"
              className="px-6 py-3 bg-gradient-to-r from-[#c084fc] to-[#e879f9] text-white font-medium rounded-lg hover:scale-105 transition-transform duration-200 shadow-lg hover:shadow-[#c084fc]/20"
            >
              Add Todo
            </button>
          </form>
        </div>

        {state.error && (
          <div className="bg-red-500/10 border border-red-500/30 text-red-400 px-4 py-3 rounded-lg mb-6">
            {state.error}
          </div>
        )}

        {state.loading && state.todos.length === 0 && (
          <div className="flex justify-center items-center py-20">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-[#c084fc]" />
          </div>
        )}

        {!state.loading && state.todos.length === 0 && (
          <div className="text-center py-20 text-gray-500">
            <div className="text-6xl mb-4 opacity-30">📋</div>
            <p className="text-xl">No todos yet. Create one above!</p>
          </div>
        )}

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {state.todos.map(todo => (
            <div
              key={todo.id}
              className="group bg-[#1f2028] rounded-xl border border-[#2e303a] p-6 hover:border-[#c084fc]/40 transition-all duration-300 hover:shadow-lg hover:shadow-[#c084fc]/5"
            >
              <div className="flex justify-between items-start mb-4">
                <h3 className="text-xl font-semibold text-white truncate flex-1 mr-3">
                  {todo.title}
                </h3>
                <span
                  className={`shrink-0 px-3 py-1 rounded-full text-xs font-medium border ${getStatusColor(todo.isCompleted)}`}
                >
                  {todo.isCompleted ? 'Completed' : 'Pending'}
                </span>
              </div>

              <div className="flex flex-col gap-1 text-xs text-gray-500 mb-4">
                <span>Created: {formatDate(todo.createdAt)}</span>
                <span>Updated: {formatDate(todo.updatedAt)}</span>
              </div>

              <div className="flex gap-2">
                <button
                  onClick={() => handleViewDetails(todo.id)}
                  className="flex-1 px-3 py-2 bg-[#16171d] hover:bg-[#c084fc]/10 text-[#c084fc] border border-[#2e303a] rounded-lg transition-all duration-200 text-sm"
                >
                  Details
                </button>
                <button
                  onClick={() => setEditingTodo(todo)}
                  className="flex-1 px-3 py-2 bg-[#16171d] hover:bg-[#c084fc]/10 text-[#c084fc] border border-[#2e303a] rounded-lg transition-all duration-200 text-sm"
                >
                  Edit
                </button>
                <button
                  onClick={() => deleteTodo(todo.id)}
                  className="px-3 py-2 bg-red-500/10 hover:bg-red-500/20 text-red-400 border border-red-500/30 rounded-lg transition-all duration-200 text-sm"
                >
                  Delete
                </button>
              </div>
            </div>
          ))}
        </div>

        {editingTodo && (
          <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center p-4 z-50">
            <div className="bg-[#1f2028] rounded-2xl border border-[#2e303a] p-8 max-w-lg w-full">
              <h2 className="text-2xl font-semibold mb-6">Edit Todo</h2>
              <form onSubmit={handleUpdateSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-2">Title</label>
                  <input
                    type="text"
                    name="title"
                    defaultValue={editingTodo.title}
                    className="w-full px-4 py-3 bg-[#16171d] border border-[#2e303a] rounded-lg focus:outline-none focus:border-[#c084fc] focus:ring-1 focus:ring-[#c084fc] text-white"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-2">Description</label>
                  <textarea
                    name="description"
                    defaultValue={editingTodo.description}
                    rows={4}
                    className="w-full px-4 py-3 bg-[#16171d] border border-[#2e303a] rounded-lg focus:outline-none focus:border-[#c084fc] focus:ring-1 focus:ring-[#c084fc] text-white resize-none"
                    required
                  />
                </div>
                <label className="flex items-center gap-2 cursor-pointer">
                  <input
                    type="checkbox"
                    name="isCompleted"
                    defaultChecked={editingTodo.isCompleted}
                    className="w-4 h-4 rounded border-gray-600 text-[#c084fc] focus:ring-[#c084fc]"
                  />
                  <span className="text-sm font-medium text-gray-300">Completed</span>
                </label>
                <div className="flex gap-3 pt-4">
                  <button
                    type="button"
                    onClick={() => setEditingTodo(null)}
                    className="flex-1 px-4 py-2 bg-[#2e303a] hover:bg-[#3a3b45] text-white rounded-lg transition-colors"
                  >
                    Cancel
                  </button>
                  <button
                    type="submit"
                    className="flex-1 px-4 py-2 bg-gradient-to-r from-[#c084fc] to-[#e879f9] text-white font-medium rounded-lg hover:scale-105 transition-transform duration-200"
                  >
                    Update Todo
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}

        {isModalOpen && state.selectedTodo && (
          <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center p-4 z-50">
            <div className="bg-[#1f2028] rounded-2xl border border-[#2e303a] p-8 max-w-lg w-full">
              <h2 className="text-2xl font-semibold mb-6">Todo Details</h2>
              <div className="space-y-4">
                <div>
                  <label className="block text-xs font-medium text-gray-400 mb-1 uppercase tracking-wider">ID</label>
                  <div className="px-4 py-3 bg-[#16171d] border border-[#2e303a] rounded-lg text-gray-400 font-mono text-sm break-all">
                    {state.selectedTodo.id}
                  </div>
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-400 mb-1 uppercase tracking-wider">Title</label>
                  <div className="px-4 py-3 bg-[#16171d] border border-[#2e303a] rounded-lg text-white">
                    {state.selectedTodo.title}
                  </div>
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-400 mb-1 uppercase tracking-wider">Description</label>
                  <div className="px-4 py-3 bg-[#16171d] border border-[#2e303a] rounded-lg text-white whitespace-pre-wrap">
                    {state.selectedTodo.description || '—'}
                  </div>
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-400 mb-1 uppercase tracking-wider">Status</label>
                  <div className="px-4 py-3 bg-[#16171d] border border-[#2e303a] rounded-lg">
                    <span
                      className={`inline-block px-3 py-1 rounded-full text-xs font-medium border ${getStatusColor(state.selectedTodo.isCompleted)}`}
                    >
                      {state.selectedTodo.isCompleted ? 'Completed' : 'Pending'}
                    </span>
                  </div>
                </div>
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-xs font-medium text-gray-400 mb-1 uppercase tracking-wider">Created</label>
                    <div className="px-4 py-3 bg-[#16171d] border border-[#2e303a] rounded-lg text-gray-300 text-sm">
                      {formatDate(state.selectedTodo.createdAt)}
                    </div>
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-gray-400 mb-1 uppercase tracking-wider">Updated</label>
                    <div className="px-4 py-3 bg-[#16171d] border border-[#2e303a] rounded-lg text-gray-300 text-sm">
                      {formatDate(state.selectedTodo.updatedAt)}
                    </div>
                  </div>
                </div>
                <div className="flex justify-end pt-4">
                  <button
                    onClick={() => { setIsModalOpen(false); clearSelected(); }}
                    className="px-6 py-2 bg-gradient-to-r from-[#c084fc] to-[#e879f9] text-white font-medium rounded-lg hover:scale-105 transition-transform duration-200"
                  >
                    Close
                  </button>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};
