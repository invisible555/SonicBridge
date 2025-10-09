import React, { useState } from "react";
import axiosInstance from "../Utils/axiosConfig";

const EditProfilePage = () => {
  const [showEmailModal, setShowEmailModal] = useState(false);
  const [showPasswordModal, setShowPasswordModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);

  const [email, setEmail] = useState("");
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [passwordForEmail, setPasswordForEmail] = useState(""); 
  const [passwordForDelete, setPasswordForDelete] = useState(""); 

  const [message, setMessage] = useState("");

  const handleEmailSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await axiosInstance.post("api/user/change-email", {
        NewEmail: email,
        Password: passwordForEmail,
      });
      setMessage("Email został zmieniony ✅");
    } catch (err: any) {
      if (err.response?.status === 401) {
        setMessage("Nieprawidłowe hasło ❌");
      } else {
        setMessage("Błąd przy zmianie emaila ❌");
      }
    }
    setShowEmailModal(false);
  };

  const handlePasswordSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await axiosInstance.post("api/user/change-password", {
        currentPassword,
        newPassword,
      });
      setMessage("Hasło zostało zmienione ✅");
    } catch (err: any) {
      if (err.response?.status === 401) {
        setMessage("Nieprawidłowe aktualne hasło ❌");
      } else {
        setMessage("Błąd przy zmianie hasła ❌");
      }
    }
    setShowPasswordModal(false);
  };

  const handleDeleteSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await axiosInstance.post("api/user/delete-account", {
        password: passwordForDelete,
      });
      setMessage("Konto zostało usunięte 🗑️");
      // Możesz np. przekierować na stronę logowania
      // window.location.href = "/login";
    } catch (err: any) {
      if (err.response?.status === 401) {
        setMessage("Nieprawidłowe hasło ❌");
      } else {
        setMessage("Błąd przy usuwaniu konta ❌");
      }
    }
    setShowDeleteModal(false);
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-[80vh]">
      <h1 className="text-2xl font-bold mb-6">Edycja profilu</h1>

      <div className="flex gap-4">
        <button
          onClick={() => setShowEmailModal(true)}
          className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
        >
          Edytuj email
        </button>

        <button
          onClick={() => setShowPasswordModal(true)}
          className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700"
        >
          Edytuj hasło
        </button>

        <button
          onClick={() => setShowDeleteModal(true)}
          className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700"
        >
          Usuń konto
        </button>
      </div>

      {message && <p className="mt-4 text-sm text-gray-700">{message}</p>}

      {/* Modal - Email */}
      {showEmailModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
          <div className="bg-white rounded-xl p-6 w-96 shadow-lg">
            <h2 className="text-lg font-bold mb-4">Zmień email</h2>
            <form onSubmit={handleEmailSubmit} className="flex flex-col gap-3">
              <input
                type="email"
                placeholder="Nowy email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="border p-2 rounded"
                required
              />
              <input
                type="password"
                placeholder="Aktualne hasło"
                value={passwordForEmail}
                onChange={(e) => setPasswordForEmail(e.target.value)}
                className="border p-2 rounded"
                required
              />
              <button
                type="submit"
                className="bg-blue-600 text-white py-2 rounded hover:bg-blue-700"
              >
                Zapisz
              </button>
              <button
                type="button"
                onClick={() => setShowEmailModal(false)}
                className="text-gray-500 mt-2"
              >
                Anuluj
              </button>
            </form>
          </div>
        </div>
      )}

      {/* Modal - Hasło */}
      {showPasswordModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
          <div className="bg-white rounded-xl p-6 w-96 shadow-lg">
            <h2 className="text-lg font-bold mb-4">Zmień hasło</h2>
            <form
              onSubmit={handlePasswordSubmit}
              className="flex flex-col gap-3"
            >
              <input
                type="password"
                placeholder="Aktualne hasło"
                value={currentPassword}
                onChange={(e) => setCurrentPassword(e.target.value)}
                className="border p-2 rounded"
                required
              />
              <input
                type="password"
                placeholder="Nowe hasło"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                className="border p-2 rounded"
                required
              />
              <button
                type="submit"
                className="bg-green-600 text-white py-2 rounded hover:bg-green-700"
              >
                Zapisz
              </button>
              <button
                type="button"
                onClick={() => setShowPasswordModal(false)}
                className="text-gray-500 mt-2"
              >
                Anuluj
              </button>
            </form>
          </div>
        </div>
      )}

      {/* Modal - Usuń konto */}
      {showDeleteModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
          <div className="bg-white rounded-xl p-6 w-96 shadow-lg">
            <h2 className="text-lg font-bold mb-4 text-red-600">Usuń konto</h2>
            <p className="text-sm text-gray-600 mb-3">
              Tej operacji nie można cofnąć! Podaj hasło, aby potwierdzić.
            </p>
            <form onSubmit={handleDeleteSubmit} className="flex flex-col gap-3">
              <input
                type="password"
                placeholder="Hasło"
                value={passwordForDelete}
                onChange={(e) => setPasswordForDelete(e.target.value)}
                className="border p-2 rounded"
                required
              />
              <button
                type="submit"
                className="bg-red-600 text-white py-2 rounded hover:bg-red-700"
              >
                Usuń
              </button>
              <button
                type="button"
                onClick={() => setShowDeleteModal(false)}
                className="text-gray-500 mt-2"
              >
                Anuluj
              </button>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default EditProfilePage;
