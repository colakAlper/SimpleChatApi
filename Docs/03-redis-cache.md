# Redis Cache

API açılışta modeli IHealthCheck ile kontrol eder. Redis yoksa uygulama başlamaz ve logda gerekli komut görünür.

Aynı soru 10 dakika boyunca Redis üzerinden döner.

Akış:

1. Cache kontrol edilir.
2. Cevap varsa direkt döner.
3. Cevap yoksa Ollama çağrılır.
4. Cevap Redis'e yazılır.