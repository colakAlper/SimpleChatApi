# Ollama

Model API içinden indirilmez. Model Docker/Ollama tarafında hazırlanır.

```bash
docker compose exec ollama ollama pull llama3.2:1b
```

API açılışta modeli IHealthCheck ile kontrol eder. Model yoksa uygulama başlamaz ve logda gerekli komut görünür.

Docker network adresi:

```text
http://ollama:11434
```
