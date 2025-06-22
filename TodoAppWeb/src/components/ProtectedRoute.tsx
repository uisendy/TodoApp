import { Navigate } from 'react-router';
import { useSelector } from 'react-redux';
import { RootState } from '../app/store';
import { JSX } from 'react';

export default function ProtectedRoute({
  children,
}: {
  children: JSX.Element;
}) {
  const { accessToken } = useSelector((state: RootState) => state.auth);
  return accessToken ? children : <Navigate to="/signin" />;
}
