import PageMeta from '../../components/common/PageMeta';
import AuthLayout from './AuthPageLayout';
import SignUpForm from '../../components/auth/SignUpForm';

export default function SignUp() {
  return (
    <>
      <PageMeta
        title="TodoIO - Register"
        description="Never miss a thought, Stay Creative"
      />
      <AuthLayout>
        <SignUpForm />
      </AuthLayout>
    </>
  );
}
