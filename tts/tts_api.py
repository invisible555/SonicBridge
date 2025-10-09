from fastapi import FastAPI, UploadFile, Form
from celery_worker_tts import generate_tts_task
from fastapi.responses import JSONResponse
import os

app = FastAPI(title="TTS API", description="API dla generowania mowy z tekstu (Coqui TTS)")

@app.post("/api/tts/generate")
async def generate_tts(
    text: str = Form(...),
    voice: str = Form("tts_models/en/ljspeech/tacotron2-DDC"),  # model Coqui
    output_name: str = Form("output.wav")
):
    """Dodaje zadanie TTS do kolejki Celery."""
    task = generate_tts_task.delay(text, voice, output_name)
    return JSONResponse({"status": "queued", "task_id": task.id})

@app.get("/api/tts/status/{task_id}")
async def get_tts_status(task_id: str):
    """Sprawdza status zadania TTS."""
    result = generate_tts_task.AsyncResult(task_id)
    if result.state == "PENDING":
        return {"status": "pending"}
    elif result.state == "SUCCESS":
        return {"status": "done", "file_path": result.result}
    elif result.state == "FAILURE":
        return {"status": "failed", "error": str(result.info)}
    else:
        return {"status": result.state}
