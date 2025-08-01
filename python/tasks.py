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
def transcribe_audio(self, file_name: str, language: str = None, user_id: int = None):
    try:
        if user_id:
            file_path = os.path.join(USERFILES_DIR, str(user_id), file_name)
        else:
            file_path = os.path.join(USERFILES_DIR, file_name)

        print(f"DEBUG: Start transkrypcji pliku {file_path}, {language}")  # 🔹 LOG DEBUG

        out_file = transcribe_file(file_path, language)

        print(f"DEBUG: Wynik zapisany w {out_file}")  # 🔹 LOG DEBUG

        return {
            "status": "done",
            "result": {"file_path": out_file}
        }
    except Exception as e:
        return {
            "status": "error",
            "result": {"text": str(e)}
        }