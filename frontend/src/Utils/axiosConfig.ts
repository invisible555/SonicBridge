import axios from 'axios';
const apiUrl = import.meta.env.VITE_API_URL || "http://localhost:7293";
const axiosInstance = axios.create({
  baseURL: apiUrl,
  withCredentials: true,
});

// Response interceptor (to catch 401)
axiosInstance.interceptors.response.use(
  response => response,
  async error => {
    const originalRequest = error.config;
    
    // Sprawdź czy błąd 401 i nie retryowaliśmy jeszcze tego requesta
    if (
      error.response?.status === 401 &&
      !originalRequest._retry &&
      !originalRequest.url.endsWith('/auth/refresh-access-token') // żeby nie pętlować na refresh!
    ) {
      originalRequest._retry = true;
      try {
        // POST na endpoint refreshujący token (dostosuj ścieżkę!)
        const resp = await axiosInstance.post('/auth/refresh-access-token');
        if (resp.status === 200) {
          // Retry oryginalnego requesta (cookies będą już odświeżone)
          return axiosInstance(originalRequest);
        }
      } catch (refreshError) {
        // Jeżeli refresh nie działa - np. refresh token nieaktualny, reject (wyloguj usera)
        return Promise.reject(refreshError);
      }
    }

    // Jeśli nie udało się odświeżyć tokena, odrzucamy oryginalny błąd
    return Promise.reject(error);
  }
);

export default axiosInstance;