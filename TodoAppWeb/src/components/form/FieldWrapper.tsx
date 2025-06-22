import { Field } from 'formik';
import Label from './Label';
import Input from './input/InputField';

export const FieldWrapper = ({
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
