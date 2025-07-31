// components/PrivateRoute.tsx
import { useSelector } from "react-redux";
import type { RootState } from "../store";
import { Navigate, Outlet } from "react-router-dom";

const PrivateRoute = () => {
  const isAuthenticated = useSelector((state: RootState) => state.auth.isAuthenticated);
  const isLoading = useSelector((state: RootState) => state.auth.isLoading);

  if (isLoading) {
    return <div>Ładowanie...</div>; // lub spinner!
  }

  return isAuthenticated ? <Outlet /> : <Navigate to="/login" replace />;
};

export default PrivateRoute;
