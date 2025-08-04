from fastapi import APIRouter, Form
from tasks import transcribe_audio

router = APIRouter()

@router.post("/transcribe")
async def transcribe(
    file_name: str = Form(...),
    language: str | None = Form(None),
    user_id: int | None = Form(None),
):
    task = transcribe_audio.delay(file_name, language, user_id)
    return {"task_id": task.id, "status": "queued"}
