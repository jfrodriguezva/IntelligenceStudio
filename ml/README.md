# MIS ML Runtime

Servicio privado de modelado para Madrid Intelligence Studio. El endpoint inicial ofrece una línea base explícita; no representa un modelo entrenado ni debe utilizarse como recomendación de apuesta.

```powershell
python -m pip install -r requirements.txt
uvicorn app.main:app --reload --port 8001
```
