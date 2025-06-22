// features/todos/todoSlice.ts
import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState, AppDispatch } from '../../app/store';
import {
  TodoPriority,
  TodoRequest,
  Todo,
  TodoTag,
  FilterOption,
} from '../../types';
import { authorizedRequest } from '../auth/authorizedRequest';

interface TodoState {
  todos: Todo[];
  priorities: TodoPriority[];
  filter: FilterOption;
  selectedTodo: Todo | null;
  tags: TodoTag[];
  status: 'idle' | 'loading' | 'succeeded' | 'failed';
  error: string | null;
}

const initialState: TodoState = {
  todos: [],
  selectedTodo: null,
  filter: 'all',
  priorities: [],
  tags: [],
  status: 'idle',
  error: null,
};

export const fetchTodos = createAsyncThunk<
  Todo[],
  void,
  { state: RootState; dispatch: AppDispatch }
>('todos/fetchTodos', async (_, { getState, dispatch, rejectWithValue }) => {
  try {
    const data = await authorizedRequest(
      { url: '/api/v1/todos', method: 'GET' },
      getState,
      dispatch
    );
    return data.data;
  } catch (error: any) {
    return rejectWithValue(
      error.response?.data?.message || 'Error fetching Todos'
    );
  }
});

export const fetchTodoById = createAsyncThunk<
  Todo,
  string,
  { state: RootState; dispatch: AppDispatch }
>(
  'todos/fetchTodoById',
  async (id, { getState, dispatch, rejectWithValue }) => {
    try {
      const data = await authorizedRequest(
        { url: `/api/v1/todos/${id}`, method: 'GET' },
        getState,
        dispatch
      );
      return data.data;
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Error fetching Todo'
      );
    }
  }
);

export const createTodo = createAsyncThunk<
  Todo,
  TodoRequest,
  { state: RootState; dispatch: AppDispatch }
>(
  'todos/createTodo',
  async (request, { getState, dispatch, rejectWithValue }) => {
    try {
      const data = await authorizedRequest(
        { url: '/api/v1/todos', method: 'POST', body: request },
        getState,
        dispatch
      );
      return data.data;
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Error Creating Todo'
      );
    }
  }
);

export const updateTodo = createAsyncThunk<
  Todo,
  { id: string; request: TodoRequest },
  { state: RootState; dispatch: AppDispatch }
>(
  'todos/updateTodo',
  async ({ id, request }, { getState, dispatch, rejectWithValue }) => {
    try {
      const data = await authorizedRequest(
        { url: `/api/v1/todos/${id}`, method: 'PUT', body: request },
        getState,
        dispatch
      );
      return data.data;
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Error updating Todo'
      );
    }
  }
);

export const toggleTodoCompletion = createAsyncThunk<
  Todo,
  string,
  { state: RootState; dispatch: AppDispatch }
>(
  'todos/toggleTodoCompletion',
  async (id, { getState, dispatch, rejectWithValue }) => {
    try {
      const data = await authorizedRequest(
        { url: `/api/v1/todos/${id}/toggle-completion`, method: 'PATCH' },
        getState,
        dispatch
      );
      return data.data;
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Error Completing Todo'
      );
    }
  }
);

export const archiveTodo = createAsyncThunk<
  string,
  string,
  { state: RootState; dispatch: AppDispatch }
>('todos/archiveTodo', async (id, { getState, dispatch, rejectWithValue }) => {
  try {
    const data = await authorizedRequest(
      { url: `/api/v1/todos/${id}/archive`, method: 'PUT' },
      getState,
      dispatch
    );
    return data.data;
  } catch (error: any) {
    return rejectWithValue(
      error.response?.data?.message || 'Error archiving Todo'
    );
  }
});

export const fetchPriorities = createAsyncThunk<
  TodoPriority[],
  void,
  { state: RootState; dispatch: AppDispatch }
>(
  'todos/fetchPriorities',
  async (_, { getState, dispatch, rejectWithValue }) => {
    try {
      const data = await authorizedRequest(
        { url: '/api/v1/todos/priorities', method: 'GET' },
        getState,
        dispatch
      );
      return data.data;
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Error fetching Priorities'
      );
    }
  }
);

export const fetchTags = createAsyncThunk<
  TodoTag[],
  void,
  { state: RootState; dispatch: AppDispatch }
>('todos/fetchTags', async (_, { getState, dispatch, rejectWithValue }) => {
  try {
    const data = await authorizedRequest(
      { url: '/api/v1/todos/tags', method: 'GET' },
      getState,
      dispatch
    );
    return data.data;
  } catch (error: any) {
    return rejectWithValue(
      error.response?.data?.message || 'Error fetching Tags'
    );
  }
});

const todoSlice = createSlice({
  name: 'todos',
  initialState,
  reducers: {
    setTodos(state, action: PayloadAction<Todo[]>) {
      state.todos = action.payload;
      state.status = 'succeeded';
    },
    setFilter(state, action: PayloadAction<FilterOption>) {
      state.filter = action.payload;
    },
    // ...
  },
  extraReducers: builder => {
    builder
      .addCase(fetchTodos.fulfilled, (state, action) => {
        state.todos = action.payload;
        state.status = 'succeeded';
      })
      .addCase(fetchTodoById.fulfilled, (state, action) => {
        state.selectedTodo = action.payload;
        state.status = 'succeeded';
      })
      .addCase(createTodo.fulfilled, (state, action) => {
        state.todos.push(action.payload);
        state.status = 'succeeded';
      })
      .addCase(updateTodo.fulfilled, (state, action) => {
        const index = state.todos.findIndex(
          todo => todo.id === action.payload.id
        );
        if (index !== -1) state.todos[index] = action.payload;
        state.status = 'succeeded';
      })
      .addCase(toggleTodoCompletion.fulfilled, (state, action) => {
        const index = state.todos.findIndex(
          todo => todo.id === action.payload.id
        );
        if (index !== -1) state.todos[index] = action.payload;
        state.status = 'succeeded';
      })
      .addCase(archiveTodo.fulfilled, (state, action) => {
        state.todos = state.todos.filter(todo => todo.id !== action.meta.arg);
        state.status = 'succeeded';
      })
      .addCase(fetchPriorities.fulfilled, (state, action) => {
        state.priorities = action.payload;
        state.status = 'succeeded';
      })
      .addCase(fetchTags.fulfilled, (state, action) => {
        state.tags = action.payload;
        state.status = 'succeeded';
      })
      .addMatcher(
        action =>
          action.type.startsWith('todos/') && action.type.endsWith('/pending'),
        state => {
          state.status = 'loading';
          state.error = null;
        }
      )
      .addMatcher(
        action =>
          action.type.startsWith('todos/') && action.type.endsWith('/rejected'),
        (state, _) => {
          state.status = 'failed';
          state.error = 'An Unknown Error Occurred';
        }
      );
  },
});

export const selectFilteredTodos = (state: RootState) => {
  const filter = state.todos.filter;
  const todos = state.todos.todos;

  switch (filter) {
    case 'inProgress':
      return todos.filter(t => !t.isCompleted && !t.isArchived);
    case 'completed':
      return todos.filter(t => t.isCompleted);
    case 'archived':
      return todos.filter(t => t.isArchived);
    default:
      return todos;
  }
};

export const { setTodos, setFilter } = todoSlice.actions;
export default todoSlice.reducer;
