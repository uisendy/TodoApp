import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import {
  fetchCurrentUser,
  signinUser,
  signupUser,
  updateUserProfile,
  verifyOtp,
} from './authAPI';
import { User, SignUpRequest } from '../../types';
import { AppDispatch, RootState } from '../../app/store';
import { authorizedRequest } from './authorizedRequest';

interface verifyOtpThunkProps {
  userId: string;
  otp: string;
}

interface AuthState {
  user: User | null;
  signUpRequest: SignUpRequest | null;
  accessToken: string | null;
  refreshToken: string | null;

  signinStatus: 'idle' | 'loading' | 'succeeded' | 'failed';
  signupStatus: 'idle' | 'loading' | 'succeeded' | 'failed';
  updateStatus: 'idle' | 'loading' | 'succeeded' | 'failed';
  loadUserStatus: 'idle' | 'loading' | 'succeeded' | 'failed';

  error: string | null;
  verifyStatus: 'idle' | 'loading' | 'succeeded' | 'failed';
  verifyError: object | string | null;
  verificationMessage: string | null;
}

const initialState: AuthState = {
  user: null,
  signUpRequest: null,
  accessToken: localStorage.getItem('accessToken') || null,
  refreshToken: localStorage.getItem('refreshToken') || null,

  signinStatus: 'idle',
  signupStatus: 'idle',
  updateStatus: 'idle',
  loadUserStatus: 'idle',

  error: null,
  verifyStatus: 'idle',
  verifyError: null,
  verificationMessage: null,
};

export const loadUser = createAsyncThunk<
  User,
  void,
  { state: RootState; dispatch: AppDispatch; rejectValue: string }
>('auth/loadUser', async (_, { getState, dispatch, rejectWithValue }) => {
  try {
    const response = await authorizedRequest(
      {
        url: '/api/v1/users/current-user',
        method: 'GET',
      },
      getState,
      dispatch
    );

    return response;
  } catch (err: any) {
    return rejectWithValue(err.message || 'Failed to load user');
  }
});

export const signin = createAsyncThunk<
  User,
  { email: string; password: string },
  { rejectValue: string; dispatch: AppDispatch; state: RootState }
>('auth/signin', async (payload, { rejectWithValue, dispatch }) => {
  try {
    const { accessToken, refreshToken } = await signinUser(payload);

    dispatch(loginSuccess({ token: accessToken, refreshToken }));

    const user = await fetchCurrentUser({ accessToken });
    return user;
  } catch (error: any) {
    return rejectWithValue(
      error?.response?.data?.message || error.message || 'Login failed'
    );
  }
});

export const signup = createAsyncThunk<
  User,
  { email: string; password: string; firstName: string; lastName: string },
  { rejectValue: string }
>('auth/signup', async (payload, { rejectWithValue }) => {
  try {
    const res = await signupUser(payload);

    if (!res?.data) {
      return rejectWithValue('No user data returned');
    }

    return res.data;
  } catch (error: any) {
    return rejectWithValue(error?.response?.data?.message);
  }
});

export const verifyOtpThunk = createAsyncThunk(
  'auth/verifyOtp',
  async ({ userId, otp }: verifyOtpThunkProps, { rejectWithValue }) => {
    try {
      const response = await verifyOtp({ userId, otp });
      return response;
    } catch (err: any) {
      return rejectWithValue(err.response?.data?.message);
    }
  }
);

export const updateUser = createAsyncThunk<
  User,
  Partial<User>,
  { state: RootState; rejectValue: string }
>('auth/updateUser', async (updatedData, { getState, rejectWithValue }) => {
  const token = getState().auth.accessToken;
  if (!token) return rejectWithValue('Unauthorized');

  try {
    const updatedUser = await updateUserProfile(updatedData, token);
    return updatedUser;
  } catch (err: any) {
    return rejectWithValue(
      err.response?.data?.message || 'Failed to update profile'
    );
  }
});

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    loginSuccess(
      state,
      action: PayloadAction<{ token: string; refreshToken?: string }>
    ) {
      state.accessToken = action.payload.token;
      state.refreshToken = action.payload.refreshToken || null;
      localStorage.setItem('accessToken', action.payload.token);
      localStorage.setItem('refreshToken', action.payload.refreshToken || '');
    },
    logout(state) {
      state.accessToken = null;
      state.refreshToken = null;
      state.user = null;
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
    },
  },
  extraReducers: builder => {
    builder
      .addCase(signup.pending, state => {
        state.signupStatus = 'loading';
        state.error = null;
      })
      .addCase(signup.fulfilled, (state, action) => {
        state.user = action.payload;
        state.signupStatus = 'succeeded';
      })
      .addCase(signup.rejected, (state, action) => {
        state.signupStatus = 'failed';
        state.error = action.payload || 'Error';
      })
      .addCase(signin.pending, state => {
        state.signinStatus = 'loading';
        state.error = null;
      })
      .addCase(signin.fulfilled, (state, action) => {
        state.user = action.payload;
        state.signinStatus = 'succeeded';
      })
      .addCase(signin.rejected, (state, action) => {
        state.signinStatus = 'failed';
        state.error = action.payload || 'Signin failed';
      })
      .addCase(verifyOtpThunk.pending, state => {
        state.verifyStatus = 'loading';
        state.verifyError = null;
      })
      .addCase(verifyOtpThunk.fulfilled, (state, action) => {
        state.verifyStatus = 'succeeded';
        state.verificationMessage = action.payload.message;
      })
      .addCase(verifyOtpThunk.rejected, (state, action) => {
        state.verifyStatus = 'failed';
        state.verifyError = action.payload || 'OTP verification failed';
      })
      .addCase(loadUser.pending, state => {
        state.loadUserStatus = 'loading';
        state.error = null;
      })
      .addCase(loadUser.fulfilled, (state, action: PayloadAction<User>) => {
        state.user = action.payload;
        state.loadUserStatus = 'succeeded';
      })
      .addCase(loadUser.rejected, (state, action) => {
        state.user = null;
        state.accessToken = null;
        state.refreshToken = null;
        state.loadUserStatus = 'failed';
        state.error = action.payload || 'Error';
        localStorage.removeItem('accessToken');
      })
      .addCase(updateUser.pending, state => {
        state.updateStatus = 'loading';
        state.error = null;
      })
      .addCase(updateUser.fulfilled, (state, action) => {
        state.updateStatus = 'succeeded';
        console.log('from Auth Slice', action.payload);
        state.user = action.payload;
      })
      .addCase(updateUser.rejected, (state, action) => {
        state.updateStatus = 'failed';
        state.error = action.payload || 'Update failed';
      });
  },
});

export const { loginSuccess, logout } = authSlice.actions;

export default authSlice.reducer;
