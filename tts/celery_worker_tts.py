import os
from celery import Celery
from TTS.api import TTS

# ==========================
#  Konfiguracja Celery
# ==========================
CELERY_BROKER_URL = os.getenv("CELERY_BROKER_URL", "redis://redis:6379/0")
CELERY_RESULT_BACKEND = os.getenv("CELERY_RESULT_BACKEND", "redis://redis:6379/0")

celery = Celery("tts_worker", broker=CELERY_BROKER_URL, backend=CELERY_RESULT_BACKEND)

# ==========================
#  Ustawienia modelu TTS
# ==========================
DEFAULT_MODEL = "tts_models/en/ljspeech/tacotron2-DDC"
OUTPUT_DIR = "/shared/UserFiles/TTS"
os.makedirs(OUTPUT_DIR, exist_ok=True)

# Cache modeli (żeby nie ładować ponownie)
tts_cache = {}

def get_tts_model(model_name: str = DEFAULT_MODEL):
    """Zwraca instancję modelu TTS (z cache)."""
    if model_name not in tts_cache:
        print(f"Ładowanie modelu TTS: {model_name}")
        tts_cache[model_name] = TTS(model_name=model_name, progress_bar=False, gpu=False)
    return tts_cache[model_name]


# ==========================
#  Zadanie Celery
# ==========================
@celery.task(name="generate_tts_task")
def generate_tts_task(
    text: str,
    voice_model: str = DEFAULT_MODEL,
    output_name: str = "output.wav",
    user_id: str = "default"
):
    """Tworzy nagranie audio z tekstu i zapisuje do folderu użytkownika."""
    try:
        # 🆕 Ścieżka użytkownika
        user_dir = os.path.join("/shared/UserFiles", user_id, "TTS")
        os.makedirs(user_dir, exist_ok=True)

        output_path = os.path.join(user_dir, output_name)

        # Załaduj model (z cache)
        tts_model = get_tts_model(voice_model)
        tts_model.tts_to_file(text=text, file_path=output_path)

        # Zwróć względną ścieżkę (dla API)
        return os.path.relpath(output_path, "/shared/UserFiles")
    except Exception as e:
        raise RuntimeError(f"Błąd TTS: {e}")
