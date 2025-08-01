import React, { useState } from "react";
import axiosInstance from "../Utils/axiosConfig";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import {login as loginAction } from "../Auth/Auth";

const LoginPage: React.FC = () => {
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const dispatch = useDispatch();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      const res = await axiosInstance.post("/auth/login", { login, password });
      // Załóżmy, że backend zwraca { login, role } 
      console.log("API URL:", import.meta.env.VITE_API_URL);
      dispatch(
        loginAction({
          user: res.data.login,
          role: res.data.role,
        })
      );
      setLoading(false);
      navigate("/"); // Albo np. "/dashboard"
    } catch (err: any) {
      setLoading(false);
      if (err.response && err.response.status === 401) {
        setError("Nieprawidłowy login lub hasło.");
      } else {
        setError("Wystąpił błąd. Spróbuj ponownie później.");
      }
    }
  };

  return (
    <div className="flex items-center justify-center min-h-[80vh] bg-gray-50">
      <form
        onSubmit={handleSubmit}
        className="bg-white rounded-xl shadow-xl p-8 w-full max-w-sm flex flex-col gap-5"
      >
        <h1 className="text-2xl font-bold text-center text-indigo-700 mb-2">
          Logowanie
        </h1>
        <div>
          <label className="block text-sm font-semibold mb-1 text-gray-700">
            Login lub e-mail
          </label>
          <input
            type="text"
            value={login}
            onChange={e => setLogin(e.target.value)}
            className="w-full px-4 py-2 rounded-lg border border-gray-300 focus:border-indigo-500 focus:outline-none"
            required
            autoComplete="username"
          />
        </div>
        <div>
          <label className="block text-sm font-semibold mb-1 text-gray-700">
            Hasło
          </label>
          <input
            type="password"
            value={password}
            onChange={e => setPassword(e.target.value)}
            className="w-full px-4 py-2 rounded-lg border border-gray-300 focus:border-indigo-500 focus:outline-none"
            required
            autoComplete="current-password"
          />
        </div>
        {error && (
          <div className="text-red-500 text-center text-sm">{error}</div>
        )}
        <button
          type="submit"
          disabled={loading}
          className="bg-indigo-600 text-white font-semibold rounded-lg py-2 hover:bg-indigo-700 transition disabled:opacity-50"
        >
          {loading ? "Logowanie..." : "Zaloguj się"}
        </button>
      </form>
    </div>
  );
};

export default LoginPage;
