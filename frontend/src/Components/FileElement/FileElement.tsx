import type FileElementProps from "./FileElementProps";
import axiosInstance from "../../Utils/axiosConfig";
import { useCallback, useState } from "react";

const FileElement: React.FC<FileElementProps> = ({ fileName }) => {
  const [showPlayer, setShowPlayer] = useState(false);
  const [audioUrl, setAudioUrl] = useState<string | null>(null);

  const [transcription, setTranscription] = useState<string>("");
  const [language, setLanguage] = useState<string>("pl");
  const [loadingTranscription, setLoadingTranscription] = useState(false);

  const displayName = fileName.includes("_")
    ? fileName.split("_").slice(1).join("_")
    : fileName;

  const togglePlayer = useCallback(async () => {
    if (showPlayer) {
      setShowPlayer(false);
      return;
    }

    try {
      const response = await axiosInstance.get(`/api/file/download/${encodeURIComponent(fileName)}`, {
        responseType: "blob",
      });
      const blobUrl = URL.createObjectURL(response.data);
      setAudioUrl(blobUrl);
      setShowPlayer(true);
    } catch (err) {
      console.error("Błąd przy pobieraniu pliku:", err);
    }
  }, [fileName, showPlayer]);

  const generateTranscription = useCallback(async () => {
    setLoadingTranscription(true);
    try {
      const response = await axiosInstance.post(`/api/transcription/get`, {
        fileName,
        language,
      });
      setTranscription(response.data.text);
    } catch (err) {
      console.error("Błąd przy generowaniu transkrypcji:", err);
    } finally {
      setLoadingTranscription(false);
    }
  }, [fileName, language]);

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
          <p className="text-gray-800 font-medium break-words">{displayName}</p>
        </div>
      </div>

      {showPlayer && audioUrl && (
        <div className="mt-3">
          <audio controls className="w-full" src={audioUrl}>
            Twoja przeglądarka nie obsługuje odtwarzacza audio.
          </audio>

          {/* Wybór języka */}
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
            <button
              onClick={generateTranscription}
              disabled={loadingTranscription}
              className="ml-auto bg-indigo-600 text-white text-sm px-3 py-1 rounded hover:bg-indigo-700 disabled:opacity-50"
            >
              {loadingTranscription ? "Generuję..." : "Transkrybuj"}
            </button>
          </div>

          {/* Pole z transkrypcją */}
          {transcription && (
            <div className="mt-3 bg-gray-100 rounded p-2 text-sm text-gray-800 whitespace-pre-wrap">
              {transcription}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default FileElement;
