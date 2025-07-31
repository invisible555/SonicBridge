from fastapi import FastAPI, Form
from fastapi.responses import JSONResponse
from tasks import transcribe_audio

app = FastAPI()

@app.post("/transcribe")
async def transcribe(file_name: str = Form(...), language: str = Form(None)):
    """
    Dodaje zadanie transkrypcji dla pliku zapisanego w /shared/UserFiles.
    """
    task = transcribe_audio.delay(file_name, language)
    return {"task_id": task.id, "status": "queued"}

@app.get("/status/{task_id}")
async def get_status(task_id: str):
    """
    Sprawdza status zadania Celery.
    """
    task = transcribe_audio.AsyncResult(task_id)
    if task.state == "PENDING":
        return {"status": "pending"}
    elif task.state == "SUCCESS":
        return {"status": "done", "result": task.result}
    elif task.state == "FAILURE":
        return {"status": "error", "result": str(task.result)}
    else:
        return {"status": task.state}
