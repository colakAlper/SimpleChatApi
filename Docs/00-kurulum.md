# Kurulum

Amaç çok basit: .NET API çalışsın, Ollama'ya mesaj atsın, Redis aynı soruyu 10 dakika cache'lesin.

```bash
cp .env.example .env
docker compose up -d redis ollama
docker compose exec ollama ollama pull llama3.2:1b
docker compose up --build api
```

Test-1: Aşağıdaki dosyadan statik olarak endpointleri kontrol edebilirsiniz.

```bash
http/chatbot.http
```

Test-2:

```bash
curl -X POST http://localhost:8080/api/chat \
  -u admin:admin123 \
  -H "Content-Type: application/json" \
  -d "{\"message\":\"Merhaba\"}"
```
