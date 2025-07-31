from celery import Celery
from transcribe import transcribe_file
import os

# Konfiguracja Celery
celery = Celery(
    "tasks",
    broker=os.getenv("CELERY_BROKER_URL", "redis://localhost:6379/0"),
    backend=os.getenv("CELERY_RESULT_BACKEND", "redis://localhost:6379/0")
)

# Wspólny folder dla plików
USERFILES_DIR = "/shared/UserFiles"

@celery.task(bind=True)
def transcribe_audio(self, file_name: str, language: str = None):
    """
    Transkrypcja pliku o podanej nazwie z katalogu UserFiles.
    """
    try:
        file_path = os.path.join(USERFILES_DIR, file_name)
        text = transcribe_file(file_path, language)
        return {"file": file_name, "text": text}
    except Exception as e:
        return {"error": str(e)}
