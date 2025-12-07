import React, { useState, useEffect, useCallback } from 'react';
import { TodoItem as TodoItemComponent } from './components/TodoItem';
import { TodoForm } from './components/TodoForm';
import { FilterBar } from './components/FilterBar';
import { todoApi } from './services/todoApi';
import { TodoItem, CreateTodoDto, UpdateTodoDto, FilterType, SortType } from './types/todo.types';
import { CheckCircle2, Loader, AlertCircle } from 'lucide-react';
import './App.css';

function App() {
  const [todos, setTodos] = useState<TodoItem[]>([]);
  const [filteredTodos, setFilteredTodos] = useState<TodoItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string>('');
  const [showForm, setShowForm] = useState(false);
  const [filter, setFilter] = useState<FilterType>('all');
  const [sortBy, setSortBy] = useState<SortType>('createdAt');

  // Load todos ....
  const loadTodos = useCallback(async () => {
    setIsLoading(true);
    setError('');
    try {
      const data = await todoApi.getAll();
      setTodos(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load todos');
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    loadTodos();
  }, [loadTodos]);

  // Filter and sort todos ..................................................
  useEffect(() => {
    let result = [...todos];

    // Apply filter ...........
    switch (filter) {
      case 'active':
        result = result.filter((t) => !t.isCompleted);
        break;
      case 'completed':
        result = result.filter((t) => t.isCompleted);
        break;
      case 'overdue':
        result = result.filter((t) => t.isOverdue);
        break;
      default:
        break;
    }

    // Apply sort ......................................................
    result.sort((a, b) => {
      switch (sortBy) {
        case 'title':
          return a.title.localeCompare(b.title);
        case 'dueDate':
          if (!a.dueDate) return 1;
          if (!b.dueDate) return -1;
          return new Date(a.dueDate).getTime() - new Date(b.dueDate).getTime();
        case 'createdAt':
        default:
          return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
      }
    });

    setFilteredTodos(result);
  }, [todos, filter, sortBy]);

  // Create todo .....................................................
  const handleCreate = async (data: CreateTodoDto) => {
    const newTodo = await todoApi.create(data);
    setTodos((prev) => [newTodo, ...prev]);
    setShowForm(false);
  };

  // Update todo .....................................................
  const handleUpdate = async (id: string, data: UpdateTodoDto) => {
    const updated = await todoApi.update(id, data);
    setTodos((prev) => prev.map((t) => (t.id === id ? updated : t)));
  };

  // Toggle completion ...............................................
  const handleToggleComplete = async (id: string, isCompleted: boolean) => {
    const updated = isCompleted
      ? await todoApi.markCompleted(id)
      : await todoApi.markIncomplete(id);
    setTodos((prev) => prev.map((t) => (t.id === id ? updated : t)));
  };

  // Delete todo ......................................................
  const handleDelete = async (id: string) => {
    await todoApi.delete(id);
    setTodos((prev) => prev.filter((t) => t.id !== id));
  };

  const activeCount = todos.filter((t) => !t.isCompleted).length;
  const completedCount = todos.filter((t) => t.isCompleted).length;

  return (
    <div className="min-h-screen bg-gradient-to-br from-purple-50 to-pink-50">
      <div className="container mx-auto px-4 py-8 max-w-4xl">
        {/* Header *****************/}

        
        <div className="text-center mb-8">
          <h1 className="text-5xl font-bold text-gray-800 mb-2 flex items-center justify-center gap-3">
            Todo List
          </h1>
        </div>

        {/* Error Message ******************/}
        {error && (
          <div className="mb-6 p-4 bg-red-100 border border-red-400 text-red-700 rounded-lg flex items-center gap-2">
            <AlertCircle size={20} />
            <span>{error}</span>
          </div>
        )}

        {/* Add Todo Button **********************/}
        {!showForm && (
          <button
            onClick={() => setShowForm(true)}
            className="w-full mb-6 bg-purple-600 text-white py-3 px-6 rounded-lg hover:bg-purple-700 transition font-medium text-lg shadow-md"
          >
            + Add New Todo
          </button>
        )}

        {/* Todo Form *****************************/}
        {showForm && (
          <div className="mb-6">
            <TodoForm
              onSubmit={handleCreate}
              onCancel={() => setShowForm(false)}
              submitLabel="Add Todo"
            />
          </div>
        )}

        {/* Loading State *****************************/}
        {isLoading ? (
          <div className="flex items-center justify-center py-12">
            <Loader className="animate-spin text-purple-600" size={48} />
          </div>
        ) : (
          <>
            {/* Filter Bar ******************************/}
            <FilterBar
              filter={filter}
              onFilterChange={setFilter}
              sortBy={sortBy}
              onSortChange={setSortBy}
              totalCount={todos.length}
              activeCount={activeCount}
              completedCount={completedCount}
            />

            {/* Todo List ******************************/}
            {filteredTodos.length === 0 ? (
              <div className="text-center py-12 bg-white rounded-lg shadow-md">
                <p className="text-gray-500 text-lg">
                  {todos.length === 0
                    ? 'No todos yet. Create your first one!'
                    : `No ${filter} todos found.`}
                </p>
              </div>
            ) : (
              <div className="space-y-4">
                {filteredTodos.map((todo) => (
                  <TodoItemComponent
                    key={todo.id}
                    todo={todo}
                    onToggleComplete={handleToggleComplete}
                    onDelete={handleDelete}
                    onUpdate={handleUpdate}
                  />
                ))}
              </div>
            )}
          </>
        )}

        {}
        <div className="mt-8 text-center text-gray-600 text-sm">
        </div>
      </div>
    </div>
  );
}

export default App;