## 1. Asterisk Entegrasyonu

### Çağrı nasıl karşılanır?

* Asterisk tarafında bir dahili numara tanımlanır.
* Kullanıcı bu numarayı arar.
* Asterisk çağrıyı `Answer()` komutu ile karşılar.
* Kullanıcıdan ses alınır.

### API nasıl çağrılır?

* Kullanıcının sesi önce yazıya çevrilir.
* Oluşan yazı API’ye gönderilir.
* API endpoint’i:

```text
POST /api/v1/chat
```

* Örnek istek:

```json
{
  "message": "Merhaba"
}
```

### Ses nasıl oynatılır?

* API’den gelen cevap yazı formatındadır.
* Bu yazı TTS ile ses dosyasına çevrilir.
* Oluşan ses dosyası Asterisk ile kullanıcıya dinletilir.
* Asterisk tarafında `Playback()` komutu kullanılabilir.

---

## 2. STT — Speech To Text

### Kullanıcının sesi yazıya çevrilecek olsa hangi teknolojiyi tercih ederdiniz?

* `Whisper` tercih ederdim.
* Lokal çalışması istenirse `whisper.cpp` tercih ederdim.

### Neden?

* Ücretsiz kullanılabilir.
* Lokal çalışabilir.
* Türkçe desteği iyidir.
* Basit şekilde ses dosyasını yazıya çevirebilir.

---

## 3. TTS — Text To Speech

### API cevabı ses olarak okutulacak olsa hangi teknolojiyi tercih ederdiniz?

* `Piper TTS` tercih ederdim.

### Neden?

* Lokal çalışabilir.
* Ücretsizdir.
* Hafiftir.
* `.wav` ses dosyası üretebilir.
* Asterisk ile kullanılabilir.

---

## 4. Yapay Zeka

### Ücretsiz veya lokal çalışabilen hangi yapay zeka çözümünü tercih ederdiniz?

* Basit işler için `Ollama` tercih ederdim.
* Karmaşık işlemler için ücretsiz ya da düşük maliyetle farklı alternatifler de kullanabilirdim.

### Neden?

* Lokal çalışabilir.
* Docker ile kolay kurulabilir.
* .NET API üzerinden kolayca HTTP ile çağrılabilir.

---

## 5. Test Süreci

### Proje nasıl çalıştırılır?

* Önce Docker servisleri başlatılır:

```bash
docker compose up -d
```

### API nasıl test edilir?

* `curl` ile test edilebilir ya da direkt olarak `http/chatbot.http` dosyasında yazılmış hazır requestler kullanılabilir.

```bash
curl -X POST http://localhost:8080/api/v1/chat \
  -u admin:admin123 \
  -H "Content-Type: application/json" \
  -d "{\"message\":\"Merhaba\"}"
```

* Beklenen cevap örneği:

```json
{
  "success": true,
  "message": "Answer is ready.",
  "data": {
    "response": "Merhaba, size nasıl yardımcı olabilirim?"
  }
}
```

### Asterisk ile nasıl test edilir?

* Asterisk üzerinde bir test dahili numarası tanımlanır.
* Bu numara arandığında çağrı karşılanır.
* Kullanıcının sesi kaydedilir.
* Ses STT ile yazıya çevrilir.
* Yazı API’ye gönderilir.
* API cevabı TTS ile sese çevrilir.
* Oluşan ses kullanıcıya dinletilir.

### Hangi araçlar kullanılır?

* Docker
* Postman
* Ollama
* Redis
* .NET SDK
* Visual Studio veya Rider

---

## 6. Teknik Tercihler

* API için `.NET 8` kullanıldı.
* Lokal yapay zeka için `Ollama` kullanıldı (ücretsiz ve docker üzerinden kolay kurulum).
* Aynı sorular için `Redis Cache` kullanıldı (global tercih).
* Loglama için `Serilog` kullanıldı (kolay kurulum + kolay kullanım + global tercih).
* Basit güvenlik için `Basic Authentication` kullanıldı (hızlı güvenlik katmanının oluşması).
* İstek yönetimi için `CQRS` ve `MediatR` kullanıldı.
* Request doğrulama için `FluentValidation` kullanıldı (ValidationBehavior ile Command'ları otomatik olarak tespit edebilir).
* Docker ile kolay çalıştırılabilir bir yapı kuruldu.