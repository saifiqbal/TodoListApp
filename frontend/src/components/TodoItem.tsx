import React, { useState } from 'react';
import { TodoItem as TodoItemType, UpdateTodoDto } from '../types/todo.types';
import { Check, X, Edit2, Trash2, Calendar, Clock } from 'lucide-react';

interface TodoItemProps {
  todo: TodoItemType;
  onToggleComplete: (id: string, isCompleted: boolean) => Promise<void>;
  onDelete: (id: string) => Promise<void>;
  onUpdate: (id: string, data: UpdateTodoDto) => Promise<void>;
}

export const TodoItem: React.FC<TodoItemProps> = ({
  todo,
  onToggleComplete,
  onDelete,
  onUpdate,
}) => {
  const [isEditing, setIsEditing] = useState(false);
  const [editTitle, setEditTitle] = useState(todo.title);
  const [editDescription, setEditDescription] = useState(todo.description || '');
  const [editDueDate, setEditDueDate] = useState(todo.dueDate || '');
  const [isLoading, setIsLoading] = useState(false);

  const handleToggle = async () => {
    setIsLoading(true);
    try {
      await onToggleComplete(todo.id, !todo.isCompleted);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async () => {
    if (window.confirm('Are you sure you want to delete this todo?')) {
      setIsLoading(true);
      try {
        await onDelete(todo.id);
      } finally {
        setIsLoading(false);
      }
    }
  };

  const handleUpdate = async () => {
    if (!editTitle.trim()) return;

    setIsLoading(true);
    try {
      await onUpdate(todo.id, {
        title: editTitle.trim(),
        description: editDescription.trim() || undefined,
        dueDate: editDueDate || undefined,
      });
      setIsEditing(false);
    } finally {
      setIsLoading(false);
    }
  };

  const handleCancelEdit = () => {
    setEditTitle(todo.title);
    setEditDescription(todo.description || '');
    setEditDueDate(todo.dueDate || '');
    setIsEditing(false);
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return null;
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
  };

  if (isEditing) {
    return (
      <div className="bg-white p-4 rounded-lg shadow-md border-2 border-purple-300">
        <input
          type="text"
          value={editTitle}
          onChange={(e) => setEditTitle(e.target.value)}
          className="w-full px-3 py-2 border border-gray-300 rounded mb-2 focus:ring-2 focus:ring-purple-500"
          placeholder="Title"
        />
        <textarea
          value={editDescription}
          onChange={(e) => setEditDescription(e.target.value)}
          className="w-full px-3 py-2 border border-gray-300 rounded mb-2 focus:ring-2 focus:ring-purple-500"
          placeholder="Description"
          rows={2}
        />
        <input
          type="date"
          value={editDueDate}
          onChange={(e) => setEditDueDate(e.target.value)}
          className="w-full px-3 py-2 border border-gray-300 rounded mb-3 focus:ring-2 focus:ring-purple-500"
        />
        <div className="flex gap-2">
          <button
            onClick={handleUpdate}
            disabled={isLoading}
            className="flex-1 bg-green-600 text-white py-2 px-4 rounded hover:bg-green-700 disabled:bg-gray-400 transition flex items-center justify-center"
          >
            <Check size={16} className="mr-1" />
            Save
          </button>
          <button
            onClick={handleCancelEdit}
            disabled={isLoading}
            className="flex-1 bg-gray-200 text-gray-700 py-2 px-4 rounded hover:bg-gray-300 transition flex items-center justify-center"
          >
            <X size={16} className="mr-1" />
            Cancel
          </button>
        </div>
      </div>
    );
  }

  return (
    <div
      className={`bg-white p-4 rounded-lg shadow-md hover:shadow-lg transition ${
        todo.isCompleted ? 'opacity-75' : ''
      } ${todo.isOverdue ? 'border-l-4 border-red-500' : ''}`}
    >
      <div className="flex items-start gap-3">
        <button style={todo.isCompleted?{background:'green'}:{}}
          onClick={handleToggle}
          disabled={isLoading}
          className={`mt-1 flex-shrink-0 w-6 h-6 rounded-full border-2 flex items-center justify-center transition ${
            todo.isCompleted
              ? 'bg-green-500 border-green-500'
              : 'border-gray-300 hover:border-purple-500'
          } disabled:opacity-50`}
        >
          {<Check size={16} className="text-black" />}
        </button>

        <div className="flex-1 min-w-0">
          <h3
            className={`text-lg font-semibold ${
              todo.isCompleted ? 'line-through text-gray-500' : 'text-gray-800'
            }`}
          >
            {todo.title}
          </h3>
          {todo.description && (
            <p className="text-gray-600 text-sm mt-1">{todo.description}</p>
          )}

          <div className="flex flex-wrap gap-3 mt-2 text-xs text-gray-500">
            {todo.dueDate && (
              <span className="flex items-center gap-1">
                <Calendar size={12} />
                Due: {formatDate(todo.dueDate)}
                {todo.isOverdue && (
                  <span className="ml-1 text-red-600 font-semibold">OVERDUE</span>
                )}
              </span>
            )}
            <span className="flex items-center gap-1">
              <Clock size={12} />
              Created: {formatDate(todo.createdAt)}
            </span>
          </div>
        </div>

        <div className="flex gap-2 flex-shrink-0">
          <button
            onClick={() => setIsEditing(true)}
            disabled={isLoading}
            className="p-2 text-blue-600 hover:bg-blue-50 rounded transition"
            title="Edit"
          >
            <Edit2 size={18} />
          </button>
          <button
            onClick={handleDelete}
            disabled={isLoading}
            className="p-2 text-red-600 hover:bg-red-50 rounded transition"
            title="Delete"
          >
            <Trash2 size={18} />
          </button>
        </div>
      </div>
    </div>
  );
};