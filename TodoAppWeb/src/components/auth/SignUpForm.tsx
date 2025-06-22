import { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate } from 'react-router';
import { AppDispatch, RootState } from '../../app/store';
import { signup } from '../../features/auth/authSlice';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';

import Label from '../form/Label';
import Input from '../form/input/InputField';
import Checkbox from '../form/input/Checkbox';
import { EyeIcon, EyeCloseIcon } from '../../icons';
import { Link } from 'react-router';
import { Modal } from '../ui/modal';

export default function SignUpForm() {
  const dispatch = useDispatch<AppDispatch>();
  const navigate = useNavigate();
  const auth = useSelector((state: RootState) => state.auth);

  const [showPassword, setShowPassword] = useState(false);
  const [isChecked, setIsChecked] = useState(false);
  const [modalOpen, setModalOpen] = useState(false);

  const validationSchema = Yup.object({
    firstName: Yup.string().required('First name is required'),
    lastName: Yup.string().required('Last name is required'),
    email: Yup.string().email('Invalid email').required('Email is required'),
    password: Yup.string()
      .min(6, 'Password must be at least 6 characters')
      .required('Password is required'),
  });

  return (
    <div className="flex flex-col flex-1 w-full overflow-y-auto lg:w-1/2 no-scrollbar">
      <div className="w-full max-w-md mx-auto mb-5 sm:pt-10"></div>
      <div className="flex flex-col justify-center flex-1 w-full max-w-md mx-auto">
        <div>
          <div className="mb-5 sm:mb-8">
            <h1 className="mb-2 font-semibold text-yellow-500 text-title-sm dark:text-stone-400 sm:text-title-md">
              Sign Up
            </h1>
            <p className="text-sm text-gray-500 dark:text-gray-400">
              Enter your email and password to sign up!
            </p>
          </div>

          <Formik
            initialValues={{
              firstName: '',
              lastName: '',
              email: '',
              password: '',
            }}
            validationSchema={validationSchema}
            onSubmit={async values => {
              const resultAction = await dispatch(signup(values));
              if (signup.fulfilled.match(resultAction)) {
                const { firstName } = resultAction.payload;
                navigate('/verify-otp', {
                  state: {
                    greeting: `👋 Hello ${firstName}`,
                    message: 'Verify OTP',
                  },
                });
              } else {
                setModalOpen(true);
              }
            }}
          >
            {({ errors, touched }) => (
              <Form>
                <div className="space-y-5">
                  <div className="grid grid-cols-1 gap-5 sm:grid-cols-2">
                    <div className="sm:col-span-1">
                      <Label>
                        First Name<span className="text-error-500">*</span>
                      </Label>
                      <Field
                        as={Input}
                        type="text"
                        id="fname"
                        name="firstName"
                        placeholder="Enter your first name"
                        error={touched.firstName && !!errors.firstName}
                        hint={
                          touched.firstName && errors.firstName
                            ? errors.firstName
                            : ''
                        }
                      />
                    </div>
                    <div className="sm:col-span-1">
                      <Label>
                        Last Name<span className="text-error-500">*</span>
                      </Label>
                      <Field
                        as={Input}
                        type="text"
                        id="lname"
                        name="lastName"
                        placeholder="Enter your last name"
                        error={touched.lastName && !!errors.lastName}
                        hint={
                          touched.lastName && errors.lastName
                            ? errors.lastName
                            : ''
                        }
                      />
                    </div>
                  </div>

                  <div>
                    <Label>
                      Email<span className="text-error-500">*</span>
                    </Label>
                    <Field
                      as={Input}
                      type="email"
                      id="email"
                      name="email"
                      placeholder="Enter your email"
                      error={touched.email && !!errors.email}
                      hint={touched.email && errors.email ? errors.email : ''}
                    />
                  </div>

                  <div>
                    <Label>
                      Password<span className="text-error-500">*</span>
                    </Label>
                    <div className="relative">
                      <Field
                        as={Input}
                        type={showPassword ? 'text' : 'password'}
                        name="password"
                        placeholder="Enter your password"
                        error={touched.password && !!errors.password}
                        hint={
                          touched.password && errors.password
                            ? errors.password
                            : ''
                        }
                      />
                      <span
                        onClick={() => setShowPassword(!showPassword)}
                        className="absolute z-30 -translate-y-1/2 cursor-pointer right-4 top-1/2"
                      >
                        {showPassword ? (
                          <EyeIcon className="fill-gray-500 dark:fill-gray-400 size-5" />
                        ) : (
                          <EyeCloseIcon className="fill-gray-500 dark:fill-gray-400 size-5" />
                        )}
                      </span>
                    </div>
                  </div>

                  <div className="flex items-center gap-3">
                    <Checkbox
                      className="w-5 h-5"
                      checked={isChecked}
                      onChange={setIsChecked}
                    />
                    <p className="inline-block font-normal text-gray-500 dark:text-gray-400">
                      By creating an account means you agree to the{' '}
                      <span className="text-gray-800 dark:text-white/90">
                        Terms and Conditions,
                      </span>{' '}
                      and our{' '}
                      <span className="text-gray-800 dark:text-white">
                        Privacy Policy
                      </span>
                    </p>
                  </div>

                  <div>
                    <button
                      type="submit"
                      className="flex items-center justify-center w-full px-4 py-3 text-sm font-medium text-white transition rounded-lg bg-gray-500 shadow-theme-xs hover:bg-yellow-600"
                      disabled={auth.signupStatus === 'loading'}
                    >
                      {auth.signupStatus === 'loading'
                        ? 'Processing...'
                        : 'Sign Up'}
                    </button>
                  </div>
                </div>
              </Form>
            )}
          </Formik>

          <div className="mt-5">
            <p className="text-sm font-normal text-center text-gray-700 dark:text-gray-400 sm:text-start">
              Already have an account?{' '}
              <Link
                to="/signin"
                className="text-yellow-500 hover:text-yellow-800 dark:text-yellow-400"
              >
                Sign In
              </Link>
            </p>
          </div>
        </div>
      </div>

      <Modal
        isOpen={modalOpen}
        onClose={() => setModalOpen(false)}
        className="max-w-md m-4"
      >
        <div className="rounded-2xl bg-white dark:bg-gray-900 p-6 text-center">
          <h2 className="mb-4 mt-10 text-xl font-semibold text-gray-800 dark:text-white">
            {auth.error}
          </h2>
          <button
            onClick={() => setModalOpen(false)}
            className="mt-4 px-4 py-2 bg-yellow-500 text-white rounded-md hover:bg-yellow-600"
          >
            Close | [Esc]
          </button>
        </div>
      </Modal>
    </div>
  );
}
