# Response Formatı

Response modeli bilerek küçük tutuldu.

## Başarılı cevap

```json
{
  "success": true,
  "message": "Answer is ready.",
  "data": {
    "response": "Merhaba, size nasıl yardımcı olabilirim?"
  }
}
```

## Datasız cevap

```json
{
  "success": true,
  "message": "Ollama model is ready.",
  "data": null
}
```

## Hatalı cevap

```json
{
  "success": false,
  "message": "Validation failed.",
  "data": {
    "Message": ["Message cannot be empty."]
  }
}
```

Controller kullanımı:

```csharp
return Ok("Ollama model is ready.");
return Ok(result, "Answer is ready.");
```
