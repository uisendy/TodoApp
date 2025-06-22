import { boolean } from 'yup';

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  bio?: string;
}

export interface ApiResponse<T> {
  status: string;
  message: string;
  data?: T;
}

export interface SignUpRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface UpdateUserPayload {
  firstName?: string;
  lastName?: string;
  email?: string;
  phone?: string;
  bio?: string;
}

export interface TodoTag {
  id: string;
  name: string;
}

export interface Todo {
  id: string;
  name: string;
  description?: string;
  isCompleted: boolean;
  isArchived: boolean;
  archivedAt: string;
  priority: number;
  createdAt: string;
  tags: TodoTag[];
}

export type PriorityLevel = 'Low' | 'Medium' | 'High';

export interface TodoPriority {
  name: string;
  value: number;
}

export interface TodoRequest {
  name: string;
  priority: number;
  tags: string[];
}

export type FilterOption = 'all' | 'inProgress' | 'completed' | 'archived';

export interface TodoItemProps {
  todo: Todo;
  markAsComplete: () => void;
  isOpen: boolean;
  onToggleDropDown: () => void;
  onEditClick: () => void;
  showMenuDropdown: boolean;
  showCheckButton: boolean;
}
