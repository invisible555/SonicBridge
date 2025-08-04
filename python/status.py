from fastapi import APIRouter
from celery_worker import celery  # globalny Celery app

router = APIRouter()

@router.get("/status/{task_id}")
async def get_status(task_id: str):
    task = celery.AsyncResult(task_id)

    payload = {"task_id": task_id, "status": task.state}

    if task.state == "SUCCESS":
        payload["result"] = task.result
        return payload

    if task.state == "FAILURE":
        payload["error"] = str(task.info)  # wyjątek z taska
        return payload

    # STARTED / RETRY / PENDING — pokaż meta, jeśli task jej używa
    if isinstance(task.info, dict):
        payload["meta"] = task.info

    payload["result"] = None
    return payload
