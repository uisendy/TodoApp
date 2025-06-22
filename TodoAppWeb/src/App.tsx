import { BrowserRouter as Router, Routes, Route } from 'react-router';
import { useEffect } from 'react';

import SignIn from './pages/AuthPages/SignIn';
import SignUp from './pages/AuthPages/SignUp';
import NotFound from './pages/OtherPage/NotFound';
import UserProfiles from './pages/UserProfiles';
import Todo from './pages/Todo/Todo';
import Blank from './pages/Blank';
import AppLayout from './layout/AppLayout';
import { ScrollToTop } from './components/common/ScrollToTop';
import Home from './pages/Dashboard/Home';
import ProtectedRoute from './components/ProtectedRoute';
import { loadUser } from './features/auth/authSlice';
import { useAppDispatch } from './hooks/useAppDispatch';
import VerifyOtp from './pages/AuthPages/VerifyOtp';

export default function App() {
  const dispatch = useAppDispatch();

  useEffect(() => {
    dispatch(loadUser());
  }, [dispatch]);

  return (
    <Router>
      <ScrollToTop />
      <Routes>
        <Route path="/signin" element={<SignIn />} />
        <Route path="/signup" element={<SignUp />} />
        <Route path="/verify-otp" element={<VerifyOtp />} />

        <Route element={<AppLayout />}>
          <Route
            index
            path="/"
            element={
              <ProtectedRoute>
                <Home />
              </ProtectedRoute>
            }
          />
          <Route
            path="/profile"
            element={
              <ProtectedRoute>
                <UserProfiles />
              </ProtectedRoute>
            }
          />
          <Route
            path="/blank"
            element={
              <ProtectedRoute>
                <Blank />
              </ProtectedRoute>
            }
          />
          <Route
            path="/todo"
            element={
              <ProtectedRoute>
                <Todo />
              </ProtectedRoute>
            }
          />
        </Route>

        <Route path="*" element={<NotFound />} />
      </Routes>
    </Router>
  );
}
