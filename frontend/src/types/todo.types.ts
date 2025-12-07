// src/types/todo.types.ts

export interface TodoItem {
  id: string;
  title: string;
  description?: string;
  dueDate?: string;
  isCompleted: boolean;
  createdAt: string;
  updatedAt?: string;
  isOverdue: boolean;
}

export interface CreateTodoDto {
  title: string;
  description?: string;
  dueDate?: string;
}

export interface UpdateTodoDto {
  title?: string;
  description?: string;
  dueDate?: string;
}

export type FilterType = 'all' | 'active' | 'completed' | 'overdue';
export type SortType = 'createdAt' | 'dueDate' | 'title';