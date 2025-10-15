import type FileElementProps from "./FileElementProps";
import axiosInstance from "../../Utils/axiosConfig";
import { useCallback, useState } from "react";
import { useSelector } from "react-redux";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Loader2, Mic, Globe, Volume2, Trash2 } from "lucide-react";

const FileElement: React.FC<FileElementProps> = ({ fileName }) => {
  const [showPlayer, setShowPlayer] = useState(false);
  const [audioUrl, setAudioUrl] = useState<string | null>(null);

  const [transcription, setTranscription] = useState<string>("");
  const [language, setLanguage] = useState<string>("pl");
  const [loadingTranscription, setLoadingTranscription] = useState(false);

  const [translation, setTranslation] = useState<string>("");
  const [targetLang, setTargetLang] = useState<string>("en");
  const [loadingTranslation, setLoadingTranslation] = useState(false);

  const [ttsLang, setTtsLang] = useState<string>("en");
  const [ttsStatus, setTtsStatus] = useState<"idle" | "queued" | "generating" | "done" | "error">("idle");
  const [ttsAudioUrl, setTtsAudioUrl] = useState<string | null>(null);
  const [ttsError, setTtsError] = useState<string | null>(null);

  const [, setError] = useState<string | null>(null); // nieużywane – ignorujemy
  const [deleting, setDeleting] = useState(false);

  const user = useSelector((state: any) => state.auth.user);
  const userId = user?.id?.toString() || "default";

  const displayName = fileName.includes("_")
    ? fileName.split("_").slice(1).join("_")
    : fileName;

  // === TRANSKRYPCJA ===
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
      const data = response.data;
      setTranscription(data.text || "⏳ Transkrypcja w toku...");
    } catch {
      setError("Wystąpił problem podczas generowania transkrypcji.");
    } finally {
      setLoadingTranscription(false);
    }
  }, [fileName, language]);

  // === TŁUMACZENIE ===
  const generateTranslation = useCallback(async () => {
    setLoadingTranslation(true);
    setError(null);
    setTranslation("");

    try {
      const form = new FormData();
      form.append("text", transcription);
      form.append("source_lang", language);
      form.append("target_lang", targetLang);

      const response = await axiosInstance.post(`/api/translation/translation`, form);
      const data = response.data;
      setTranslation(data.translated_text || "⚠️ Nie udało się pobrać tłumaczenia.");
    } catch {
      setError("Wystąpił problem podczas tłumaczenia.");
    } finally {
      setLoadingTranslation(false);
    }
  }, [transcription, language, targetLang]);

  // === TTS ===
  const generateTTS = useCallback(async () => {
    if (!translation) {
      setTtsError("Brak tekstu do wygenerowania głosu.");
      return;
    }

    setTtsStatus("queued");
    setTtsError(null);
    setTtsAudioUrl(null);

    try {
      const form = new FormData();
      form.append("text", translation);
      form.append("voice", `tts_models/${ttsLang}/ljspeech/tacotron2-DDC`);
      form.append("output_name", `${fileName}_${ttsLang}.wav`);
      form.append("user_id", userId);

      const res = await axiosInstance.post(`/api/tts/generate`, form);
      console.log("TTS generate:", res.data);

      if (res.data.status === "done" && res.data.file_path) {
        const filePath = res.data.file_path;
        const [userIdFromPath, , filename] = filePath.split("/");
        const audioRes = await axiosInstance.get(`/api/tts/download/${userIdFromPath}/${filename}`, {
          responseType: "blob",
        });
        const blobUrl = URL.createObjectURL(audioRes.data);
        setTtsAudioUrl(blobUrl);
        setTtsStatus("done");
        return;
      }

      const { task_id } = res.data;
      if (!task_id) throw new Error("Brak task_id w odpowiedzi TTS.");

      setTtsStatus("generating");
      const interval = setInterval(async () => {
        try {
          const statusRes = await axiosInstance.get(`/api/tts/status/${task_id}`);
          if (statusRes.data.status === "done") {
            clearInterval(interval);
            const filePath = statusRes.data.file_path;
            const [userIdFromPath, , filename] = filePath.split("/");
            const audioRes = await axiosInstance.get(`/api/tts/download/${userIdFromPath}/${filename}`, {
              responseType: "blob",
            });
            const blobUrl = URL.createObjectURL(audioRes.data);
            setTtsAudioUrl(blobUrl);
            setTtsStatus("done");
          }
        } catch {
          clearInterval(interval);
          setTtsStatus("error");
          setTtsError("Błąd sprawdzania statusu TTS.");
        }
      }, 2000);
    } catch {
      setTtsError("Błąd przy wysyłaniu żądania TTS.");
      setTtsStatus("error");
    }
  }, [translation, ttsLang, fileName, userId]);

  // === PLAYER ===
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
    } catch {
      setAudioUrl(null);
    }
    setShowPlayer(true);
  }, [fileName, showPlayer]);

  // === DELETE FILE ===
  const handleDelete = useCallback(async () => {
    if (!window.confirm(`Czy na pewno chcesz usunąć plik "${displayName}"?`)) return;
    setDeleting(true);
    try {
      await axiosInstance.delete(`/api/file/delete/${encodeURIComponent(fileName)}`);
      alert("Plik został usunięty.");
    } catch {
      alert("Nie udało się usunąć pliku.");
    } finally {
      setDeleting(false);
    }
  }, [fileName, displayName]);

  // === UI ===
  return (
    <Card className="shadow-lg hover:shadow-xl transition-all border-gray-200 dark:border-gray-700 bg-white dark:bg-neutral-900">
      <CardHeader className="flex justify-between items-center">
        <CardTitle className="text-lg font-semibold flex items-center gap-2 text-gray-800 dark:text-gray-100">
          🎧 {displayName}
        </CardTitle>
        <Button
          variant="destructive"
          size="sm"
          onClick={handleDelete}
          disabled={deleting}
          className="flex items-center gap-2"
        >
          <Trash2 className="h-4 w-4" />
          {deleting ? "Usuwanie..." : "Usuń"}
        </Button>
      </CardHeader>

      <CardContent className="space-y-6">
        {/* PLAYER */}
        <div className="w-full flex justify-center">
          <Button onClick={togglePlayer} variant="secondary" className="flex items-center gap-2">
            <Volume2 className="h-4 w-4" />
            {showPlayer ? "Ukryj odtwarzacz" : "Odtwórz"}
          </Button>
        </div>
        {showPlayer && (
          <div className="mt-3">
            {audioUrl ? (
              <audio controls className="w-full rounded-md border dark:border-gray-700">
                <source src={audioUrl} type="audio/wav" />
              </audio>
            ) : (
              <p className="text-sm text-gray-500 dark:text-gray-400">Brak pliku audio.</p>
            )}
          </div>
        )}

        {/* === TRANSKRYPCJA === */}
        <Card className="p-4 border border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-neutral-800">
          <CardTitle className="flex items-center gap-2 text-base font-medium text-gray-700 dark:text-gray-200">
            <Mic className="h-4 w-4 text-blue-600" /> Transkrypcja
          </CardTitle>
          <div className="flex items-center gap-2 mt-3">
            <select
              value={language}
              onChange={(e) => setLanguage(e.target.value)}
              className="border rounded px-2 py-1 text-sm bg-white dark:bg-neutral-900 text-gray-800 dark:text-gray-100"
            >
              <option value="pl">Polski</option>
              <option value="en">Angielski</option>
              <option value="fr">Francuski</option>
              <option value="de">Niemiecki</option>
            </select>
            <Button onClick={generateTranscription} disabled={loadingTranscription}>
              {loadingTranscription ? <Loader2 className="animate-spin h-4 w-4 mr-2" /> : "Generuj"}
            </Button>
          </div>
          {transcription && (
            <div className="mt-3 bg-gray-100 dark:bg-neutral-700 rounded p-2 text-sm whitespace-pre-wrap">
              {transcription}
            </div>
          )}
        </Card>

        {/* === TŁUMACZENIE === */}
        {transcription && (
          <Card className="p-4 border border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-neutral-800">
            <CardTitle className="flex items-center gap-2 text-base font-medium text-gray-700 dark:text-gray-200">
              <Globe className="h-4 w-4 text-green-600" /> Tłumaczenie
            </CardTitle>
            <div className="flex items-center gap-2 mt-3">
              <select
                value={targetLang}
                onChange={(e) => setTargetLang(e.target.value)}
                className="border rounded px-2 py-1 text-sm bg-white dark:bg-neutral-900 text-gray-800 dark:text-gray-100"
              >
                <option value="en">Angielski</option>
                <option value="pl">Polski</option>
                <option value="fr">Francuski</option>
                <option value="de">Niemiecki</option>
              </select>
              <Button onClick={generateTranslation} disabled={loadingTranslation}>
                {loadingTranslation ? <Loader2 className="animate-spin h-4 w-4 mr-2" /> : "Przetłumacz"}
              </Button>
            </div>
            {translation && (
              <div className="mt-3 bg-green-50 dark:bg-green-900/30 rounded p-2 text-sm whitespace-pre-wrap">
                {translation}
              </div>
            )}
          </Card>
        )}

        {/* === TTS === */}
        {translation && (
          <Card className="p-4 border border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-neutral-800">
            <CardTitle className="flex items-center gap-2 text-base font-medium text-gray-700 dark:text-gray-200">
              <Volume2 className="h-4 w-4 text-purple-600" /> Generowanie głosu
            </CardTitle>
            <div className="flex items-center gap-2 mt-3">
              <select
                value={ttsLang}
                onChange={(e) => setTtsLang(e.target.value)}
                className="border rounded px-2 py-1 text-sm bg-white dark:bg-neutral-900 text-gray-800 dark:text-gray-100"
              >
                <option value="en">English</option>
                <option value="pl">Polski</option>
                <option value="fr">Français</option>
                <option value="de">Deutsch</option>
              </select>
              <Button onClick={generateTTS} disabled={ttsStatus === "queued" || ttsStatus === "generating"}>
                {ttsStatus === "queued" || ttsStatus === "generating" ? (
                  <Loader2 className="animate-spin h-4 w-4 mr-2" />
                ) : (
                  "Generuj głos"
                )}
              </Button>
            </div>
            {ttsError && <p className="mt-2 text-sm text-red-500">{ttsError}</p>}
            {ttsAudioUrl && (
              <div className="mt-3">
                <audio controls className="w-full rounded border dark:border-gray-700">
                  <source src={ttsAudioUrl} type="audio/wav" />
                </audio>
              </div>
            )}
          </Card>
        )}
      </CardContent>
    </Card>
  );
};

export default FileElement;
