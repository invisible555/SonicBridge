from fastapi import FastAPI, Form
from fastapi.responses import JSONResponse, FileResponse
from celery_worker_tts import generate_tts_task
import os

app = FastAPI(
    title="TTS API",
    description="API dla generowania mowy z tekstu (Coqui TTS)",
    version="1.0.0",
)

OUTPUT_DIR = "/shared/UserFiles/TTS"
os.makedirs(OUTPUT_DIR, exist_ok=True)


# =====================================
#  Dodanie zadania TTS do kolejki Celery
# =====================================
@app.post("/api/tts/generate")
async def generate_tts(
    text: str = Form(...),
    voice: str = Form("tts_models/en/ljspeech/tacotron2-DDC"),  # model Coqui
    output_name: str = Form("output.wav"),
):
    """Tworzy zadanie TTS w tle (kolejka Celery)."""
    task = generate_tts_task.delay(text, voice, output_name)
    return JSONResponse({"status": "queued", "task_id": task.id})


# =====================================
#  Sprawdzanie statusu zadania TTS
# =====================================
@app.get("/api/tts/status/{task_id}")
async def get_tts_status(task_id: str):
    """Zwraca status zadania Celery."""
    result = generate_tts_task.AsyncResult(task_id)

    if result.state == "PENDING":
        return {"status": "pending"}

    elif result.state == "SUCCESS":
        filename = result.result  # tylko nazwa pliku, np. "output.wav"
        return {"status": "done", "file_name": filename}

    elif result.state == "FAILURE":
        return {"status": "failed", "error": str(result.info)}

    else:
        return {"status": result.state}


# =====================================
#  Pobranie wygenerowanego pliku audio
# =====================================
@app.get("/api/tts/download/{filename}")
async def download_tts_file(filename: str):
    """Pobiera wygenerowany plik TTS z katalogu output."""
    file_path = os.path.join(OUTPUT_DIR, filename)

    if not os.path.exists(file_path):
        return JSONResponse({"error": "Plik nie istnieje"}, status_code=404)

    return FileResponse(
        file_path,
        media_type="audio/wav",
        filename=filename,
    )
