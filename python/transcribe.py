import whisper
import os

# Ładujemy model raz
model = whisper.load_model("small")

def transcribe_file(file_path: str, language: str = None) -> str:
    """
    Transkrybuje plik audio za pomocą Whisper.
    """
    if not os.path.exists(file_path):
        raise FileNotFoundError(f"Plik nie istnieje: {file_path}")

    result = model.transcribe(file_path, language=language)
    return result["text"]
