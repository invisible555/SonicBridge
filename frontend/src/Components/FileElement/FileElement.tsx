import type FileElementProps from "./FileElementProps";
import axiosInstance from "../../Utils/axiosConfig";
import { useCallback, useState } from "react";

const FileElement: React.FC<FileElementProps> = ({ fileName }) => {
  const [showPlayer, setShowPlayer] = useState(false);
  const [audioUrl, setAudioUrl] = useState<string | null>(null);

  const [transcription, setTranscription] = useState<string>("");
  const [language, setLanguage] = useState<string>("pl");
  const [loadingTranscription, setLoadingTranscription] = useState(false);

  const [translation, setTranslation] = useState<string>("");
  const [targetLang, setTargetLang] = useState<string>("en");
  const [loadingTranslation, setLoadingTranslation] = useState(false);

  const [error, setError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);

  const displayName = fileName.includes("_")
    ? fileName.split("_").slice(1).join("_")
    : fileName;

  const generateTranscription = useCallback(async () => {
    setLoadingTranscription(true);
    setError(null);
    setTranscription("");
    setTranslation("");

    try {
      const response = await axiosInstance.post(`/api/transcription/get`, {
        fileName,
        language,
      });

      console.log("Odpowiedź backendu (transcription):", response.data);

      // jeśli backend zwraca { text: "..."} → użyj text
      // jeśli backend zwraca { status, taskId } → wyświetl to w JSON.stringify
      if (typeof response.data.text === "string") {
        setTranscription(response.data.text);
      } else {
        setTranscription(JSON.stringify(response.data, null, 2));
      }
    } catch (err) {
      console.error("Błąd przy generowaniu transkrypcji:", err);
      setError("Wystąpił problem podczas generowania transkrypcji.");
    } finally {
      setLoadingTranscription(false);
    }
  }, [fileName, language]);

  const generateTranslation = useCallback(async () => {
    setLoadingTranslation(true);
    setError(null);
    setTranslation("");
    try {
      const form = new FormData();
      form.append("text", transcription);
      form.append("source_lang", language);
      form.append("target_lang", targetLang);

      const response = await axiosInstance.post(
        `api/translation/translation`,
        form
      );

      console.log("Odpowiedź backendu (translation):", response.data);

      if (typeof response.data.translated_text === "string") {
        setTranslation(response.data.translated_text);
      } else {
        setTranslation(JSON.stringify(response.data, null, 2));
      }
    } catch (err) {
      console.error("Błąd przy tłumaczeniu:", err);
      setError("Wystąpił problem podczas tłumaczenia.");
    } finally {
      setLoadingTranslation(false);
    }
  }, [transcription, language, targetLang]);

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
  }, [fileName, showPlayer]);

  const handleDelete = useCallback(async () => {
    if (!window.confirm(`Czy na pewno chcesz usunąć plik "${displayName}"?`)) return;

    setDeleting(true);
    try {
      await axiosInstance.delete(`/api/file/delete/${encodeURIComponent(fileName)}`);
      alert("Plik został usunięty.");
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
            <label className="text-sm text-gray-700">Język transkrypcji:</label>
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
            <button
              className="ml-3 px-3 py-1 bg-blue-600 text-white rounded text-sm hover:bg-blue-700"
              onClick={generateTranscription}
              disabled={loadingTranscription}
            >
              {loadingTranscription ? "Generuję..." : "Generuj transkrypcję"}
            </button>
          </div>

          {error && <p className="mt-2 text-sm text-red-600">{error}</p>}

          {transcription && !loadingTranscription && (
            <div className="mt-3 bg-gray-100 rounded p-3 text-sm text-gray-800 whitespace-pre-wrap">
              <div><b>Transkrypcja:</b></div>
              {transcription}
            </div>
          )}

          {transcription && !loadingTranscription && (
            <div className="mt-4">
              <div className="flex items-center gap-2">
                <label className="text-sm text-gray-700">Język docelowy:</label>
                <select
                  value={targetLang}
                  onChange={(e) => setTargetLang(e.target.value)}
                  className="border rounded px-2 py-1 text-sm"
                >
                  <option value="en">Angielski</option>
                  <option value="pl">Polski</option>
                  <option value="fr">Francuski</option>
                  <option value="de">Niemiecki</option>
                </select>
                <button
                  className="ml-3 px-3 py-1 bg-green-600 text-white rounded text-sm hover:bg-green-700"
                  onClick={generateTranslation}
                  disabled={loadingTranslation}
                >
                  {loadingTranslation ? "Tłumaczę..." : "Przetłumacz"}
                </button>
              </div>
              {loadingTranslation && (
                <p className="mt-2 text-sm text-gray-500">Tłumaczenie w toku...</p>
              )}
              {translation && !loadingTranslation && (
                <div className="mt-3 bg-green-50 rounded p-3 text-sm text-gray-900 whitespace-pre-wrap">
                  <div><b>Tłumaczenie:</b></div>
                  {translation}
                </div>
              )}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default FileElement;
