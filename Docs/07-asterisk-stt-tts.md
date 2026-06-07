# Asterisk, STT ve TTS Notları

Bu projede kod yalnızca .NET API, Redis ve Ollama iletişimi için yazıldı.

## Asterisk

Çağrı dialplan ile karşılanır. Ses STT ile yazıya çevrilir. Yazı API'ye gönderilir. API cevabı TTS ile sese çevrilip çağrıya oynatılır.

## STT

Lokal ve ücretsiz kullanım için `whisper.cpp` tercih edilebilir.

## TTS

Lokal ve hızlı kullanım için `Piper` tercih edilebilir.

## Yapay Zeka

Lokal model için `Ollama` tercih edildi. Docker ile kolay çalışır ve .NET API HTTP üzerinden konuşur.
