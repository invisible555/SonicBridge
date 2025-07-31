# Wersja bazowa
FROM python:3.11-slim

# Ustawienia środowiska
ENV PYTHONDONTWRITEBYTECODE=1
ENV PYTHONUNBUFFERED=1

# Instalacja zależności systemowych (ffmpeg dla Whisper)
RUN apt-get update && apt-get install -y ffmpeg && rm -rf /var/lib/apt/lists/*

# Katalog roboczy
WORKDIR /app

# Instalacja zależności Pythona
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Skopiuj projekt
COPY . .

# Port API
EXPOSE 8000

# Domyślny start (można nadpisać w docker-compose)
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]
