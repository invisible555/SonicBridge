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
    voice: str = Form("tts_models/en/ljspeech/tacotron2-DDC"),
    output_name: str = Form("output.wav"),
    user_id: str = Form(...),  # 🆕 identyfikator użytkownika
):
    """Tworzy zadanie TTS w tle (kolejka Celery)."""

    task = generate_tts_task.delay(text, voice, output_name, user_id)
    return JSONResponse({"status": "queued", "task_id": task.id})



# =====================================
#  Sprawdzanie statusu zadania TTS
# =====================================
@app.get("/api/tts/status/{task_id}")
async def get_tts_status(task_id: str):
    result = generate_tts_task.AsyncResult(task_id)

    if result.state == "SUCCESS":
        rel_path = result.result  # np. "1/TTS/output.wav"
        return {"status": "done", "file_path": rel_path}
    elif result.state == "FAILURE":
        return {"status": "failed", "error": str(result.info)}
    elif result.state == "PENDING":
        return {"status": "pending"}
    else:
        return {"status": result.state}

# =====================================
#  Pobranie wygenerowanego pliku audio
# =====================================
@app.get("/api/tts/download/{user_id}/{filename}")
async def download_tts_file(user_id: str, filename: str):
    file_path = os.path.join("/shared/UserFiles", user_id, "TTS", filename)

    if not os.path.exists(file_path):
        return JSONResponse({"error": "Plik nie istnieje"}, status_code=404)

    return FileResponse(file_path, media_type="audio/wav", filename=filename)