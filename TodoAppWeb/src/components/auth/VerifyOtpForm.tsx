import { useRef, useState } from 'react';
import Label from '../form/Label';
import { Modal } from '../ui/modal';
import { useNavigate } from 'react-router';
import { useAppDispatch } from '../../hooks/useAppDispatch';
import { useAppSelector } from '../../hooks/useAppSelector';
import { verifyOtpThunk } from '../../features/auth/authSlice';

export default function VerifyOtpForm() {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [modalMessage, setModalMessage] = useState('');
  const [otp, setOtp] = useState<string[]>(['', '', '', '', '', '']);
  const inputsRef = useRef<(HTMLInputElement | null)[]>([]);

  const dispatch = useAppDispatch();
  const { user } = useAppSelector(state => state.auth);

  const handleVerifyOtp = async (code: string) => {
    if (!user?.id) {
      setModalMessage('Missing user ID');
      setIsModalOpen(true);
      return;
    }

    const resultAction = await dispatch(
      verifyOtpThunk({ userId: user?.id, otp: code })
    );

    if (verifyOtpThunk.fulfilled.match(resultAction)) {
      setModalMessage('Email Verified Successfully!');
    } else {
      setModalMessage('Email Verification failed');
    }

    setIsModalOpen(true);
  };

  const navigate = useNavigate();
  const handleChange = (index: number, value: string) => {
    if (!/^\d?$/.test(value)) return; // Only allow digits
    const newOtp = [...otp];
    newOtp[index] = value;
    setOtp(newOtp);

    if (value && index < 5) {
      inputsRef.current[index + 1]?.focus();
    }

    if (index === 5 && value) {
      handleVerifyOtp(newOtp.join(''));
    }
  };

  const handleResend = () => {
    console.log('Resend OTP');
  };

  const closeModal = () => {
    setIsModalOpen(false);
    if (modalMessage === 'Email Verified Successfully!') {
      navigate('/signin');
    }
  };

  return (
    <>
      <div className="flex flex-col flex-1 w-full overflow-y-auto lg:w-1/2 no-scrollbar">
        <div className="w-full max-w-md mx-auto mb-5 sm:pt-10" />

        <div className="flex flex-col justify-center flex-1 w-full max-w-md mx-auto">
          <div className="mb-5 sm:mb-8">
            <h1 className="mb-2 text-center font-semibold text-yellow-500 text-title-sm dark:text-stone-400 sm:text-title-md">
              Verify OTP
            </h1>
            <p className="text-sm text-center text-gray-500 dark:text-gray-400">
              Enter the 6-digit code sent to your email or phone
            </p>
          </div>

          <form>
            <div className="space-y-6">
              <div>
                <Label className="text-center">Enter OTP</Label>
                <div className="flex gap-2 mt-2 w-full justify-center items-center">
                  {otp.map((digit, i) => (
                    <input
                      key={i}
                      ref={(el: HTMLInputElement | null) => {
                        inputsRef.current[i] = el;
                      }}
                      type="text"
                      inputMode="numeric"
                      maxLength={1}
                      value={digit}
                      onChange={e => handleChange(i, e.target.value)}
                      className="w-9 h-9 md:w-18 md:h-18 text-center border rounded-lg dark:bg-stone-800 border-gray-300 dark:border-gray-600 text-lg"
                    />
                  ))}
                </div>
              </div>

              <div>
                <button
                  type="submit"
                  className="flex items-center justify-center w-full px-4 py-3 text-sm font-medium text-white transition rounded-lg bg-gray-500 shadow-theme-xs hover:bg-yellow-600"
                  onClick={e => {
                    e.preventDefault();
                    handleVerifyOtp(otp.join(''));
                  }}
                >
                  Verify OTP
                </button>
              </div>

              <div className="text-sm text-center text-gray-600 dark:text-gray-400">
                Didn’t receive the code?{' '}
                <button
                  type="button"
                  onClick={handleResend}
                  className="text-yellow-500 hover:text-yellow-700"
                >
                  Resend OTP
                </button>
              </div>
            </div>
          </form>
        </div>
      </div>
      <Modal isOpen={isModalOpen} onClose={closeModal} className="max-w-md m-4">
        <div className="rounded-2xl m-10 bg-white dark:bg-gray-900 p-6 text-center">
          <h2 className="mb-4 text-xl font-semibold text-gray-800 dark:text-white">
            {modalMessage}
          </h2>
          <button
            onClick={closeModal}
            className="mt-4 px-4 py-2 bg-yellow-500 text-white rounded-md hover:bg-yellow-600"
          >
            {modalMessage === 'OTP Verified Successfully!'
              ? 'Proceed to Sign In'
              : 'Try Again'}
          </button>
        </div>
      </Modal>
    </>
  );
}
