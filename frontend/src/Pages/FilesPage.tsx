import React, { useState } from "react";
import axiosInstance from "../Utils/axiosConfig";
import FileList from "../components/FileList/FileList";
//TODO ma się wyświetlać plik audio a jeżeli nie znajdzie go to poprostu nie można odtwarzać
const FilesPage = () => {
  const [file, setFile] = useState<File | null>(null);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFile(e.target.files ? e.target.files[0] : null);
    setSuccess(null);
    setError(null);
  };

  const handleUpload = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    if (!file) {
      setError("Wybierz plik audio!");
      return;
    }

    const formData = new FormData();
    formData.append("file", file);

    setUploading(true);
    try {
      await axiosInstance.post("/api/file/upload", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });
      setSuccess("Plik został wysłany!");
      setFile(null);
    } catch (err: any) {
      setError("Błąd przy wysyłaniu pliku.");
    } finally {
      setUploading(false);
    }
  };

  return (
    <div className="flex flex-col md:flex-row gap-8 justify-center items-start min-h-[60vh] p-6">
      {/* Form Upload */}
      <form
        onSubmit={handleUpload}
        className="flex flex-col gap-4 p-6 rounded-xl shadow bg-white w-full max-w-sm"
      >
        <h2 className="font-bold text-lg text-indigo-700">Wyślij plik audio</h2>
        <input
          type="file"
          accept="audio/*"
          onChange={handleFileChange}
          className="border rounded px-2 py-1"
        />
        {error && <div className="text-red-600">{error}</div>}
        {success && <div className="text-green-600">{success}</div>}
        <button
          type="submit"
          className="bg-indigo-600 text-white py-2 rounded-lg font-semibold hover:bg-indigo-700 disabled:opacity-50"
          disabled={uploading}
        >
          {uploading ? "Wysyłanie..." : "Wyślij"}
        </button>
      </form>

      {/* File List */}
      <div className="bg-white rounded-xl shadow p-6 w-full max-w-2xl">
        <h2 className="text-xl font-semibold text-gray-800 mb-4">Lista plików</h2>
        <FileList />
      </div>
    </div>
  );
};

export default FilesPage;
