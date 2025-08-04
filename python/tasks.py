from celery_worker import celery
import os
import whisper

USERFILES_DIR = "/shared/UserFiles"

@celery.task(bind=True, name="tasks.transcribe_audio")
def transcribe_audio(self, file_name: str, language: str | None = None, user_id: int | None = None):
    try:
        # budowa ścieżki
        if user_id:
            file_path = os.path.join(USERFILES_DIR, str(user_id), file_name)
        else:
            file_path = os.path.join(USERFILES_DIR, file_name)

        # progres (zobaczysz w /status)
        self.update_state(state="STARTED", meta={"step": "loading_model"})

        # ładuj model wewnątrz taska
        model = whisper.load_model("small")

        self.update_state(state="STARTED", meta={"step": "transcribing", "file": file_path})
        result = model.transcribe(file_path, language=language)
        text = result.get("text", "")

        # zapisz plik obok audio
        base, _ = os.path.splitext(file_path)
        lang = language or result.get("language", "unknown")
        out_file = f"{base}_original_{lang}.txt"
        with open(out_file, "w", encoding="utf-8") as f:
            f.write(text)

        return {"status": "done", "result": {"file_path": out_file, "language": lang}}
    except Exception as e:
        # Celery oznaczy task jako FAILURE, a /status pokaże 'error': ...
        raise e
