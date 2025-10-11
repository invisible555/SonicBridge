import os
from celery import Celery


CELERY_BROKER_URL     = os.getenv("CELERY_BROKER_URL", "redis://redis:6379/0")
CELERY_RESULT_BACKEND = os.getenv("CELERY_RESULT_BACKEND", "redis://redis:6379/0")

celery = Celery("sonicbridge", broker=CELERY_BROKER_URL, backend=CELERY_RESULT_BACKEND)
celery.conf.update(
    task_track_started=True,
    worker_prefetch_multiplier=1,
    result_expires=3600,
    timezone="UTC",
)
celery.conf.task_default_queue = "whisper"

# tasks są w tasks.py, więc nie trzeba autodiscover – po imporcie będzie OK
import tasks