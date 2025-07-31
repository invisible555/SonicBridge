@echo off
echo === Uruchamianie wszystkich serwisów ===

REM 🔹 Backend .NET
start "Backend" cmd /k "cd Backend && dotnet run"

REM 🔹 Frontend React/Vite
start "Frontend" cmd /k "cd frontend && npm run dev"

REM 🔹 API Whisper (FastAPI)
start "Whisper API" cmd /k "cd python && uvicorn main:app --host 0.0.0.0 --port 8000"

REM 🔹 Celery Worker
start "Celery Worker" cmd /k "cd python && celery -A celery_worker.celery worker --loglevel=info"

echo === Wszystkie serwisy odpalone ===
pause
