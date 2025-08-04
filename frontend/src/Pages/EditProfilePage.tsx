import React, { useState } from "react";

const EditProfilePage = () => {
  const [email, setEmail] = useState(""); // Nowy email
  const [password, setPassword] = useState(""); // Nowe hasło
  const [currentPassword, setCurrentPassword] = useState(""); // Aktualne hasło (do potwierdzenia)
  const [message, setMessage] = useState("");

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    // Prosta walidacja
    if (!email && !password) {
      setMessage("Wprowadź nowy e-mail lub hasło");
      return;
    }
    if (!currentPassword) {
      setMessage("Podaj swoje aktualne hasło");
      return;
    }

    // Przygotowanie payload do wysłania na backend
    const payload = { email, password, currentPassword };

    try {
      // Tu podmień URL na swój endpoint w backendzie!
      const response = await fetch("/api/user/update-profile", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      const data = await response.json();
      if (response.ok) {
        setMessage("Profil zaktualizowany!");
        setEmail("");
        setPassword("");
        setCurrentPassword("");
      } else {
        setMessage(data.error || "Wystąpił błąd.");
      }
    } catch (error) {
      setMessage("Błąd połączenia z serwerem.");
    }
  };

  return (
    <div className="max-w-md mx-auto mt-10 bg-white shadow rounded-xl p-6">
      <h1 className="text-2xl font-bold mb-6">Edytuj profil</h1>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block font-medium">Nowy e-mail</label>
          <input
            type="email"
            className="w-full border rounded px-3 py-2"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Nowy adres e-mail"
            autoComplete="email"
          />
        </div>
        <div>
          <label className="block font-medium">Nowe hasło</label>
          <input
            type="password"
            className="w-full border rounded px-3 py-2"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Nowe hasło"
            autoComplete="new-password"
          />
        </div>
        <div>
          <label className="block font-medium">Aktualne hasło <span className="text-red-500">*</span></label>
          <input
            type="password"
            className="w-full border rounded px-3 py-2"
            value={currentPassword}
            onChange={(e) => setCurrentPassword(e.target.value)}
            placeholder="Podaj obecne hasło"
            autoComplete="current-password"
            required
          />
        </div>
        <button
          type="submit"
          className="w-full bg-blue-600 text-white font-bold py-2 rounded hover:bg-blue-700"
        >
          Zapisz zmiany
        </button>
        {message && (
          <div className="mt-4 text-center text-red-500">{message}</div>
        )}
      </form>
    </div>
  );
};

export default EditProfilePage;
