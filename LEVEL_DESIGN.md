# 🗺️ LEVEL DESIGN SYSTEM - Complete Guide

**Crazy Flasher tarzı "İlerle → Dur → Savaş → İlerle" mekanizması**

Data-driven, designer-friendly, profesyonel level sistemi!

---

## 📋 İçindekiler

- [Sistem Özellikleri](#-sistem-özellikleri)
- [4 Level Tasarımları](#-4-level-tasarımları)
- [Kurulum](#-kurulum)
- [Level Config Oluşturma](#-level-config-oluşturma)
- [Wave Tasarımı](#-wave-tasarımı)
- [Kullanım Örnekleri](#-kullanım-örnekleri)

---

## ✨ Sistem Özellikleri

### 1. **Data-Driven Design** 📊
```
❌ Kod yazmaya GEREK YOK!
✅ ScriptableObject ile level tasarımı
✅ Unity Editor'de drag-and-drop
✅ JSON export/import desteği
```

### 2. **Wave System** 🌊
```
✅ Trigger-based spawn (oyuncu X'e gelince)
✅ Kamera kilidi (arena battle)
✅ Delay ile spawn (dramatic effect)
✅ Ödül sistemi (wave clear bonus)
```

### 3. **Camera Management** 📷
```
✅ Auto follow (normal movement)
✅ Lock (wave battle)
✅ Smooth transition (lerp)
✅ Invisible walls (escape önleme)
```

### 4. **Environment** 🏙️
```
✅ Destructible objects (kasalar, variller)
✅ Parallax backgrounds (3 layer)
✅ Auto-scroll (metro sahnesi)
✅ Interactive props
```

---

## 🗺️ 4 LEVEL TASARIMLARI

### LEVEL 1: THE SLUMS (Gecekondu)

#### Atmosfer
```
Tema: Çürüme, kirli sokaklar
Zaman: Akşamüstü
Renk Paleti: Turuncu/Gri
Zemin: Çatlak asfalt
Özel: Graffiti duvarlar
```

#### Parallax Layers
```
Background (x0.1):  Silüet gökdelenler
Midground (x0.5):   Harabe binalar
Foreground (x1.2):  Tel örgüler (blur)
```

#### Wave Structure
```
Wave 1 "Entrance" (X: 15):
  - 2x Thug (Isınma)

Wave 2 "Alley Ambush" (X: 45):
  - 1x Brawler
  - 2x Thug (çöp kutularının arkasından)

Wave 3 "Subway Entrance" (X: 120):
  - 1x Heavy Tank (Mini-boss!)
  - 2x Knife Jack (Tank ile birlikte)
```

#### Destructibles
```
- Ahşap kasalar (Health pickup)
- Çöp poşetleri (Money)
- Yanan variller (Tehlike! Hasar verir)
```

---

### LEVEL 2: NEON MARKET (Çin Mahallesi)

#### Atmosfer
```
Tema: Cyberpunk, canlı renkler
Zaman: Gece, yağmurlu
Renk Paleti: Neon pembe/mavi/yeşil
Zemin: Islak parke (yansımalar!)
Özel: Hologram reklamlar
```

#### Parallax Layers
```
Background (x0.1):  Devasa hologram panolar
Midground (x0.5):   Dükkan vitrinleri, fenerler
Foreground (x1.3):  Yağmur damlacıkları
```

#### Wave Structure
```
Wave 1 "Market Entry" (X: 20):
  - 3x Knife Jack (Hızlı, sinir bozucu!)

Wave 2 "Food Court" (X: 60):
  - 2x Brawler
  - 2x Gunner (Tezgahların arkasından ateş!)

Wave 3 "Neon Alley" (X: 100):
  - 4x Mixed (Brawler + Knife Jack)

Sürpriz:
  - Motosikletli düşmanlar (ekrandan geçiyor, ezilme riski!)
```

#### Destructibles
```
- Balık tezgahları (Weapon: Balık!)
- Neon tabelalar (Parlama efekti)
- Fenerler (Ateş efekti)
```

---

### LEVEL 3: SUBWAY CHASE (Metro)

#### Atmosfer
```
Tema: Hız, klostrofobi
Zaman: Gece
Renk Paleti: Turuncu/Sarı (tünel ışıkları)
Zemin: Metal vagon zemini
Özel: HAREKET HALİNDE!
```

#### Parallax Layers (Özel!)
```
Background (Auto-scroll):  Tünel duvarları (LOOP!)
Midground (Auto-scroll):   Kablolar, borular
Foreground (Sway):         Tutacaklar (sallanıyor)
```

#### Wave Structure
```
Wave 1 "First Wagon" (X: 10):
  - 2x Thug (Kapıdan giriş)

Wave 2 "Inter-Wagon" (X: 30):
  - Camları kırarak giriş
  - 3x Knife Jack

Wave 3 "Last Wagon" (X: 50):
  - Tünel ışıkları yanıp söndüğünde
  - 2x Heavy Tank (Karanlıkta spawn!)

Mekanik:
  - Arka plan sürekli sola akıyor (illüzyon)
  - Oyuncu gerçekte ilerliyor ama tren hareketi hissi var
```

#### Hazards (Tehlikeler)
```
- Tünel ışıkları (eğilmek gerekir)
- Elektrik kabloları (hasar)
- Açık kapılar (düşme riski)
```

---

### LEVEL 4: SYNDICATE TOWER (Gökdelen)

#### Atmosfer
```
Tema: Lüks, soğuk teknoloji
Zaman: Gece
Renk Paleti: Mavi/Beyaz (steril)
Zemin: Cam/metal (parlak)
Özel: Yükseklik korkusu, rüzgar!
```

#### Parallax Layers
```
Background (x0.05): Bulutlar, dolunay
Midground (x0.3):   Helikopter pisti işaretleri
Foreground (x1.5):  Uçuşan kağıtlar, rüzgar
```

#### Wave Structure
```
Wave 1 "Rooftop Guards" (X: 15):
  - 4x Gunner (Uzaktan ateş!)

Wave 2 "Executive Protection" (X: 50):
  - 2x Tank
  - 3x Brawler

Wave 3 "Final Showdown" (X: 100):
  - 6x Mixed (Herşey bir arada!)
```

#### Special Mechanic: Ring Out
```
Ekranın sağı ve solunda korkuluk yok!
Düşmanları aşağı atabilirsin = INSTA-KILL!
Risk: Sen de düşebilirsin!
```

#### Boss Fight
```
Boss: "The Kingpin"
Position: Helipad merkezi
Phases: 3 faz
Mechanic: Phase 2'de helikopter ateş ediyor!
```

---

## 🛠️ Kurulum

### Adım 1: Level Config Asset Oluştur

```
Project penceresinde sağ tıkla
Create > Neon Syndicate > Level > Level Config

İsim: "Level_01_Slums_Config"
```

---

### Adım 2: Inspector Ayarları

#### General Settings
```
Level Name: "The Slums"
Level Index: 1
Description: "Karanlık sokaklar, suçun başlangıcı..."
```

#### Audio
```
Background Music: [Drag Music Asset]
Ambient Sound: [Drag Ambient Asset] (opsiyonel)
```

#### Environment
```
Environment Prefab: [Drag Level_01_Environment]
Parallax Prefab: [Drag Level_01_Parallax]
```

#### Boundaries
```
Level Start X: 0
Level End X: 150
Min Y: -3.5
Max Y: -1.0
```

---

### Adım 3: Wave Ekleme

#### Wave 1:
```
Click "+" on Waves list

Wave Name: "Entrance Ambush"
Trigger Pos X: 15.0
Lock Camera: ☑ true
Camera Lock X: 15.0

Enemies (2 adet):
  [0]:
    Type: Thug
    Spawn Offset: (10, 0)
    Spawn Delay: 0

  [1]:
    Type: Thug
    Spawn Offset: (-10, 0.5)
    Spawn Delay: 1.5
```

#### Wave 2:
```
Wave Name: "Mid-Street Brawl"
Trigger Pos X: 45.0

Enemies (3 adet):
  [0]:
    Type: Brawler
    Spawn Offset: (12, -1)
    Spawn Delay: 0

  [1]:
    Type: Thug
    Spawn Offset: (12, 1)
    Spawn Delay: 0.5

  [2]:
    Type: Thug
    Spawn Offset: (-12, 0)
    Spawn Delay: 2.0
```

---

### Adım 4: Scene Setup

```
Hierarchy:
  - GameManager
    + LevelManager ← YENİ!
    + WaveSpawner ← YENİ!
    + CameraLockController ← YENİ!
  
  - Main Camera
    + CameraFollow
    + CameraLockController (veya LevelManager'da reference)
  
  - Player
  
  - InvisibleWalls (Empty)
    ├── WallLeft (BoxCollider2D)
    └── WallRight (BoxCollider2D)
```

---

### Adım 5: LevelManager Bağlantıları

```
LevelManager Inspector:
  Current Level Config: [Drag Level_01_Slums_Config]
  Player: [Drag Player]
  Wave Spawner: [Drag WaveSpawner]
  Camera Controller: [Drag CameraLockController]
```

---

### Adım 6: WaveSpawner Bağlantıları

```
WaveSpawner Inspector:
  Enemy Prefabs: (5 adet)
    [0]: Type: Thug,      Prefab: [Enemy_Thug]
    [1]: Type: Brawler,   Prefab: [Enemy_Brawler]
    [2]: Type: KnifeJack, Prefab: [Enemy_FastDodger]
    [3]: Type: Tank,      Prefab: [Enemy_HeavyTank]
    [4]: Type: Boss,      Prefab: [Boss_Prefab]
  
  Use Object Pooling: ☑ true
```

---

## 🎯 Kullanım Örnekleri

### Örnek 1: Basit Level (3 Wave)

```json
{
  "levelName": "Test Level",
  "waves": [
    {
      "waveName": "Wave 1",
      "triggerPosX": 10,
      "enemies": [
        { "type": "Thug", "spawnOffset": [5, 0], "delay": 0 },
        { "type": "Thug", "spawnOffset": [-5, 0], "delay": 1 }
      ]
    },
    {
      "waveName": "Wave 2",
      "triggerPosX": 30,
      "enemies": [
        { "type": "Brawler", "spawnOffset": [8, 0], "delay": 0 },
        { "type": "Brawler", "spawnOffset": [-8, 0], "delay": 0.5 }
      ]
    },
    {
      "waveName": "Wave 3",
      "triggerPosX": 50,
      "enemies": [
        { "type": "Tank", "spawnOffset": [10, 0], "delay": 0 }
      ]
    }
  ]
}
```

---

### Örnek 2: Metro Level (Auto-Scroll)

#### Parallax Setup:
```
Background_TunnelWalls:
  - ParallaxBackground.cs
  - Auto Scroll: ☑ true
  - Scroll Speed: 2.0
  - Use Looping: ☑ true
  - Sprite Width: 20
```

#### Level Config:
```
Level End X: 80 (daha kısa, çünkü scroll var)

Waves:
  - Daha sık (her 15 birimde bir)
  - Daha az düşman (hareketten dolayı zor)
```

---

### Örnek 3: Boss Level

```
Level Config:
  Has Boss: ☑ true
  Boss Prefab: [Boss_IronHead]
  Boss Spawn Position: (150, 0)

Boss Wave:
  - Tüm normal wave'ler bittikten sonra
  - Kamera wide lock
  - Boss health bar gösterilir
```

---

## 🎮 Wave Tasarım Prensipleri

### Pacing (Tempo)

#### Easy Start
```
Wave 1: 2 düşman (tek tip)
  → Oyuncuya ısınma süresi ver
```

#### Gradual Difficulty
```
Wave 2: 3 düşman (mix)
Wave 3: 4 düşman + 1 tank
  → Yavaş yavaş zorluk artar
```

#### Climax
```
Last Wave: Çok sayıda düşman VEYA Tank
  → Dramatic finish
```

### Spawn Patterns

#### Pattern 1: Pincer Attack (Makas)
```
Enemy 1: Spawn Offset (10, 0)  - Sağdan
Enemy 2: Spawn Offset (-10, 0) - Soldan
  → Oyuncuyu ortadan sıkıştırma
```

#### Pattern 2: Reinforcement (Takviye)
```
Enemy 1-2: Delay 0    - İlk dalga
Enemy 3-4: Delay 3s   - Takviye gelir
  → Dinamik zorluk
```

#### Pattern 3: Surprise (Sürpriz)
```
Enemy 1: Type Thug, Delay 0
Enemy 2: Type Tank, Delay 5s (oyuncu rahatladığında!)
  → Momentum shift
```

---

## 🏗️ Scene Hierarchy

### Level 1 Scene Örneği

```
Level_01_Slums
├── GameManager (Persistent)
│   ├── GameManager.cs
│   ├── LevelManager.cs ← YENİ!
│   ├── WaveSpawner.cs ← YENİ!
│   └── ...
│
├── Main Camera
│   ├── CameraFollow.cs
│   └── CameraLockController.cs ← YENİ!
│
├── Player
│
├── Environment
│   ├── Ground
│   ├── Buildings_BG
│   ├── Buildings_MG
│   └── Destructibles
│       ├── Crate_01
│       ├── Crate_02
│       └── Barrel_01
│
├── Parallax
│   ├── Layer_Background (ParallaxBackground.cs, x0.1)
│   ├── Layer_Midground (ParallaxBackground.cs, x0.5)
│   └── Layer_Foreground (ParallaxBackground.cs, x1.2)
│
├── InvisibleWalls
│   ├── WallLeft (deaktif, wave sırasında aktif)
│   └── WallRight (deaktif)
│
└── Lighting
    └── GlobalLight2D
```

---

## 📊 LevelConfigSO Detaylı Örnek

### The Slums - Full Config

```
GENERAL
=======
Level Name: "The Slums"
Level Index: 1
Description: "Karanlık sokaklar, suçun doğduğu yer..."

AUDIO
=====
Background Music: Slums_Theme_DarkSynth.mp3
Ambient Sound: Rain_Light.wav

ENVIRONMENT
===========
Environment Prefab: Level_01_Environment
Parallax Prefab: Level_01_Parallax

BOUNDARIES
==========
Level Start X: 0
Level End X: 150
Min Y: -3.5
Max Y: -1.0

WAVES (3 adet)
==============
Wave 1:
  Name: "Entrance Ambush"
  Trigger X: 15.0
  Lock Camera: true
  Camera Lock X: 15.0
  Enemies:
    [0] Thug, (10, 0), delay 0
    [1] Thug, (-10, 0.5), delay 1.5
  Reward: Health_Pickup

Wave 2:
  Name: "Mid-Street Brawl"
  Trigger X: 45.0
  Lock Camera: true
  Camera Lock X: 45.0
  Enemies:
    [0] Brawler, (12, -1), delay 0
    [1] Thug, (12, 1), delay 0.5
    [2] Thug, (-12, 0), delay 2.0
  Reward: Weapon_Bat

Wave 3:
  Name: "Subway Entrance"
  Trigger X: 120.0
  Lock Camera: true
  Camera Lock X: 120.0
  Enemies:
    [0] Tank, (15, 0), delay 0
    [1] KnifeJack, (-10, 0), delay 5.0
    [2] KnifeJack, (10, 0.5), delay 5.5
  Reward: Money_Bundle

BOSS
====
Has Boss: false
```

---

## 🎨 Environment Prefab Yapısı

### Level_01_Environment Prefab

```
Level_01_Environment
├── Background
│   └── Buildings_Silhouette (Sprite)
│
├── Midground
│   ├── Building_01 (Sprite)
│   ├── Building_02 (Sprite)
│   ├── FireEscape (Sprite)
│   └── ElectricPole (Sprite)
│
├── Ground
│   └── CrackedAsphalt (TilemapRenderer)
│
├── Foreground
│   └── ChainFence (Sprite, Alpha: 0.3)
│
└── Props
    ├── Destructibles
    │   ├── Crate_01 (DestructibleObject.cs)
    │   ├── Crate_02
    │   └── Barrel_01
    │
    └── Static
        ├── Graffiti_01
        └── TrashBags
```

---

## 📐 Parallax Setup

### 3-Layer Parallax

#### Background Layer
```
Prefab: Buildings_Background
Component: ParallaxBackground.cs
  - Parallax Multiplier: 0.1
  - Lock Y: true
  - Use Looping: false
  
Sorting Layer: Background
Order: -100
```

#### Midground Layer
```
Prefab: Buildings_Midground
Component: ParallaxBackground.cs
  - Parallax Multiplier: 0.5
  - Lock Y: true
  - Use Looping: false
  
Sorting Layer: Midground
Order: -50
```

#### Foreground Layer
```
Prefab: Fence_Foreground
Component: ParallaxBackground.cs
  - Parallax Multiplier: 1.2
  - Lock Y: true
  - Use Looping: false
  
Sorting Layer: Foreground
Order: 100
Color: (1, 1, 1, 0.3) ← Alpha için
```

---

## 🚂 Metro Level (Auto-Scroll)

### Setup

```
Tunnel_Background:
  ParallaxBackground.cs
    - Auto Scroll: ☑ true
    - Scroll Speed: 2.0
    - Use Looping: ☑ true ← ÖNEMLİ!
    - Sprite Width: 20 (background genişliği)

Duplicate:
  - Tunnel_Background_01 (X: 0)
  - Tunnel_Background_02 (X: 20)
  
Sonuç: Sonsuz loop!
```

### İllüzyon Yaratma

```
1. Background sola scroll ediyor (auto-scroll)
2. Player sağa ilerliyor (normal hareket)
3. = Tren hareketi illüzyonu!

Bonus:
  - Tutacaklar hafifçe sallanıyor (sine wave)
  - Vagon bağlantı sesleri (SFX)
```

---

## 🎯 Test Etme

### Debug Mode

```
LevelManager:
  Show Debug Info: ☑ true

Ekranda görünecekler:
  Level: The Slums
  Wave: 1 / 3
  Wave Active: true
  Active Enemies: 2
  Player X: 15.2
  Next Trigger: 45.0
```

### Test Senaryoları

#### Test 1: Wave Trigger
```
1. Play mode
2. Player'ı X: 15'e götür
3. ✓ Wave 1 başlamalı
4. ✓ Kamera kilitlenmeli
5. ✓ 2 düşman spawn olmalı
```

#### Test 2: Wave Complete
```
1. Tüm düşmanları öldür
2. ✓ "GO!" text gösterilmeli
3. ✓ Kamera kilidi açılmalı
4. ✓ Reward spawn olmalı
```

#### Test 3: Multiple Waves
```
1. Wave 1 tamamla
2. X: 45'e git
3. ✓ Wave 2 başlamalı
4. Devam et...
```

#### Test 4: Parallax
```
1. Player'ı hareket ettir
2. ✓ Background yavaş hareket etmeli (x0.1)
3. ✓ Midground orta hız (x0.5)
4. ✓ Foreground hızlı (x1.2)
```

---

## 🐛 Troubleshooting

### Problem 1: Wave Başlamıyor
```
Sebep: Trigger position yanlış

Çözüm:
✓ Player X pozisyonu >= triggerPosX mi?
✓ LevelManager aktif mi?
✓ Config assign edilmiş mi?
```

### Problem 2: Düşman Spawn Olmuyor
```
Sebep: Prefab mapping eksik

Çözüm:
✓ WaveSpawner'da Enemy Prefabs listesi dolu mu?
✓ Her enemy type için prefab var mı?
✓ Object Pooler setup'ı yapılmış mı? (kullanıyorsan)
```

### Problem 3: Kamera Kilitlenmiyor
```
Sebep: CameraLockController bağlantısı yok

Çözüm:
✓ LevelManager'da Camera Controller referansı var mı?
✓ Lock Camera checkbox işaretli mi? (wave'de)
✓ Invisible walls oluşturulmuş mu?
```

### Problem 4: Parallax Çalışmıyor
```
Sebep: Main Camera bulunamıyor

Çözüm:
✓ Main Camera tag'i "MainCamera" mi?
✓ ParallaxBackground.cs eklenmiş mi?
✓ Multiplier değerleri doğru mu?
```

---

## 📊 Balancing Guidelines

### Wave Difficulty Curve

```
Wave 1 (Easy):      2-3 düşman, tek tip
Wave 2 (Medium):    3-4 düşman, mix
Wave 3 (Hard):      4-5 düşman + Tank
Wave 4 (Very Hard): 6+ düşman, çok mix
```

### Spawn Delay Önerileri

```
Aynı anda spawn:    Delay 0
Hızlı arka arkaya:  Delay 0.5s
Normal:             Delay 1-2s
Dramatic:           Delay 3-5s (oyuncu rahatladığında!)
```

### Enemy Mix Formulas

```
Easy:   80% Thug, 20% Brawler
Medium: 50% Brawler, 30% KnifeJack, 20% Tank
Hard:   30% Brawler, 40% KnifeJack, 30% Tank
Boss:   Boss + 2 Tank + 4 Mixed
```

---

## 🎨 Visual Design Tips

### Atmosphere

#### The Slums
```
Color Grading:
  - Saturation: -20%
  - Contrast: +10%
  - Color Tint: Orange

Lighting:
  - Global Light: Warm (orange)
  - Intensity: 0.8
  - Street lights: Point lights (yellow)
```

#### Neon Market
```
Color Grading:
  - Saturation: +30% (vibrant!)
  - Bloom: Enabled (neon glow)
  - Color Tint: Cyan/Magenta mix

Lighting:
  - 2D lights (neon signs)
  - Rain particle system
  - Puddle reflections (shader)
```

---

## 🔧 Advanced Features

### Dynamic Difficulty

```csharp
// LevelManager'a ekle
void AdjustDifficultyBasedOnPerformance()
{
    float playerHP = player.CurrentHealth / player.MaxHealth;
    
    if (playerHP < 0.3f) // Player zor durumda
    {
        // Bir sonraki wave'i kolaylaştır
        nextWave.enemies.RemoveAt(nextWave.enemies.Count - 1);
    }
    else if (playerHP > 0.9f) // Player çok iyi
    {
        // Bir düşman daha ekle
        nextWave.enemies.Add(hardEnemy);
    }
}
```

### Random Variations

```csharp
// Wave spawn'da:
if (Random.value < 0.3f)
{
    // %30 şans ile sürpriz düşman
    SpawnBonusEnemy();
}
```

---

## 📋 Checklist

### Level Config
- [ ] ScriptableObject oluşturuldu
- [ ] Boundaries ayarlandı
- [ ] Music assign edildi
- [ ] Environment prefab hazır
- [ ] Wave'ler eklendi
- [ ] Boss ayarları yapıldı (varsa)

### Scene Setup
- [ ] LevelManager var
- [ ] WaveSpawner var
- [ ] CameraLockController var
- [ ] Player var
- [ ] Invisible walls var
- [ ] Environment spawn edildi

### Wave Design
- [ ] Her wave'in trigger'ı doğru
- [ ] Enemy prefab mappings tam
- [ ] Spawn delays balanced
- [ ] Rewards assign edilmiş

### Test
- [ ] Wave trigger çalışıyor
- [ ] Camera lock çalışıyor
- [ ] Enemies spawn oluyor
- [ ] Wave completion çalışıyor
- [ ] Level completion çalışıyor

---

## 💡 Pro Tips

### 1. Whiteboxing (Gri Kutu Tasarım)
```
Önce gri kutularla level'ı test et:
- Düşman = Gri cube
- Destructible = Beyaz cube
- Wall = Kırmızı cube

Gameplay doğruysa → Görsel ekle
```

### 2. Pacing
```
5 saniyelik dövüş → 3 saniyelik dinlenme
= Oyuncu yorulmasın, sıkılmasın
```

### 3. Environmental Storytelling
```
Slums: Çöp, graffiti, harabecilik
Market: Canlı, neon, renkli
Metro: Claustrophobic, hız
Tower: Temiz, soğuk, yüksek
```

### 4. Checkpoint System
```
Her 2 wave'de bir checkpoint
Player öldüyse en son checkpoint'ten başlar
```

---

## 🎊 Sonuç

**LEVEL DESIGN SYSTEM** artık hazır!

### ✅ Features
- Data-driven level design
- Wave system
- Camera lock
- Destructibles
- Parallax backgrounds
- Auto-scroll support
- Boss integration

### 📚 Documentation
- Complete guide
- 4 level designs
- Setup instructions
- Examples
- Troubleshooting

**Artık Crazy Flasher kalitesinde levellar yapabilirsiniz!** 🗺️

---

**Happy Level Designing!** 🎮🗺️⚔️

