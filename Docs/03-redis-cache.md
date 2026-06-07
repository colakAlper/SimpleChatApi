# Redis Cache

Aynı soru 10 dakika boyunca Redis üzerinden döner.

Akış:

1. Cache kontrol edilir.
2. Cevap varsa direkt döner.
3. Cevap yoksa Ollama çağrılır.
4. Cevap Redis'e yazılır.

