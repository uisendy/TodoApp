import { RootState, AppDispatch } from '../../app/store';
import { refreshAccessToken } from './authAPI';
import { loginSuccess, logout } from './authSlice';
import { baseURI } from '../../config';

interface RequestConfig {
  url: string;
  method?: string;
  headers?: Record<string, string>;
  body?: any;
}

export const authorizedRequest = async (
  config: RequestConfig,
  getState: () => RootState,
  dispatch: AppDispatch
): Promise<any> => {
  const state = getState();
  let { accessToken, refreshToken } = state.auth;

  const makeRequest = async (token: string | null): Promise<Response> => {
    const headers = {
      'Content-Type': 'application/json',
      ...config.headers,
      Authorization: `Bearer ${token}`,
    };

    const response = await fetch(`${baseURI}${config.url}`, {
      method: config.method || 'GET',
      headers,
      body: config.body ? JSON.stringify(config.body) : undefined,
    });

    return response;
  };

  try {
    let response = await makeRequest(accessToken);

    if (response.status === 401 && refreshToken) {
      try {
        const tokens = await refreshAccessToken(refreshToken);
        console.log('Token refresh successful:', tokens);

        dispatch(
          loginSuccess({
            token: tokens.accessToken,
            refreshToken: tokens.refreshToken,
          })
        );

        response = await makeRequest(tokens.accessToken);
      } catch (refreshError) {
        dispatch(logout());
        throw new Error('Token refresh failed');
      }
    }

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Request failed: ${response.status} ${errorText}`);
    }

    const contentType = response.headers.get('Content-Type');
    if (contentType?.includes('application/json')) {
      return await response.json();
    } else {
      return await response.text();
    }
  } catch (err: any) {
    console.error('Request failed:', err.message || err);
    throw err;
  }
};
