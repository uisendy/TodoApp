import PageMeta from '../../components/common/PageMeta';
import TodoMetric from '../../components/todo/TodoMetric';
import ProgressMeter from '../../components/todo/ProgressMeter';
import TodoListChartTab from '../../components/common/TodoListChartTab';
import TodoItem from '../../components/todo/TodoItem';
import { useAppDispatch } from '../../hooks/useAppDispatch';
import { useAppSelector } from '../../hooks/useAppSelector';
import { MdFullscreen } from 'react-icons/md';
import { RiProgress7Line } from 'react-icons/ri';
import { IoMdCheckbox } from 'react-icons/io';
import { RiInboxArchiveFill } from 'react-icons/ri';
import {
  fetchTodos,
  selectFilteredTodos,
  toggleTodoCompletion,
} from '../../features/todo/todoSlice';
import { useEffect, useState } from 'react';
import { MdTouchApp } from 'react-icons/md';
import { Link } from 'react-router';

export default function Home() {
  const dispatch = useAppDispatch();
  const { status } = useAppSelector(state => state.todos);
  const todos = useAppSelector(selectFilteredTodos);

  const [openDropdownId, setOpenDropdownId] = useState<string | null>(null);

  const allTodos = todos;
  const inProgressTodos = todos.filter(
    todo => !todo.isCompleted && !todo.isArchived
  );
  const completedTodos = todos.filter(todo => todo.isCompleted);
  const archivedTodos = todos.filter(todo => todo.isArchived);

  const progress = (completedTodos.length / allTodos.length) * 100;

  useEffect(() => {
    dispatch(fetchTodos());
  }, [dispatch]);

  return (
    <>
      <PageMeta title="TodoIO - Home" description="Your Todo, Your Progress" />
      <div className="grid grid-cols-12 gap-4 md:gap-6">
        <div className="col-span-12 space-y-6 xl:col-span-7">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 md:gap-6">
            <TodoMetric
              label="All Todo"
              metric={allTodos.length.toString()}
              icon={
                <MdFullscreen className="text-gray-800 size-6 dark:text-white/90" />
              }
            />
            <TodoMetric
              label="In Progress"
              metric={inProgressTodos.length.toString()}
              icon={
                <RiProgress7Line className="text-gray-800 size-6 dark:text-white/90" />
              }
            />
            <TodoMetric
              label="Completed"
              metric={completedTodos.length.toString()}
              icon={
                <IoMdCheckbox className="text-gray-800 size-6 dark:text-white/90" />
              }
            />
            <TodoMetric
              label="Archived"
              metric={archivedTodos.length.toString()}
              icon={
                <RiInboxArchiveFill className="text-gray-800 size-6 dark:text-white/90" />
              }
            />
          </div>
        </div>

        <div className="col-span-12 xl:col-span-5">
          <ProgressMeter
            progress={Number(progress.toFixed(2))}
            all={allTodos?.length?.toString()}
            open={inProgressTodos?.length?.toString()}
            completed={completedTodos?.length.toString()}
          />
        </div>

        <div className="col-span-12">
          <div>
            <div className="rounded-2xl border border-gray-200 bg-white px-5 pb-5 pt-5 dark:border-gray-800 dark:bg-white/[0.03] sm:px-6 sm:pt-6">
              <div className="flex flex-col sm:flex-row justify-between sm:justify-between pb-6 mb-3 items-center gap-2">
                <h3 className="text-lg font-semibold text-gray-800 dark:text-white/90">
                  Recent Todo
                </h3>
              </div>
              <div className="flex flex-col sm:flex-row justify-between sm:justify-between pb-6 mb-3 items-center gap-2">
                <TodoListChartTab />

                <Link
                  to="/todo"
                  className="border border-gray-200 dark:border-gray-800 text-left text-sm text-gray-700 flex items-center gap-3 px-3 py-2 font-medium  rounded-lg group hover:bg-gray-100 hover:text-gray-700 dark:text-gray-400 dark:hover:bg-white/5 dark:hover:text-gray-300"
                >
                  <MdTouchApp />
                  Manage
                </Link>
              </div>

              <div className="max-w-full overflow-x-auto custom-scrollbar">
                <div className="min-w-[1000px] xl:min-w-full">
                  {status === 'loading' && (
                    <p className="my-5 text-gray-600 dark:text-gray-400">
                      Loading Todos . . .
                    </p>
                  )}
                  {status === 'succeeded' && todos.length === 0 && (
                    <p className="my-7 text-gray-600 dark:text-gray-400">
                      No todos found for this filter.
                    </p>
                  )}
                  {status === 'succeeded' &&
                    todos.map(todo => (
                      <TodoItem
                        todo={todo}
                        key={todo.id}
                        markAsComplete={() =>
                          dispatch(toggleTodoCompletion(todo.id))
                        }
                        isOpen={openDropdownId === todo.id}
                        onToggleDropDown={() =>
                          setOpenDropdownId(prev =>
                            prev === todo.id ? null : todo.id
                          )
                        }
                        onEditClick={() => {
                          setOpenDropdownId(null);
                        }}
                        showMenuDropdown={false}
                        showCheckButton={false}
                      />
                    ))}
                  {status === 'failed' && (
                    <p className="my-10 text-gray-600 dark:text-gray-400">
                      Failed to load todos.
                    </p>
                  )}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
