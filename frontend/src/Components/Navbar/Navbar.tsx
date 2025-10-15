import { NavLink, useNavigate } from "react-router-dom";
import { Popover, PopoverButton, PopoverPanel } from '@headlessui/react';
import { useSelector, useDispatch } from "react-redux";
import type { RootState } from "../../store";
import { logout as logoutAction } from "../../Auth/Auth";
import axiosInstance from "../../Utils/axiosConfig";
import { User, LogIn, LogOut, FileText, Info, Phone, UserPlus } from "lucide-react";

const profile = [
  { name: 'Edytuj profil', link: "/edit-profile", icon: <User className="w-4 h-4 mr-2"/> },
  { name: 'Profil', link: "/profile", icon: <User className="w-4 h-4 mr-2"/> },
  { name: 'Pliki', link: "/files", icon: <FileText className="w-4 h-4 mr-2"/> },
  { name: 'Wyloguj', icon: <LogOut className="w-4 h-4 mr-2"/> },
];

const navLinks = [
  { name: "O nas", link: "/about-us", icon: <Info className="w-5 h-5" /> },
  { name: "Kontakt", link: "/contact", icon: <Phone className="w-5 h-5" /> },
];

const baseButton = "font-semibold px-3 py-2 rounded-lg shadow transition flex items-center gap-2 focus:outline-none focus:ring-2 focus:ring-blue-400";
const activeButton = "bg-blue-600 text-white hover:bg-blue-700 shadow-md";
const inactiveButton = "bg-white text-gray-900 hover:bg-gray-100";

const Navbar = () => {
  const profileName = useSelector((state: RootState) => state.auth.user) || "Gość";
  const isAuthenticated = useSelector((state: RootState) => state.auth.isAuthenticated);
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const handleLogout = async () => {
    try {
      await axiosInstance.post("/auth/logout");
    } catch (err) {}
    dispatch(logoutAction());
    navigate("/login");
  };

  return (
    <header className="bg-gray-800 text-white px-6 py-4 w-full shadow">
      <div className="flex flex-col sm:flex-row items-center justify-between w-full">
        <NavLink
          to="/"
          className="text-2xl font-bold tracking-tight mb-2 sm:mb-0 hover:text-blue-300 transition-colors flex items-center gap-2"
          end
        >
          <FileText className="w-6 h-6" />
          SonicBridge
        </NavLink>
        <nav>
          <ul className="flex flex-col sm:flex-row gap-2 sm:gap-4 text-base items-center">
            {navLinks.map((item) => (
              <li key={item.name}>
                <NavLink
                  to={item.link}
                  end={item.link === "/"}
                  className={({ isActive }) =>
                    [
                      baseButton,
                      isActive ? activeButton : inactiveButton
                    ].join(" ")
                  }
                >
                  {item.icon}
                  <span className="hidden sm:inline">{item.name}</span>
                </NavLink>
              </li>
            ))}

            {isAuthenticated ? (
              <li>
                <Popover className="relative">
                  <PopoverButton className={baseButton + " " + inactiveButton}>
                    <User className="w-5 h-5" />
                    <span className="hidden sm:inline">{profileName}</span>
                  </PopoverButton>
                  <PopoverPanel className="absolute right-0 z-10 mt-2 w-56 rounded-xl bg-white shadow-lg ring-1 ring-gray-900/5">
                    <div className="p-2 flex flex-col gap-1">
                      {profile.map((item) =>
                        item.name === "Wyloguj" ? (
                          <button
                            key={item.name}
                            onClick={handleLogout}
                            className="flex items-center font-semibold rounded-lg px-3 py-2 transition text-gray-900 hover:bg-blue-100 focus:outline-none"
                          >
                            {item.icon}
                            {item.name}
                          </button>
                        ) : (
                          <NavLink
                            key={item.name}
                            to={item.link!}
                            className={({ isActive }) =>
                              [
                                "flex items-center font-semibold rounded-lg px-3 py-2 transition focus:outline-none",
                                isActive
                                  ? "bg-blue-600 text-white hover:bg-blue-700"
                                  : "text-gray-900 hover:bg-blue-100 focus:bg-blue-100"
                              ].join(" ")
                            }
                          >
                            {item.icon}
                            {item.name}
                          </NavLink>
                        )
                      )}
                    </div>
                  </PopoverPanel>
                </Popover>
              </li>
            ) : (
              <li className="flex gap-2">
                <NavLink
                  to="/login"
                  className={({ isActive }) =>
                    [
                      baseButton,
                      isActive ? activeButton : inactiveButton
                    ].join(" ")
                  }
                >
                  <LogIn className="w-5 h-5" />
                  <span className="hidden sm:inline">Zaloguj</span>
                </NavLink>
                <NavLink
                  to="/registration"
                  className={({ isActive }) =>
                    [
                      baseButton,
                      isActive ? activeButton : inactiveButton
                    ].join(" ")
                  }
                >
                  <UserPlus className="w-5 h-5" />
                  <span className="hidden sm:inline">Zarejestruj się</span>
                </NavLink>
              </li>
            )}
          </ul>
        </nav>
      </div>
    </header>
  );
};

export default Navbar;
