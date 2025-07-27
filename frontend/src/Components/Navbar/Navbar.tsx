import { NavLink } from "react-router-dom";
import { Popover, PopoverButton, PopoverPanel } from '@headlessui/react';

const profile = [
  { name: 'Edytuj profil', link: "/edit-profile" },
  { name: 'Profil', link: "/profile" },
  { name: 'Wyloguj', link: "/logout" },
  { name: 'Pliki', link: "/files" },
  { name: 'Ustawienia', link: "/settings" },
];

const navLinks = [
  { name: "O nas", link: "/about-us" },
  { name: "Kontakt", link: "/contact" },
];

const baseButton =
  "font-semibold px-4 py-2 rounded-lg shadow transition flex items-center focus:outline-none focus:ring-2 focus:ring-blue-400";
const activeButton = "bg-blue-600 text-white hover:bg-blue-700 shadow-md";
const inactiveButton = "bg-white text-gray-900 hover:bg-gray-100";

const Header = () => (
   <header className="bg-gray-800 text-white px-6 py-4 w-full shadow">
    <div className="flex flex-col sm:flex-row items-center justify-between w-full">
      <NavLink
        to="/"
        className="text-2xl font-bold tracking-tight mb-2 sm:mb-0 hover:text-blue-300 transition-colors"
        end
      >
        SonicBridge
      </NavLink>
      <nav>
        <ul className="flex flex-col sm:flex-row gap-2 sm:gap-4 text-base">
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
                {item.name}
              </NavLink>
            </li>
          ))}
          <li>
            <Popover className="relative">
              <PopoverButton className={baseButton + " " + inactiveButton}>
                Profil
                <span className="ml-1 text-gray-900">&#9660;</span>
              </PopoverButton>
              <PopoverPanel className="absolute right-0 z-10 mt-2 w-48 rounded-xl bg-white shadow-lg ring-1 ring-gray-900/5">
                <div className="p-2 flex flex-col gap-2">
                  {profile.map((item) => (
                    <NavLink
                      key={item.name}
                      to={item.link}
                      className={({ isActive }) =>
                        [
                          "block font-semibold rounded-lg px-4 py-2 transition focus:outline-none",
                          isActive
                            ? "bg-blue-600 text-white hover:bg-blue-700"
                            : "text-gray-900 hover:bg-blue-100 focus:bg-blue-100"
                        ].join(" ")
                      }
                    >
                      {item.name}
                    </NavLink>
                  ))}
                </div>
              </PopoverPanel>
            </Popover>
          </li>
        </ul>
      </nav>
    </div>
  </header>
);

export default Header;
