import axios from 'axios';
import { ApiResponse, UpdateUserPayload, User } from '../../types';
import { baseURI } from '../../config';

export async function fetchCurrentUser({
  accessToken,
}: {
  accessToken: string;
}): Promise<User> {
  const res = await axios.get<ApiResponse<User>>(
    `${baseURI}/api/v1/users/current-user`,
    {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    }
  );

  const user = res.data?.data;

  if (!user) {
    throw new Error('Invalid API response: missing user data');
  }

  return user;
}

export async function signupUser({
  email,
  password,
  firstName,
  lastName,
}: {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}): Promise<ApiResponse<User>> {
  const response = await axios.post(`${baseURI}/api/v1/auth/register`, {
    email,
    password,
    firstName,
    lastName,
  });
  return response.data;
}

export async function verifyOtp({
  userId,
  otp,
}: {
  userId: string;
  otp: string;
}): Promise<{ message: string }> {
  const response = await axios.post(`${baseURI}/api/v1/auth/verify-otp`, {
    userId,
    otp,
  });
  return response.data;
}

export async function signinUser({
  email,
  password,
}: {
  email: string;
  password: string;
}): Promise<{
  accessToken: string;
  refreshToken: string;
}> {
  const response = await axios.post(`${baseURI}/api/v1/auth/login`, {
    email,
    password,
  });

  return {
    accessToken: response.headers['x-access-token'],
    refreshToken: response.headers['x-refresh-token'],
  };
}

export async function updateUserProfile(
  data: UpdateUserPayload,
  accessToken: string
): Promise<User> {
  const response = await axios.put(
    `${baseURI}/api/v1/users/update-profile`,
    data,
    {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    }
  );

  return response.data;
}

export const refreshAccessToken = async (refreshToken: string) => {
  const response = await axios.post(
    `${baseURI}/api/v1/auth/refresh-token`,
    null,
    {
      headers: {
        'X-Refresh-Token': refreshToken,
      },
    }
  );

  const newAccessToken = response.headers['x-access-token'];
  const newRefreshToken = response.headers['x-refresh-token'];

  if (!newAccessToken) {
    throw new Error('No access token returned');
  }

  return {
    accessToken: newAccessToken,
    refreshToken: newRefreshToken,
  };
};
