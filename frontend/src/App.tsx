
import './App.css'
import { Route, Routes,Navigate } from 'react-router-dom';
import HomePage from './Pages/HomePage';
import Navbar from './components/Navbar/Navbar';
import ContactPage from './Pages/ContactPage';
import Footer from './components/Footer/Footer';
import ProfilePage from './Pages/ProfilePage';
import EditProfilePage from './Pages/EditProfilePage';
import AboutUsPage from './Pages/AboutUsPage';
import LoginPage from './Pages/LoginPage';
import PrivateRoute from './PrivateRoute/PrivateRoute';
import { useDispatch } from "react-redux";
import { useEffect } from "react";
import axiosInstance from "./Utils/axiosConfig";
import { login, logout  } from "./Auth/Auth";
import FilePage from './Pages/FilesPage';
import RegistrationPage from './Pages/RegistrationPage';
function App() {
const dispatch = useDispatch();

    useEffect(() => {
    console.log("useEffect działa!"); // Dodaj ten log
    axiosInstance.get("/auth/me")
      .then(res => {
        dispatch(login({ user: res.data.login, role: res.data.role }));
      })
      .catch(() => {
        dispatch(logout());
      });
  }, [dispatch]);

  return (
    <>
    <div className="flex flex-col min-h-screen">
        <Navbar/>
        <main className="flex-1">
        <Routes>
          <Route path="/home" element={<HomePage />} />
          <Route path="/contact" element={<ContactPage/>}/>
          <Route path="/about-us" element={<AboutUsPage/>}/>
          <Route path="/login" element={<LoginPage/>}/>
          <Route path="/registration" element={<RegistrationPage/>}/>
          <Route element={<PrivateRoute />}>
            <Route path="/profile" element={<ProfilePage/>}/>
            <Route path="/edit-profile" element={<EditProfilePage/>}/>
            <Route path="/files" element={<FilePage/>}/>
          </Route>
          <Route path="*" element={<Navigate to="/home" replace />} />
        </Routes>
        </main>
        <Footer/>
    </div>
    </>
  )
}

export default App
