import { useState } from 'react';
import Badge from '../ui/badge/Badge';
import { useAppSelector } from '../../hooks/useAppSelector';
import { useAppDispatch } from '../../hooks/useAppDispatch';
import { setFilter } from '../../features/todo/todoSlice';

const TodoListChartTab: React.FC = () => {
  const [selected, setSelected] = useState<
    'optionOne' | 'optionTwo' | 'optionThree' | 'optionFour'
  >('optionOne');

  const dispatch = useAppDispatch();
  const { todos } = useAppSelector(state => state.todos);

  const handleSelect = (
    option: 'optionOne' | 'optionTwo' | 'optionThree' | 'optionFour',
    filterValue: 'all' | 'inProgress' | 'completed' | 'archived'
  ) => {
    setSelected(option);
    dispatch(setFilter(filterValue)); // 👈 dispatch filter
  };

  const getButtonClass = (option: typeof selected) =>
    selected === option
      ? 'shadow-theme-xs text-gray-900 dark:text-white bg-white dark:bg-gray-800'
      : 'text-gray-500 dark:text-gray-400';

  const allTodos = todos;
  const inProgressTodos = todos.filter(
    todo => !todo.isCompleted && !todo.isArchived
  );
  const completedTodos = todos.filter(todo => todo.isCompleted);
  const archivedTodos = todos.filter(todo => todo.isArchived);

  return (
    <div className="flex items-center gap-0.5 rounded-lg bg-gray-100 p-0.5 dark:bg-gray-900">
      <button
        onClick={() => handleSelect('optionOne', 'all')}
        className={`flex flex-row gap-1 justify-between items-center px-3 py-2 font-medium w-full rounded-md text-theme-sm hover:text-gray-900 dark:hover:text-white ${getButtonClass(
          'optionOne'
        )}`}
      >
        <span>All</span>
        <span>Todo</span>
        <Badge variant="light" color="dark" size="sm">
          {allTodos.length}
        </Badge>
      </button>

      <button
        onClick={() => handleSelect('optionTwo', 'inProgress')}
        className={`flex flex-row gap-1 justify-between items-center px-3 py-2 font-medium w-full rounded-md text-theme-sm hover:text-gray-900 dark:hover:text-white ${getButtonClass(
          'optionTwo'
        )}`}
      >
        <span>In</span>
        <span>Progress</span>
        <Badge variant="solid" color="warning" size="sm">
          {inProgressTodos.length}
        </Badge>
      </button>

      <button
        onClick={() => handleSelect('optionThree', 'completed')}
        className={`hidden md:flex flex-row gap-1 justify-between items-center px-3 py-2 font-medium w-full rounded-md text-theme-sm hover:text-gray-900 dark:hover:text-white ${getButtonClass(
          'optionThree'
        )}`}
      >
        <span>Completed</span>
        <Badge variant="solid" color="success" size="sm">
          {completedTodos.length}
        </Badge>
      </button>

      <button
        onClick={() => handleSelect('optionFour', 'archived')}
        className={`hidden md:flex flex-row gap-1 justify-between items-center px-3 py-2 font-medium w-full rounded-md text-theme-sm hover:text-gray-900 dark:hover:text-white ${getButtonClass(
          'optionFour'
        )}`}
      >
        <span>Archived</span>
        <Badge variant="solid" color="error" size="sm">
          {archivedTodos.length}
        </Badge>
      </button>
    </div>
  );
};

export default TodoListChartTab;
