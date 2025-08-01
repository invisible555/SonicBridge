import whisper
import os



def transcribe_file(file_path: str, language: str = None) -> str:
    if not os.path.exists(file_path):
        raise FileNotFoundError(f"Plik nie istnieje: {file_path}")
    model = whisper.load_model("large")
    print(f"heheheh1")  # 🔹 LOG DEBUG
    result = model.transcribe(file_path, language=language)
    print(f"hwhwhhwhhe2")  # 🔹 LOG DEBUG
    text = result["text"]

    # 📌 Tworzymy ścieżkę dla pliku wynikowego
    base, _ = os.path.splitext(file_path)
    lang = language if language else "unknown"
    out_file = f"{base}_original_{lang}.txt"

    # 📌 Zapis transkrypcji do pliku
    with open(out_file, "w", encoding="utf-8") as f:
        f.write(text)

    return out_file  # zwraca tylko nazwę pliku wynikowego