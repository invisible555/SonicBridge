import type FileElementProps from "./FileElementProps";
import axiosInstance from "../../Utils/axiosConfig";
import { useCallback, useState } from "react";

const FileElement: React.FC<FileElementProps> = ({ fileName }) => {
  const [showPlayer, setShowPlayer] = useState(false);
  const [audioUrl, setAudioUrl] = useState<string | null>(null);

  const [transcription, setTranscription] = useState<string>("");
  const [language, setLanguage] = useState<string>("pl");
  const [loadingTranscription, setLoadingTranscription] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);

  const displayName = fileName.includes("_")
    ? fileName.split("_").slice(1).join("_")
    : fileName;

  const generateTranscription = useCallback(async () => {
    setLoadingTranscription(true);
    setError(null);
    try {
      const response = await axiosInstance.post(`/api/transcription/get`, {
        fileName,
        language,
      });
      setTranscription(response.data.text);
    } catch (err) {
      console.error("Błąd przy generowaniu transkrypcji:", err);
      setError("Wystąpił problem podczas generowania transkrypcji.");
    } finally {
      setLoadingTranscription(false);
    }
  }, [fileName, language]);

  const togglePlayer = useCallback(async () => {
    if (showPlayer) {
      setShowPlayer(false);
      return;
    }

    setError(null);

    try {
      const response = await axiosInstance.get(
        `/api/file/download/${encodeURIComponent(fileName)}`,
        { responseType: "blob" }
      );
      const blobUrl = URL.createObjectURL(response.data);
      setAudioUrl(blobUrl);
    } catch (err) {
      console.warn("Plik audio nie został znaleziony, otwieram bez odtwarzacza.");
      setAudioUrl(null);
    }

    setShowPlayer(true);
    generateTranscription();
  }, [fileName, showPlayer, generateTranscription]);

  const handleDelete = useCallback(async () => {
    if (!window.confirm(`Czy na pewno chcesz usunąć plik "${displayName}"?`)) return;

    setDeleting(true);
    try {
      await axiosInstance.delete(`/api/file/delete/${encodeURIComponent(fileName)}`);
      alert("Plik został usunięty.");
      // tutaj możesz np. wywołać callback do odświeżenia listy
    } catch (err) {
      console.error("Błąd przy usuwaniu pliku:", err);
      alert("Nie udało się usunąć pliku.");
    } finally {
      setDeleting(false);
    }
  }, [fileName, displayName]);

  return (
    <div className="bg-white rounded-lg shadow-md p-4 flex flex-col hover:shadow-lg transition-shadow w-full">
      <div className="flex items-center justify-between">
        <div
          className="flex items-center space-x-4 min-w-0 cursor-pointer"
          onClick={togglePlayer}
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className="h-8 w-8 text-gray-500 flex-shrink-0"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M12 6v6l4 2m0 0l4-2m-4 2V6m0 6l-4-2m0 0V6m0 6l-4-2m8 10H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v12a2 2 0 01-2 2h-3z"
            />
          </svg>
          <p className="text-gray-800 font-medium truncate">{displayName}</p>
        </div>

        {/* 🔥 przycisk usuwania */}
        <button
          onClick={handleDelete}
          disabled={deleting}
          className="text-red-600 hover:text-red-800 text-sm ml-2"
        >
          {deleting ? "Usuwanie..." : "Usuń"}
        </button>
      </div>

      {showPlayer && (
        <div className="mt-3">
          {audioUrl ? (
            <audio controls className="w-full" src={audioUrl}>
              Twoja przeglądarka nie obsługuje odtwarzacza audio.
            </audio>
          ) : (
            <p className="text-sm text-gray-500">Brak pliku audio do odtworzenia.</p>
          )}

          <div className="mt-3 flex items-center gap-2">
            <label className="text-sm text-gray-700">Język:</label>
            <select
              value={language}
              onChange={(e) => setLanguage(e.target.value)}
              className="border rounded px-2 py-1 text-sm"
            >
              <option value="pl">Polski</option>
              <option value="en">Angielski</option>
              <option value="fr">Francuski</option>
              <option value="de">Niemiecki</option>
            </select>
          </div>

          {loadingTranscription && (
            <p className="mt-2 text-sm text-gray-500">Ładowanie transkrypcji...</p>
          )}

          {error && <p className="mt-2 text-sm text-red-600">{error}</p>}

          {transcription && !loadingTranscription && (
            <div className="mt-3 bg-gray-100 rounded p-3 text-sm text-gray-800 whitespace-pre-wrap">
              {transcription}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default FileElement;
