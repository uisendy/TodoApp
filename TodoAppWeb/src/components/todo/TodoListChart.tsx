import TodoListChartTab from '../common/TodoListChartTab';

export default function TodoListChart() {
  return (
    <div className="rounded-2xl border border-gray-200 bg-white px-5 pb-5 pt-5 dark:border-gray-800 dark:bg-white/[0.03] sm:px-6 sm:pt-6">
      <div className="flex flex-col gap-5 mb-6 sm:flex-row sm:justify-between ">
        <div className="w-full">
          <h3 className="text-lg font-semibold text-gray-800 dark:text-white/90">
            Todos
          </h3>
          <p className="mt-1 text-gray-500 text-theme-sm dark:text-gray-400">
            My TodoList
          </p>
        </div>
        <div className="flex items-start w-full gap-3 sm:justify-end">
          <TodoListChartTab />
        </div>
      </div>

      <div className="max-w-full overflow-x-auto custom-scrollbar">
        <div className="min-w-[1000px] xl:min-w-full">
          {/* <TodoItem
            title="My First Todo"
            completed={false}
            markAsComplete={() => {}}
            date="27/9/2025"
            priority="High"
            tags={['Engineering', 'Home', 'Work']}
          />
          <TodoItem
            title="My First Todo"
            completed={false}
            markAsComplete={() => {}}
            date="27/9/2025"
            priority="Medium"
            tags={['Engineering', 'Home', 'Work']}
          /> */}
        </div>
      </div>
    </div>
  );
}
