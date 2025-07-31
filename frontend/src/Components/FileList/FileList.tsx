import React, { useEffect, useState, useCallback } from "react";
import FileElement from "../FileElement/FileElement";
import axiosInstance from "../../Utils/axiosConfig";

interface FileData {
  id: number;
  fileName: string;
  fileType?: string;
  filePath?: string;
}

const FileList: React.FC = () => {
  const [files, setFiles] = useState<FileData[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const getFiles = useCallback(async () => {
    try {
      const response = await axiosInstance.get("/api/file/list");
      setFiles(response.data);
    } catch (err) {
      setError("Nie udało się pobrać plików.");
      console.error("Error fetching files:", err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    getFiles();
  }, [getFiles]);

  if (loading) return <p className="text-center mt-6">Ładowanie plików...</p>;
  if (error) return <p className="text-center mt-6 text-red-500">{error}</p>;

  return (
    <div className="flex flex-col gap-3 w-full p-4">
      {files.length > 0 ? (
        files.map((file) => (
          <FileElement key={file.id} fileName={file.fileName} />
        ))
      ) : (
        <p className="text-gray-500 text-center">Brak plików do wyświetlenia.</p>
      )}
    </div>
  );
};

export default FileList;
