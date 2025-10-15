import { createSlice } from "@reduxjs/toolkit";

// 🔹 Wczytaj dane z localStorage przy starcie aplikacji
const savedUser = localStorage.getItem("user");
const savedRole = localStorage.getItem("role");

const initialState = {
  isAuthenticated: !!savedUser,
  user: savedUser ? JSON.parse(savedUser) : null,
  role: savedRole || null,
  isLoading: false,
};

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    login: (state, action) => {
      state.isAuthenticated = true;
      state.user = action.payload.user;
      state.role = action.payload.role;
      state.isLoading = false;

      // 💾 Zapisz dane użytkownika w localStorage
      localStorage.setItem("user", JSON.stringify(action.payload.user));
      localStorage.setItem("role", action.payload.role);
    },
    logout: (state) => {
      state.isAuthenticated = false;
      state.user = null;
      state.role = null;
      state.isLoading = false;

      // 🧹 Usuń dane z localStorage
      localStorage.removeItem("user");
      localStorage.removeItem("role");
    },
    setLoading: (state, action) => {
      state.isLoading = action.payload;
    },
  },
});

export const { login, logout, setLoading } = authSlice.actions;
export default authSlice.reducer;
