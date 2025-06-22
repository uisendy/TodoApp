import PageMeta from '../../components/common/PageMeta';
import AuthLayout from './AuthPageLayout';
import VerifyOtpForm from '../../components/auth/VerifyOtpForm';

export default function VerifyOtp() {
  return (
    <>
      <PageMeta
        title="TodoIO - Verify OTP"
        description="Verify your account to continue"
      />
      <AuthLayout>
        <VerifyOtpForm />
      </AuthLayout>
    </>
  );
}
