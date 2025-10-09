FROM python:3.10-slim

WORKDIR /app
COPY . .

# Instalacja zależności systemowych (dla PyTorch i audio)
RUN apt-get update && apt-get install -y git espeak ffmpeg libsndfile1 && rm -rf /var/lib/apt/lists/*

RUN pip install --no-cache-dir -r requirements.txt

ENV CELERY_BROKER_URL=redis://redis:6379/0
ENV CELERY_RESULT_BACKEND=redis://redis:6379/0

CMD ["uvicorn", "tts_api:app", "--host", "0.0.0.0", "--port", "8100"]
