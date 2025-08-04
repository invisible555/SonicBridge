from fastapi import FastAPI
from translate import router as translate_router
from transcribe import router as transcribe_router
from status import router as status_router
app = FastAPI()

app.include_router(translate_router)
app.include_router(transcribe_router)
app.include_router(status_router)