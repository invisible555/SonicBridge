# ---- Stage 1: base image ----
FROM python:3.10-slim AS whisper-base

# Instalacja zależności systemowych
RUN apt-get update && apt-get install -y ffmpeg && rm -rf /var/lib/apt/lists/*

# Ustawienie katalogu roboczego
WORKDIR /app

# Kopiujemy plik requirements osobno (dla cache)
COPY requirements.txt .

# Instalacja paczek
RUN pip install --no-cache-dir --timeout=300 --retries=10 -r requirements.txt

# 🔽 Pobranie modelu Whisper przy buildzie (raz!)
#    Zapisze się w /root/.cache/whisper (zmapowanym do ./whisper_models)
RUN python - <<EOF
import whisper, os
model_name = "large-v3"
print(f"📦 Pobieram model Whisper: {model_name}")
model = whisper.load_model(model_name, download_root="/root/.cache/whisper")
print("✅ Model Whisper został pobrany i zapisany w /root/.cache/whisper")
EOF

# Kopiujemy kod aplikacji
COPY . .

# Domyślny port aplikacji FastAPI
EXPOSE 8000

CMD ["python", "main.py"]
