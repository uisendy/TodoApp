import Checkbox from '../form/input/Checkbox';
import Badge from '../ui/badge/Badge';
import { MoreDotIcon } from '../../icons';
import { Dropdown } from '../ui/dropdown/Dropdown';
import { DropdownItem } from '../ui/dropdown/DropdownItem';
import { TodoItemProps } from '../../types';
import { priorityMap } from '../../utils/priorityMapper';
import { smartDateTime } from '../../utils/smartDateTime';
import { archiveTodo } from '../../features/todo/todoSlice';
import { useAppDispatch } from '../../hooks/useAppDispatch';

export default function TodoItem({
  todo,
  markAsComplete,
  isOpen,
  onToggleDropDown,
  onEditClick,
  showMenuDropdown,
  showCheckButton,
}: TodoItemProps) {
  const dispatch = useAppDispatch();

  const handleArchive = async () => {
    await dispatch(archiveTodo(todo.id));
  };

  return (
    <div className="rounded-xl border border-gray-200 bg-white px-2 pb-1 pt-1 my-1 dark:border-gray-800 dark:bg-white/[0.03] md:px-5 md:py-3 md:my-2">
      <div className="flex flex-row justify-between items-center">
        <div className="flex items-center gap-4">
          {showCheckButton && (
            <Checkbox checked={todo?.isCompleted} onChange={markAsComplete} />
          )}
          {todo?.isCompleted ? (
            <div className="flex flex-row justify-center w-20">
              <Badge variant="light" color="dark" size="sm">
                Completed
              </Badge>
            </div>
          ) : (
            <div className="flex flex-row justify-center w-20">
              <Badge
                variant="light"
                color={
                  todo.priority === 2
                    ? 'error'
                    : todo.priority === 1
                    ? 'info'
                    : 'light'
                }
                size="sm"
              >
                {`${priorityMap[todo.priority] || 'Low'}`}
              </Badge>
            </div>
          )}
          <div>
            <p
              className={`mt-1 text-gray-500 text-theme-sm dark:text-gray-400 ${
                todo.isCompleted &&
                'text-gray-400 dark:text-gray-700 line-through'
              }`}
            >
              {todo.name}
            </p>
          </div>
        </div>
        <div className="flex items-center gap-2">
          {todo.tags &&
            todo.tags.length > 0 &&
            todo?.tags?.map((tag: any) => (
              <Badge key={tag.id} variant="light" color="success" size="sm">
                {tag.name}
              </Badge>
            ))}
          <p className="mt-1 text-gray-500 text-theme-sm dark:text-gray-400">
            {smartDateTime(todo.createdAt)}
          </p>
          {showMenuDropdown && (
            <button className="dropdown-toggle" onClick={onToggleDropDown}>
              <MoreDotIcon className="text-gray-400 hover:text-gray-700 dark:hover:text-gray-300 size-6" />
            </button>
          )}
          <Dropdown
            isOpen={isOpen}
            onClose={onToggleDropDown}
            className="w-40 p-2"
          >
            <DropdownItem
              onItemClick={onEditClick}
              className="flex w-full font-normal text-left text-gray-500 rounded-lg hover:bg-gray-100 hover:text-gray-700 dark:text-gray-400 dark:hover:bg-white/5 dark:hover:text-gray-300"
            >
              Edit
            </DropdownItem>
            <DropdownItem
              onItemClick={handleArchive}
              className="flex w-full font-normal text-left text-gray-500 rounded-lg hover:bg-gray-100 hover:text-gray-700 dark:text-gray-400 dark:hover:bg-white/5 dark:hover:text-gray-300"
            >
              Archive
            </DropdownItem>
          </Dropdown>
        </div>
      </div>
    </div>
  );
}
