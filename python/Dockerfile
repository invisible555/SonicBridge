# ---- Stage 1: base image ----
FROM python:3.10-slim AS whisper-base

# Instalacja zależności systemowych
RUN apt-get update && apt-get install -y ffmpeg && rm -rf /var/lib/apt/lists/*

# Ustawienie katalogu roboczego
WORKDIR /app

# Kopiujemy plik requirements osobno (dla cache)
COPY requirements.txt .

# Instalacja paczek
RUN pip install --no-cache-dir -r requirements.txt

# Pobranie modelu przy buildzie (opcjonalnie - można wyłączyć i pobierać przy starcie)
RUN python -c "import whisper; whisper.load_model('large')"

# Kopiujemy kod aplikacji
COPY . .

# Domyślny port (zmień na ten, którego używasz w FastAPI)
EXPOSE 8000

CMD ["python", "main.py"]
