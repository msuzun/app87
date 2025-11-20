# ⚡ QUICK START - Hızlı Başlangıç

**5 dakikada** projeyi çalıştır!

---

## 🚀 Hızlı Kurulum (5 Adım)

### 1️⃣ Unity Projesi Oluştur
```
Unity Hub > New Project
Template: 2D (URP)
Unity Version: 2021.3 LTS+
```

### 2️⃣ Dosyaları Kopyala
```
Assets/_Game klasörünü Unity projesine kopyala
```

### 3️⃣ Paketleri Yükle
```
Window > Package Manager
- Input System (1.5.0+) ✅
- 2D Animation (9.0.0+) ✅
- TextMeshPro ✅
```

### 4️⃣ Input System Aktifleştir
```
Edit > Project Settings > Player
Active Input Handling: Input System Package (New)
Unity'yi restart et
```

### 5️⃣ Test Sahnesi Kur
```
1. Boş sahne oluştur
2. GameManager GameObject ekle + tüm manager script'lerini ekle
3. Player prefab'ı sahneye sürükle
4. Main Camera'ya CameraFollow ekle
5. Play!
```

---

## 🎮 İlk Test

### Minimal Setup (1 Dakika)

**Hierarchy:**
```
- GameManager
  + GameManager.cs
  + InputHandler.cs
  + ObjectPooler.cs
  + SoundManager.cs
  + AITokenManager.cs

- Main Camera
  + CameraFollow.cs (Target: Player)

- Player (0, 0, 0)
  (Prefab veya manuel setup)
```

**Play Mode'a Gir**: ✅ Çalışıyor!

---

## 📋 Kontrol Testi

Test etmek için:

- **WASD**: Hareket ediyor mu? ✅
- **Shift**: Koşuyor mu? ✅
- **Z**: Saldırı animasyonu? ✅
- **Space**: Zıplıyor mu? ✅
- **Shift (Tap)**: Dash yapıyor mu? ✅

**Hepsi çalışıyorsa: Başarılı! 🎉**

---

## 🆘 Hızlı Sorun Giderme

### Input Çalışmıyor
```
Project Settings > Player > Active Input Handling
"Input System Package (New)" olmalı
Unity'yi restart et
```

### Karakterler Birbirinden Geçiyor
```
Rigidbody2D > Collision Detection: Continuous
Physics2D > Layer Collision Matrix kontrol et
```

### Animasyon Yok
```
Animator Controller bağlı mı?
Parameters oluşturulmuş mu? (IsWalking, IsRunning, vb.)
```

### State Machine Hataları
```
Console'u kontrol et
PlayerStateMachine Awake() çağrılıyor mu?
State instance'ları null mı?
```

---

## 📚 Sonraki Adımlar

### Yeni Başlayanlar
1. ✅ [CONTROLS.md](CONTROLS.md) - Kontrolleri öğren
2. ✅ [SETUP_GUIDE.md](SETUP_GUIDE.md) - Detaylı kurulum
3. ✅ Test Gym sahnesi kur (pratik için)

### Geliştiriciler
1. ✅ [HYBRID_SYSTEM.md](HYBRID_SYSTEM.md) - Mimariyi anla
2. ✅ [ARCHITECTURE.md](ARCHITECTURE.md) - Derin teknik bilgi
3. ✅ [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) - Dosya yapısı

---

## 🎯 İlk Hedefler

### Checkpoint 1: Temel Hareket ✅
```
[ ] Karakter hareket ediyor
[ ] Koşma çalışıyor
[ ] Zıplama çalışıyor
[ ] Dash çalışıyor
```

### Checkpoint 2: Basit Combat ✅
```
[ ] Light attack animasyonu
[ ] Hitbox aktif oluyor
[ ] Dummy düşman hasar alıyor
[ ] Kombo sayacı çalışıyor
```

### Checkpoint 3: Düşman AI ✅
```
[ ] Düşman spawn oluyor
[ ] Oyuncuyu kovalıyor
[ ] Saldırı yapıyor
[ ] Token sistemi çalışıyor
```

---

## 💡 Pro Tips

1. **Scene View'da Test Et**
   - Gizmo'ları görmek için
   - Hitbox range'leri kontrol et

2. **Console'u Aç**
   - Debug logları takip et
   - Hataları hemen gör

3. **Inspector'da Watch Et**
   - State değişimlerini gör
   - Stamina/Rage barlarını takip et

4. **Slow Motion Kullan**
   - Edit > Preferences > General
   - Time Scale: 0.5x (Test için)

---

## 🎮 Test Modu Önerisi

### Kolay Test İçin:
```csharp
// PlayerController.cs - Update() içine ekle
if (Input.GetKeyDown(KeyCode.T))
{
    Debug.Log($"State: {stateMachine.CurrentState.GetType().Name}");
    Debug.Log($"Grounded: {isGrounded}, Running: {isRunning}");
    Debug.Log($"Stamina: {staminaCurrent}/{staminaMax}");
}
```

**T tuşu** ile durumu görebilirsin!

---

## ✅ Kurulum Başarılı mı?

Eğer bunlar çalışıyorsa **başarılısın**:

- ✅ WASD ile hareket
- ✅ Shift ile koş
- ✅ Space ile zıpla
- ✅ Z ile saldır
- ✅ Console'da hata yok

**Tebrikler!** 🎉 Artık geliştirmeye başlayabilirsin!

---

## 🔗 Hızlı Linkler

- [Ana README](README.md)
- [Detaylı Kurulum](SETUP_GUIDE.md)
- [Kontroller](CONTROLS.md)
- [Hibrit Sistem](HYBRID_SYSTEM.md)

---

**Takıldın mı?** Console'u kontrol et ve hata mesajını kopyala!

**Başarılı oldun mu?** Harika! Şimdi kendi özelliklerini eklemeye başla! 🚀

