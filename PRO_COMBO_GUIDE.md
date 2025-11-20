# 🎮 PRO-LEVEL COMBO SYSTEM - Kullanım Rehberi

**Data-Driven, Branching Combos, Input Buffering, Cancel Windows**

Street Fighter / Devil May Cry / Crazy Flasher tarzı profesyonel combo sistemi!

---

## 📋 İçindekiler

- [Sistem Özellikleri](#-sistem-özellikleri)
- [Kurulum](#-kurulum)
- [Combo Asset Oluşturma](#-combo-asset-oluşturma)
- [Branching Combos](#-branching-combos)
- [Kullanım Örnekleri](#-kullanım-örnekleri)
- [Troubleshooting](#-troubleshooting)

---

## ✨ Sistem Özellikleri

### 1. **Data-Driven Design**
```
❌ Kod yazmaya GEREK YOK!
✅ Tüm kombolar ScriptableObject ile tanımlanır
✅ Unity Editor'de drag-and-drop
```

### 2. **Branching Combos**
```
Light → Light → Light  (Basic 3-hit combo)
Light → Light → Heavy  (Launcher combo)
Light → Heavy          (Quick launcher)
```

### 3. **Input Buffering**
```
Oyuncu tuşa erken basabilir (0.2s önce)
Sistem bunu buffer'da saklar
Zamanı gelince otomatik uygular
= LAG HİSSİ YOK!
```

### 4. **Cancel Windows**
```
Animasyonun sadece %30-%80 arasında kombo yapılabilir
Bu "timing skill" gerektirir
Spam yapmayı engeller
```

### 5. **Hit Stop**
```
Vuruş anında oyun 0.1 saniye donar
Crazy Flasher'daki o tatmin edici his!
```

---

## 🛠️ Kurulum

### Adım 1: Player'a Component Ekle

```
Hierarchy'de Player seç
Inspector > Add Component:
  - ProComboSystem ✓
  - InputBuffer ✓
```

**Not**: `Animator` ve `Rigidbody2D` zaten olmalı.

---

### Adım 2: Mevcut Sistemle Uyumluluk

Eğer mevcut `PlayerCombat` ve `ComboSystem` varsa:

**Seçenek A**: Her ikisini kullan (farklı karakterler için)
```
Axel → ProComboSystem (branching combos)
Boss → Eski ComboSystem (basit)
```

**Seçenek B**: Tamamen ProComboSystem'e geç
```
PlayerCombat → Disable
ComboSystem → Disable
ProComboSystem → Enable ✓
```

---

## 📦 Combo Asset Oluşturma

### Asset Oluşturma

```
Project penceresinde sağ tıkla
Create > Neon Syndicate > Combat > Combo Move

İsim: "Axel_Light_1"
```

---

### Asset Ayarlama (Inspector)

#### **Animation & Visuals**
```
Animation Name: "Punch1"
  (Animator'daki state ismi)

Animation Length: 0.5
  (Animasyon süresi - Animator'da ayarlananla aynı olmalı)

Hit Effect Name: "HitSpark"
  (Object Pooler'daki efekt ismi)
```

#### **Combat Data**
```
Damage: 10
  (Bu saldırının hasar miktarı)

Knockback: (5, 2)
  (x: yatay, y: dikey)

Hit Stop Duration: 0.1
  (Vuruş anında donma süresi)

Is Launcher: ☐ false
  (Bu saldırı düşmanı havaya kaldırır mı?)

Is Finisher: ☐ false
  (Kombo'nun son vuruşu mu?)
```

#### **Timing (Cancel Windows)**
```
Min Cancel Time: 0.3
  (Animasyonun %30'unda kombo yapılabilir)

Max Cancel Time: 0.8
  (Animasyonun %80'inde kombo penceresi kapanır)

Örnek:
  0.5 saniyelik animasyon
  0.15s (30%) - 0.40s (80%) arası kombo yapılabilir
```

#### **Movement**
```
Forward Momentum: 2.0
  (Saldırı sırasında ileri hareket)

Can Use In Air: ☐ false
  (Havada kullanılabilir mi?)
```

#### **Audio**
```
Attack Sound Name: "Whoosh_Attack"
Hit Sound Name: "Hit_Impact"
```

---

## 🌳 Branching Combos

### Basit 3-Hit Combo

**Asset'ler:**
1. `Axel_Light_1`
2. `Axel_Light_2`
3. `Axel_Light_3`

**Bağlantı:**

#### Axel_Light_1:
```
Next Moves: (1 eleman)
  [0]:
    Required Input: Light
    Next Move: Axel_Light_2
    Requires Airborne: ☐ false
    Minimum Combo Count: 0
```

#### Axel_Light_2:
```
Next Moves: (1 eleman)
  [0]:
    Required Input: Light
    Next Move: Axel_Light_3
```

#### Axel_Light_3:
```
Next Moves: (0 eleman)
  (Kombo burada biter)
```

**Sonuç**: Z → Z → Z (3-hit combo)

---

### Branching Combo (Dallanma)

#### Axel_Light_2 (güncellenmiş):
```
Next Moves: (2 eleman)
  [0]:
    Required Input: Light
    Next Move: Axel_Light_3
    (Devam eden kombo)
  
  [1]:
    Required Input: Heavy
    Next Move: Axel_Heavy_Launcher
    (Farklı dallanma)
```

**Sonuç**:
- Z → Z → Z (Basic combo)
- Z → Z → X (Launcher combo)

---

### Havada Kombo

**Asset**: `Axel_Air_Attack`

```
Can Use In Air: ☑ true
```

**Launcher'dan bağlantı:**

#### Axel_Heavy_Launcher:
```
Is Launcher: ☑ true

Next Moves:
  [0]:
    Required Input: Jump
    Next Move: Axel_Air_Attack
    Requires Airborne: ☑ true
```

**Sonuç**: Z → Z → X (Launcher) → Space → Z (Air combo)

---

## 🎯 Kullanım Örnekleri

### Örnek 1: Axel'in Basic Combo'su

#### Asset Yapısı:
```
Axel_Light_1 (Jab)
  └→ Light: Axel_Light_2

Axel_Light_2 (Cross)
  ├→ Light: Axel_Light_3
  └→ Heavy: Axel_Heavy_Launcher

Axel_Light_3 (Uppercut)
  └→ (Finisher)

Axel_Heavy_Launcher (Kick Launcher)
  └→ Jump: Axel_Air_Combo
```

#### Inspector Ayarları:

**Axel_Light_1** (Jab):
```
Animation Name: Punch_Jab
Animation Length: 0.4
Damage: 10
Min Cancel: 0.25 (0.1s)
Max Cancel: 0.75 (0.3s)
Forward Momentum: 1.5
```

**Axel_Light_2** (Cross):
```
Animation Name: Punch_Cross
Animation Length: 0.5
Damage: 12
Min Cancel: 0.3
Max Cancel: 0.8
Forward Momentum: 2.0
```

**Axel_Light_3** (Uppercut):
```
Animation Name: Punch_Uppercut
Animation Length: 0.6
Damage: 15
Is Finisher: ☑ true
Min Cancel: 1.0 (No cancel - finisher)
```

**Axel_Heavy_Launcher** (Kick):
```
Animation Name: Kick_Launcher
Animation Length: 0.7
Damage: 20
Knockback: (8, 10) - Strong upward
Is Launcher: ☑ true
Min Cancel: 0.4
Max Cancel: 0.9
```

---

### Örnek 2: Kombo Ağacı Diyagramı

```
                    START
                      |
                   [Light]
                      |
                 Light_1 (Jab)
                /           \
            [Light]       [Heavy]
              /               \
         Light_2 (Cross)    Heavy_1 (Launcher)
        /         \              |
    [Light]    [Heavy]        [Jump]
      /            \              |
Light_3      Heavy_Launcher   Air_Combo
(Finisher)    (Launcher)      /       \
                           [Light]   [Heavy]
                             /           \
                      Air_Light     Air_Heavy
                      (Juggle)      (Smash)
```

---

## 🎮 ProComboSystem Inspector Ayarları

```
Default Light Opener: Axel_Light_1
  (Z tuşu ile başlayan combo)

Default Heavy Opener: Axel_Heavy_1
  (X tuşu ile başlayan combo)

Combo Timeout: 2.0
  (2 saniye vuruş yoksa combo sıfırlanır)

Show Debug Info: ☑ (Test için)
```

---

## 🎨 Animator Setup

### Animation States

Her combo asset için Animator'da state olmalı:

```
Animator Controller:
  - Idle
  - Punch_Jab (Axel_Light_1 için)
  - Punch_Cross (Axel_Light_2 için)
  - Punch_Uppercut (Axel_Light_3 için)
  - Kick_Launcher (Axel_Heavy_Launcher için)
  - Air_Combo (Axel_Air_Attack için)
```

### Transitions

**ÖNEMLİ**: Transitions'ı manuel yapmaya gerek YOK!

ProComboSystem `CrossFade()` kullanır, otomatik geçiş yapar.

Sadece şunlar gerekli:
```
Any State → Idle (Exit Time: false)
```

---

## 🔊 Audio Setup

SoundManager'a ses efektleri ekle:

```
Whoosh_Attack: Saldırı sıfır sesi
Hit_Impact: Vuruş sesi
Heavy_Swing: Ağır saldırı sesi
Air_Whoosh: Havada saldırı sesi
```

---

## 🎯 Test Etme

### Debug Mode

ProComboSystem'de:
```
Show Debug Info: ☑ true
```

**Ekranda görünecekler**:
```
=== PRO COMBO DEBUG ===
Attacking: true
Current Move: Axel_Light_2
Timer: 0.32 / 0.50
In Cancel Window: true
Combo Counter: 2
Buffer Count: 0
```

### Test Senaryoları

#### Test 1: Basic Combo
```
1. Play mode'a gir
2. Z tuşuna 3 kez bas (hızlıca)
3. ✓ 3-hit combo yapılmalı
4. Debug: "Combo Counter: 3" görmeli
```

#### Test 2: Branching
```
1. Z → Z → X bas
2. ✓ Launcher animasyonu oynamalı
3. ✓ Düşman havaya kalkmalı (Is Launcher: true)
```

#### Test 3: Input Buffer
```
1. Z bas (1. saldırı başlar)
2. Hemen Z bas (animasyon bitmeden)
3. ✓ 2. saldırı otomatik gelmeli
4. Debug: "Buffer Count: 1" sonra "0"
```

#### Test 4: Cancel Window
```
1. Z bas
2. Animasyon %10'dayken Z bas (çok erken)
3. ✓ Kabul edilmemeli (min cancel: 0.3)
4. Animasyon %50'dayken Z bas
5. ✓ Kabul edilmeli (cancel window içinde)
```

#### Test 5: Hit Stop
```
1. Dummy düşman oluştur
2. Z ile vur
3. ✓ Vuruş anında oyun 0.1s donmalı
4. ✓ Tatmin edici "impact" hissi olmalı
```

---

## 🐛 Troubleshooting

### Problem 1: Kombo Çalışmıyor
```
Sebep: Animator state isimleri yanlış

Çözüm:
✓ ComboMoveSO → Animation Name
✓ Animator → State ismi
İkisi birebir aynı olmalı!
```

### Problem 2: Input Buffer Çalışmıyor
```
Sebep: InputBuffer component eksik

Çözüm:
✓ Player'da InputBuffer.cs var mı?
✓ Buffer Time > 0 mı? (örn: 0.2)
```

### Problem 3: Cancel Window Çalışmıyor
```
Sebep: Timing yanlış ayarlanmış

Çözüm:
✓ Min Cancel < Max Cancel olmalı
✓ Min Cancel genelde 0.3-0.4
✓ Max Cancel genelde 0.7-0.9
```

### Problem 4: Hit Stop Çalışmıyor
```
Sebep: Hitbox entegrasyonu eksik

Çözüm:
✓ Hitbox.cs güncellenmiş mi?
✓ Owner ProComboSystem var mı?
✓ Hit Stop Duration > 0 mı?
```

### Problem 5: Branching Çalışmıyor
```
Sebep: Next Moves yanlış ayarlanmış

Çözüm:
✓ Required Input doğru mu?
✓ Next Move null değil mi?
✓ Airborne/Combo Count şartları sağlanıyor mu?
```

---

## 📊 Performans İpuçları

### Optimization

```csharp
// ✅ İyi: ScriptableObject reference (hızlı)
public ComboMoveSO currentMove;

// ❌ Kötü: String comparison (yavaş)
if (moveNamecurrentMove == "Axel_Light_1") { ... }
```

### Buffer Size

```
Buffer Time = 0.2s (Önerilen)
  - Çok düşük (0.1s): Strict timing, pro players
  - Çok yüksek (0.5s): Çok kolay, spam
```

### Cancel Windows

```
Skill-based gameplay için:
  Min: 0.4 (Geç başlama)
  Max: 0.7 (Erken bitirme)
  = Dar pencere, timing skill gerektirir

Casual gameplay için:
  Min: 0.2 (Erken başlama)
  Max: 0.9 (Geç bitirme)
  = Geniş pencere, kolay combo
```

---

## 🎓 İleri Seviye

### Custom Conditions

Branching'e özel şartlar eklemek:

```csharp
// ComboBranch'e yeni field:
public bool requiresFullStamina = false;

// ProComboSystem'de check:
if (branch.requiresFullStamina && stamina < maxStamina)
{
    continue; // Şart sağlanmadı
}
```

### Combo Modifiers

Hasar çarpanları:

```csharp
// ComboMoveSO'ya:
public float damageMultiplier = 1.0f;

// Hit confirm'de:
float finalDamage = currentMove.damage * currentMove.damageMultiplier;
```

### Chain Cancels

Saldırıyı dash ile iptal etme:

```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.LeftShift) && isAttacking && isInCancelWindow)
    {
        // Dash cancel
        proCombo.ForceInterrupt();
        StartDash();
    }
}
```

---

## 📚 Örnek Combo Database

### Street Fighter Tarzı

```
Light → Light → Light → Heavy (Target Combo)
Light → Light → Special (Cancel into special)
Heavy → Special (Crush Counter)
```

### Devil May Cry Tarzı

```
Light x4 (Basic combo)
Light x2 → Pause → Light x2 (Delay combo)
Light x2 → Heavy → Jump → Air Combo (Launcher combo)
```

### Crazy Flasher Tarzı

```
Light x3 (Punch combo)
Light x2 → Heavy (Launcher)
Heavy → Jump → Light → Heavy (Juggle)
```

---

## ✅ Checklist

### Asset Hazırlığı
- [ ] Tüm combo move'ları oluşturuldu
- [ ] Animation isimleri doğru
- [ ] Timing'ler ayarlandı
- [ ] Branching bağlantıları yapıldı
- [ ] Hasar değerleri balanced

### Component Setup
- [ ] ProComboSystem eklendi
- [ ] InputBuffer eklendi
- [ ] Default opener'lar ayarlandı
- [ ] Hitbox entegrasyonu yapıldı

### Animator
- [ ] Tüm state'ler oluşturuldu
- [ ] İsimler eşleşiyor
- [ ] Idle transition var

### Test
- [ ] Basic combo çalışıyor
- [ ] Branching çalışıyor
- [ ] Input buffer çalışıyor
- [ ] Cancel window çalışıyor
- [ ] Hit stop çalışıyor

---

## 🎉 Sonuç

**PRO-LEVEL COMBO SYSTEM** artık hazır!

- ✅ Data-driven design
- ✅ Branching combos
- ✅ Input buffering
- ✅ Cancel windows
- ✅ Hit stop
- ✅ Kod yazmadan kombo oluşturma!

**Artık Street Fighter kalitesinde kombolar yapabilirsiniz!** 🥊

---

**Happy Combo Making!** 🎮⚔️

