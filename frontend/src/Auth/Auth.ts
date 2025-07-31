// Auth/Auth.ts
import { createSlice } from '@reduxjs/toolkit';

const authSlice = createSlice({
    name: 'auth',
    initialState: {
        isAuthenticated: false,
        user: null,
        role: null,
        isLoading: true, // <--- DODAJ TO!
    },
    reducers: {
        login: (state, action) => {
            state.isAuthenticated = true;
            state.user = action.payload.user;
            state.role = action.payload.role;
            state.isLoading = false;
        },
        logout: (state) => {
            state.isAuthenticated = false;
            state.user = null;
            state.role = null;
            state.isLoading = false;
        },
        setLoading: (state, action) => {
            state.isLoading = action.payload;
        }
    }
});
export const { login, logout, setLoading } = authSlice.actions;
export default authSlice.reducer;
