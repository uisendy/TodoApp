import PageMeta from '../../components/common/PageMeta';
import AuthLayout from './AuthPageLayout';
import SignInForm from '../../components/auth/SignInForm';

export default function SignIn() {
  return (
    <>
      <PageMeta
        title="TodoIO - SignIn"
        description="Never miss a thought, Stay Creative"
      />
      <AuthLayout>
        <SignInForm />
      </AuthLayout>
    </>
  );
}
