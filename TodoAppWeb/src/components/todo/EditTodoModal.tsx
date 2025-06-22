import { Modal } from '../ui/modal/index';
import { Formik, Form, Field } from 'formik';
import { useDispatch } from 'react-redux';
import { updateTodo, archiveTodo } from '../../features/todo/todoSlice';
import MultiSelect from '../form/MultiSelect';
import { FieldWrapper } from '../form/FieldWrapper';
import Label from '../form/Label';
import Button from '../ui/button/Button';
import { Todo, TodoRequest, TodoTag } from '../../types';
import { useAppDispatch } from '../../hooks/useAppDispatch';
import { useAppSelector } from '../../hooks/useAppSelector';

export default function EditTodoModal({
  isOpen,
  onClose,
  todo,
  todoTags,
}: {
  isOpen: boolean;
  onClose: () => void;
  todo: Todo;
  todoTags: string[];
}) {
  const dispatch = useAppDispatch();
  const { status, tags, priorities } = useAppSelector(state => state.todos);

  const initialValues = {
    name: todo.name,
    priority: todo.priority.toString(),
    tags: todo.tags.map(t => t.id),
  };

  const handleUpdate = async (values: typeof initialValues) => {
    console.log(values);
    await dispatch(
      updateTodo({
        id: todo.id,
        request: {
          ...values,
          priority: Number(values.priority),
        },
      })
    );
    onClose();
  };

  const handleArchive = async () => {
    await dispatch(archiveTodo(todo.id));
    onClose();
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} className="max-w-[700px] m-4">
      <div className="no-scrollbar relative w-full max-w-[700px] overflow-y-auto rounded-3xl bg-white p-4 dark:bg-gray-900 lg:p-11">
        <div className="px-2 pr-14">
          <h4 className="mb-2 text-2xl font-semibold text-gray-800 dark:text-white/90">
            Edit Todo
          </h4>
          <p className="mb-6 text-sm text-gray-500 dark:text-gray-400 lg:mb-7">
            Update todo details below.
          </p>
        </div>

        <Formik initialValues={initialValues} onSubmit={handleUpdate}>
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
                          <option key={p.value} value={p.value.toString()}>
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
                        onChange={selectedIds =>
                          setFieldValue('tags', selectedIds)
                        }
                        defaultSelected={initialValues.tags}
                      />
                    </div>
                  </div>
                </div>
              </div>

              <div className="flex items-center gap-3 px-2 mt-6 lg:justify-end">
                <Button size="sm" variant="outline" onClick={onClose}>
                  Close
                </Button>
                <Button
                  size="sm"
                  onClick={handleArchive}
                  className="bg-red-500"
                >
                  Archive
                </Button>
                <button
                  type="submit"
                  className="flex items-center justify-center w-full px-4 py-3 text-sm font-medium text-white transition rounded-lg bg-gray-500 shadow-theme-xs hover:bg-yellow-600"
                  disabled={isSubmitting}
                >
                  {isSubmitting ? 'Saving...' : 'Save Changes'}
                </button>
              </div>
            </Form>
          )}
        </Formik>
      </div>
    </Modal>
  );
}
