import axios from 'axios';

const axiosInstance = axios.create({
  baseURL: 'http://localhost:5500/api/v1',
});

axiosInstance.interceptors.request.use(config => {
  const token = localStorage.getItem('accessToken');
  config.headers['Authorization'] = `Bearer ${token}`;
  return config;
});

export default axiosInstance;
