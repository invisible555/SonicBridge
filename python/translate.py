import os
import requests
from fastapi import APIRouter, Form, HTTPException

router = APIRouter()

# można nadpisać w docker-compose: LIBRETRANSLATE_URL=http://libretranslate:5000
LIBRETRANSLATE_URL = os.getenv("LIBRETRANSLATE_URL", "http://libretranslate:5000")

@router.post("/translate")
async def translate(
    text: str = Form(...),
    source_lang: str = Form(...),
    target_lang: str = Form(...)
):
    payload = {"q": text, "source": source_lang, "target": target_lang, "format": "text"}
    try:
        r = requests.post(f"{LIBRETRANSLATE_URL}/translate", json=payload, timeout=30)
        r.raise_for_status()
        data = r.json()
        translated = data.get("translatedText")
        if translated is None:
            raise KeyError("Brak pola 'translatedText' w odpowiedzi LibreTranslate.")
        return {"translated_text": translated}
    except (requests.RequestException, KeyError) as e:
        # 502 – błąd w serwisie downstream
        raise HTTPException(status_code=502, detail=f"LibreTranslate error: {e}")
