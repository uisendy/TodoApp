import { useEffect, useState } from 'react';
import PageBreadcrumb from '../../components/common/PageBreadCrumb';
import PageMeta from '../../components/common/PageMeta';
import TodoListChartTab from '../../components/common/TodoListChartTab';
import TodoItem from '../../components/todo/TodoItem';
import Button from '../../components/ui/button/Button';
import { useAppDispatch } from '../../hooks/useAppDispatch';
import { useAppSelector } from '../../hooks/useAppSelector';
import {
  createTodo,
  fetchPriorities,
  fetchTags,
  fetchTodos,
  selectFilteredTodos,
  toggleTodoCompletion,
} from '../../features/todo/todoSlice';
import { Modal } from '../../components/ui/modal';
import { Field, Form, Formik } from 'formik';
import MultiSelect from '../../components/form/MultiSelect';
import Label from '../../components/form/Label';
import Input from '../../components/form/input/InputField';
import type { Todo } from '../../types';
import EditTodoModal from '../../components/todo/EditTodoModal';

export default function Todo() {
  const dispatch = useAppDispatch();
  const { status, tags, priorities } = useAppSelector(state => state.todos);
  const todos = useAppSelector(selectFilteredTodos);

  const [isAddOpen, setAddOpen] = useState(false);
  const [openDropdownId, setOpenDropdownId] = useState<string | null>(null);
  const [editTodo, setEditTodo] = useState<Todo | null>(null);

  useEffect(() => {
    dispatch(fetchTodos());
    dispatch(fetchTags());
    dispatch(fetchPriorities());
  }, [dispatch]);

  const openAddModal = () => {
    dispatch(fetchTags());
    dispatch(fetchPriorities());
    setAddOpen(true);
  };
  const closeAddModal = () => setAddOpen(false);

  const initialValues = {
    name: '',
    priority: 1,
    tags: [] as string[],
  };

  const handleAdd = async (
    values: typeof initialValues,
    { setSubmitting }: any
  ) => {
    await dispatch(
      createTodo({ ...values, priority: Number(values.priority) })
    );
    setSubmitting(false);
    closeAddModal();
    dispatch(fetchTodos());
  };

  return (
    <>
      <div>
        <PageMeta title="TodoIO - Manage" description="You and Your Todo" />
        <PageBreadcrumb pageTitle="Todo" />
        <div className="rounded-2xl border border-gray-200 bg-white px-5 pb-5 pt-5 dark:border-gray-800 dark:bg-white/[0.03] sm:px-6 sm:pt-6">
          <div className="flex flex-col sm:flex-row justify-between sm:justify-between pb-6 mb-3 items-center gap-2">
            <TodoListChartTab />
            <Button
              variant="primary"
              size="sm"
              onClick={openAddModal}
              className="my-2"
            >
              +Add
            </Button>
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
                      setEditTodo(todo);
                      setOpenDropdownId(null);
                    }}
                    showMenuDropdown={true}
                    showCheckButton={true}
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
      <Modal
        isOpen={isAddOpen}
        onClose={closeAddModal}
        className="max-w-[700px] m-4"
      >
        <div className="no-scrollbar relative w-full max-w-[700px] overflow-y-auto rounded-3xl bg-white p-4 dark:bg-gray-900 lg:p-11">
          <div className="px-2 pr-14">
            <h4 className="mb-2 text-2xl font-semibold text-gray-800 dark:text-white/90">
              Add New Todo
            </h4>
            <p className="mb-6 text-sm text-gray-500 dark:text-gray-400 lg:mb-7">
              Enter todo details below.
            </p>
          </div>

          <Formik initialValues={initialValues} onSubmit={handleAdd}>
            {({ isSubmitting, setFieldValue, values }) => (
              <Form className="flex flex-col">
                <div className="custom-scrollbar h-[450px] overflow-y-auto px-2 pb-3">
                  <div className="mt-7">
                    <h5 className="mb-5 text-lg font-medium text-gray-800 dark:text-white/90 lg:mb-6">
                      Todo Information
                    </h5>

                    <div className="grid grid-cols-1 gap-x-6 gap-y-5 lg:grid-cols-2">
                      <FieldWrapper name="name" label="Title" />
                      <div className="col-span-2 lg:col-span-1">
                        <Label>Priority</Label>
                        <Field
                          as="select"
                          name="priority"
                          className="w-full border border-gray-300 rounded-lg p-2 dark:bg-gray-800"
                        >
                          <option value="">Select priority</option>
                          {priorities.map(p => (
                            <option
                              key={p.value}
                              value={p.value}
                              onChange={value =>
                                setFieldValue(
                                  'priority',
                                  parseInt(value.toString(), 10)
                                )
                              }
                            >
                              {p.name}
                            </option>
                          ))}
                        </Field>
                      </div>
                      <div className="col-span-2">
                        <MultiSelect
                          label="Tags"
                          options={tags.map(tag => ({
                            value: tag.id,
                            text: tag.name,
                          }))}
                          onChange={values => setFieldValue('tags', values)}
                          defaultSelected={values.tags}
                        />
                      </div>
                    </div>
                  </div>
                </div>

                <div className="flex items-center gap-3 px-2 mt-6 lg:justify-end">
                  <Button size="sm" variant="outline" onClick={closeAddModal}>
                    Close
                  </Button>
                  <button
                    type="submit"
                    className="flex items-center justify-center w-full px-4 py-3 text-sm font-medium text-white transition rounded-lg bg-gray-500 shadow-theme-xs hover:bg-yellow-600"
                    disabled={isSubmitting}
                  >
                    {isSubmitting ? 'Creating...' : 'Create Todo'}
                  </button>
                </div>
              </Form>
            )}
          </Formik>
        </div>
      </Modal>
      {editTodo && (
        <EditTodoModal
          isOpen={!!editTodo}
          onClose={() => setEditTodo(null)}
          todo={editTodo}
        />
      )}
    </>
  );
}

const FieldWrapper = ({
  name,
  label,
  fullWidth = false,
}: {
  name: string;
  label: string;
  fullWidth?: boolean;
}) => (
  <div className={fullWidth ? 'col-span-2' : 'col-span-2 lg:col-span-1'}>
    <Label>{label}</Label>
    <Field name={name} as={Input} type="text" />
  </div>
);
