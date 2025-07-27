
import './App.css'
import { Route, Routes,Navigate } from 'react-router-dom';
import HomePage from './Pages/HomePage';
import Navbar from './Components/Navbar/Navbar';
import ContactPage from './Pages/ContactPage';
import Footer from './Components/Footer/Footer';
import ProfilePage from './Pages/ProfilePage';
import EditProfilePage from './Pages/EditProfilePage';
import AboutUsPage from './Pages/AboutUsPage';
import LoginPage from './Pages/LoginPage';
function App() {

  return (
    <>
    <div className="flex flex-col min-h-screen">
        <Navbar/>
        <main className="flex-1">
        <Routes>
          <Route path="/home" element={<HomePage />} />
          <Route path="/contact" element={<ContactPage/>}/>
          <Route path="/profile" element={<ProfilePage/>}/>
          <Route path="/edit-profile" element={<EditProfilePage/>}/>
          <Route path="/about-us" element={<AboutUsPage/>}/>
          <Route path="/login" element={<LoginPage/>}/>
          <Route path="*" element={<Navigate to="/home" replace />} />
        </Routes>
        </main>
        <Footer/>
    </div>
    </>
  )
}

export default App
