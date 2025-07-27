import type FileElementProps from "./FileElementProps";
//import axiosInstance from "../../Utils/axiosConfig";
/*
const getFiles = async () => {
    try {
        const response = await axiosInstance.get('/files');
        return response.data;
    } catch (error) {
        console.error("Error fetching files:", error);
        throw error;
    }
}
*/
const FileElement: React.FC<FileElementProps> = ({ fileName }) => {
    return (
        <div className="bg-white rounded-lg shadow-md p-4 flex items-center space-x-4 hover:shadow-lg transition-shadow">
            <div className="flex-shrink-0">
                <svg
                    xmlns="http://www.w3.org/2000/svg"
                    className="h-8 w-8 text-gray-500"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                >
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6l4 2m0 0l4-2m-4 2V6m0 6l-4-2m0 0V6m0 6l-4-2m8 10H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v12a2 2 0 01-2 2h-3z" />
                </svg>
            </div>
            <div className="flex-1">
                <p className="text-gray-800 font-medium">{fileName}</p>
            </div>
        </div>
    );
}
export default FileElement