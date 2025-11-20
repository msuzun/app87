# 🚀 Neon Syndicate: Retribution - Setup Guide

Bu kılavuz, projeyi sıfırdan kurmak için adım adım talimatlar içerir.

---

## 📋 Ön Gereksinimler

### Yazılım Gereksinimleri
- **Unity Hub** (En son versiyon)
- **Unity 2021.3 LTS** veya daha yenisi
- **Visual Studio 2022** veya **Rider** (C# IDE)
- **Git** (Versiyon kontrolü için)

### Donanım Önerileri
- **CPU**: Intel i5 veya AMD Ryzen 5+
- **RAM**: 8GB minimum, 16GB önerilen
- **GPU**: DirectX 11 uyumlu (Intel HD 4000+)
- **Disk**: 5GB boş alan

---

## 🎯 Adım 1: Unity Projesi Oluşturma

### 1.1 Unity Hub'ı Aç

```
Unity Hub > Projects > New Project
```

### 1.2 Template Seçimi

```
Template: 2D (URP) - Universal Render Pipeline
Project Name: NeonSyndicateRetribution
Location: [İstediğiniz klasör]
```

### 1.3 Proje Ayarları

```
Unity Version: 2021.3 LTS
```

**Create** butonuna tıklayın ve Unity'nin açılmasını bekleyin.

---

## 📦 Adım 2: Gerekli Paketleri Yükleme

### 2.1 Package Manager'ı Açın

```
Window > Package Manager
```

### 2.2 Şu Paketleri Yükleyin:

#### **Input System**
```
Package Manager > Unity Registry
Arama: "Input System"
Versiyon: 1.5.0 veya üzeri
Install
```

#### **2D Animation**
```
Arama: "2D Animation"
Versiyon: 9.0.0 veya üzeri
Install
```

#### **TextMeshPro**
```
Arama: "TextMesh Pro"
Install
TMP Importer açılırsa: "Import TMP Essentials"
```

### 2.3 Input System Aktifleştirme

```
Edit > Project Settings > Player
Active Input Handling: Input System Package (New)
```

**Uyarı**: Unity yeniden başlatma isteyecek. "Yes" deyin.

---

## 📂 Adım 3: Proje Dosyalarını İçe Aktarma

### 3.1 Scripts Klasörünü Kopyalama

```bash
# Terminal'de (veya Windows Explorer ile manuel)
# Bu repo'nun Assets/_Game/Scripts klasörünü kopyalayın
cp -r [kaynak]/Assets/_Game/Scripts [proje]/Assets/_Game/Scripts
```

### 3.2 Klasör Yapısını Oluşturma

Unity Project penceresinde:

```
Assets/ klasörüne sağ tıkla > Create > Folder > "_Game"
_Game klasörü içinde sırayla oluştur:
  - Scripts (zaten kopyaladık)
  - Prefabs
  - Animations
  - Art
  - Data
  - Audio
  - VFX
  - Scenes
```

---

## 🎮 Adım 4: Input Ayarları

### 4.1 Input Actions Asset Oluşturma

```
Assets klasörüne sağ tıkla
Create > Input Actions
İsim: "PlayerInputActions"
```

### 4.2 Input Haritasını Yapılandırma

**PlayerInputActions** dosyasına çift tıkla.

#### **Action Map: Player**

```
Movement (Value, Vector2)
  ├── WASD (Keyboard)
  │     W: Up
  │     S: Down
  │     A: Left
  │     D: Right
  └── Arrow Keys (Keyboard)

Attack (Button)
  ├── J (Keyboard)
  └── Left Mouse (Mouse)

HeavyAttack (Button)
  ├── K (Keyboard)
  └── Right Mouse (Mouse)

Jump (Button)
  └── Space (Keyboard)

Dodge (Button)
  └── Left Shift (Keyboard)

Grab (Button)
  └── E (Keyboard)
```

**Save Asset** butonuna tıklayın.

---

## 🏗️ Adım 5: İlk Sahneyi Kurma

### 5.1 Yeni Sahne Oluşturma

```
File > New Scene
Şablon: Basic 2D (URP)
File > Save As
İsim: "TestScene"
Kaydet: Assets/_Game/Scenes/
```

### 5.2 GameManager Kurulumu

#### Boş GameObject Oluştur
```
Hierarchy > Sağ tıkla > Create Empty
İsim: "GameManager"
Transform Position: (0, 0, 0)
```

#### Component'leri Ekle
```
Inspector > Add Component
  - GameManager.cs
  - InputHandler.cs
  - ObjectPooler.cs
  - SoundManager.cs
  - AITokenManager.cs
```

#### Audio Source'ları Ayarla (SoundManager için)
```
Add Component > Audio Source (2 adet)
  1. Music Source (Loop: true)
  2. SFX Source (Loop: false)
```

SoundManager Inspector'ında:
```
Music Source: [Drag MusicSource]
SFX Source: [Drag SFXSource]
```

### 5.3 Kamera Ayarları

**Main Camera** seçin:

```
Inspector:
  Camera > Projection: Orthographic
  Size: 5
  Background: Solid Color (Siyah veya koyu gri)

Add Component:
  - CameraFollow.cs
```

---

## 🎨 Adım 6: Player Prefab Oluşturma

### 6.1 Player GameObject

```
Hierarchy > Sağ tıkla > Create Empty
İsim: "Player"
Tag: "Player" (Inspector'da ayarla)
Layer: Default
Position: (0, 0, 0)
```

### 6.2 Player Components

```
Add Component:
  - Rigidbody2D
      Body Type: Dynamic
      Gravity Scale: 0 (2.5D için)
      Linear Drag: 5
      Freeze Rotation: Z
  
  - Box Collider 2D
      Size: (0.8, 1.8)
  
  - Animator
      (Şimdilik boş bırak)
  
  - Sprite Renderer
      Sprite: [Placeholder: Unity's Default Sprite]
      Color: Blue (Test için)
  
  - SortingOrderController.cs
      Auto Update: true
      Sorting Layer Name: "Characters"
  
  - PlayerController.cs
  - PlayerStateMachine.cs
  - PlayerCombat.cs
  - ComboSystem.cs
```

### 6.3 Hitbox'ları Ekle

#### Punch Hitbox
```
Player > Sağ tıkla > Create Empty
İsim: "PunchHitbox"
Position: (0.6, 0.5, 0) (Sağ el pozisyonu)

Add Component:
  - Circle Collider 2D
      Is Trigger: true
      Radius: 0.3
      Enabled: false (başlangıçta)
  
  - Hitbox.cs
      Damage: 10
      Knockback Force: 5
      Target Layers: Enemy
```

#### Kick Hitbox
```
Player > Sağ tıkla > Create Empty
İsim: "KickHitbox"
Position: (0.7, -0.3, 0) (Ayak pozisyonu)

Add Component:
  - Circle Collider 2D
      Is Trigger: true
      Radius: 0.4
      Enabled: false
  
  - Hitbox.cs
      Damage: 15
      Knockback Force: 8
```

### 6.4 Player Prefab Kaydetme

```
Player GameObject'i seç
Drag & Drop: Assets/_Game/Prefabs/ klasörüne
```

---

## 👾 Adım 7: Enemy Prefab Oluşturma

### 7.1 Enemy GameObject

```
Hierarchy > Create Empty
İsim: "Enemy_Thug"
Tag: "Enemy"
Layer: Enemy (Yeni layer oluştur)
Position: (3, 0, 0)
```

### 7.2 Enemy Components

```
Add Component:
  - Rigidbody2D (Player ile aynı ayarlar)
  - Box Collider 2D (Size: 0.8, 1.8)
  - Animator
  - Sprite Renderer (Color: Red - Test)
  - SortingOrderController.cs
  - EnemyController.cs
      Enemy Type: Thug
  - EnemyAI.cs
      Detection Range: 8
      Attack Range: 1.5
```

### 7.3 Enemy Hitbox

```
Enemy_Thug > Create Empty
İsim: "AttackHitbox"
Position: (0.5, 0, 0)

Add Component:
  - Circle Collider 2D (Trigger: true, Radius: 0.3)
  - Hitbox.cs
      Target Layers: Player
```

### 7.4 Prefab Kaydet

```
Enemy_Thug > Drag to Assets/_Game/Prefabs/
```

---

## 📊 Adım 8: ScriptableObject Verileri Oluşturma

### 8.1 Player Stats

```
Assets/_Game/Data/ klasörüne sağ tıkla
Create > Neon Syndicate > Player Stats
İsim: "Axel_Stats"

Inspector:
  Max Health: 100
  Move Speed: 5
  Attack Damage: 10
  Max Stamina: 100
  Stamina Regen Rate: 10
```

### 8.2 Enemy Stats

```
Create > Neon Syndicate > Enemy Stats
İsim: "Thug_Stats"

Inspector:
  Enemy Name: "Street Thug"
  Max Health: 50
  Move Speed: 3
  Attack Damage: 10
  Detection Range: 8
```

### 8.3 Combo Data

```
Create > Neon Syndicate > Combo Data
İsim: "Axel_BasicCombo"

Inspector:
  Combo Name: "Basic Combo"
  
  Combo Steps (4 adet ekle):
    Step 1:
      Animation Trigger: "Attack1"
      Damage: 10
      Cancel Window: 0.5
    
    Step 2:
      Animation Trigger: "Attack2"
      Damage: 12
      Cancel Window: 0.5
    
    Step 3:
      Animation Trigger: "Attack3"
      Damage: 15
      Cancel Window: 0.4
    
    Step 4:
      Animation Trigger: "Attack4"
      Damage: 20
      Is Launcher: true
      Cancel Window: 0.2
```

### 8.4 ScriptableObject'leri Bağlama

**Player Prefab'ı aç**:
```
PlayerCombat > Combo System
  Combo Data: [Drag Axel_BasicCombo]
```

**Enemy Prefab'ı aç**:
```
EnemyController
  (Şu an için ScriptableObject bağlantısı opsiyonel)
```

---

## 🎭 Adım 9: Basit Animasyon Kurulumu

### 9.1 Animator Controller Oluşturma

```
Assets/_Game/Animations/Controllers/ klasörüne sağ tıkla
Create > Animator Controller
İsim: "Player_Animator"
```

### 9.2 Animator Controller'ı Yapılandırma

**Player_Animator** dosyasına çift tıkla (Animator penceresi açılır).

#### Parameters Ekle:
```
IsWalking (Bool)
IsAttacking (Bool)
IsDead (Bool)
Hit (Trigger)
Attack1, Attack2, Attack3, Attack4 (Trigger)
Jump (Trigger)
Dodge (Trigger)
```

#### States Oluştur:
```
Idle (Default State - Turuncu)
Walk
Attack1
Attack2
Attack3
Attack4
Jump
Dodge
Hurt
Death
```

#### Transitions (Basit versiyon):
```
Idle → Walk (IsWalking = true)
Walk → Idle (IsWalking = false)
Any State → Hurt (Hit trigger)
Any State → Death (IsDead = true)
```

### 9.3 Placeholder Animasyonlar

**Not**: Gerçek animasyonlar yoksa boş animasyon klipleri oluştur:

```
Assets/_Game/Animations/Clips/ klasörüne sağ tıkla
Create > Animation
İsim: "Player_Idle"

(Diğer state'ler için tekrarla)
```

Her state'e ilgili animation clip'ini ata.

### 9.4 Animator'ı Player'a Bağla

```
Player Prefab > Animator Component
  Controller: [Drag Player_Animator]
```

---

## 🧪 Adım 10: İlk Test

### 10.1 Sahne Düzenleme

```
Hierarchy:
  - GameManager (✓)
  - Main Camera (✓)
  - Player (0, 0, 0)
  - Enemy_Thug (3, 0, 0)
  - Ground (Opsiyonel görsel için)
```

### 10.2 Layers ve Collision Matrix

```
Edit > Project Settings > Tags and Layers
Layers:
  8: Player
  9: Enemy
  10: Hitbox
```

```
Edit > Project Settings > Physics 2D
Layer Collision Matrix:
  Player ↔ Enemy: ✓
  Hitbox (Player) ↔ Enemy: ✓
  Hitbox (Enemy) ↔ Player: ✓
```

### 10.3 Play Moduna Gir

```
Unity Editor > Play butonu (Ctrl + P)
```

**Beklenen Sonuçlar**:
- ✅ WASD ile hareket edilebilmeli
- ✅ Karakter düşmanın önüne/arkasına geçebilmeli (sorting çalışıyor)
- ✅ J tuşu ile saldırı animasyonu tetiklenmeli
- ✅ Düşman oyuncuyu kovalamaya başlamalı

---

## 🐛 Yaygın Sorunlar ve Çözümleri

### Problem 1: Input Çalışmıyor

**Çözüm**:
```
Edit > Project Settings > Player
Active Input Handling: "Both" veya "Input System Package (New)" olmalı
Unity'yi yeniden başlat
```

### Problem 2: Karakterler Birbirinden Geçiyor

**Çözüm**:
```
Player ve Enemy'nin Rigidbody2D > Collision Detection:
  "Continuous" yap
```

### Problem 3: Animasyon Çalışmıyor

**Çözüm**:
```
Animator Controller'da transition'lara bak
"Has Exit Time" seçeneğini kapat (responsive olması için)
Transition Duration: 0.1 saniye yap
```

### Problem 4: State Machine Hataları

**Çözüm**:
```
Console'u kontrol et
PlayerStateMachine > Awake() metodu çağrılıyor mu?
State instance'ları null değilse devam et
```

---

## 🎨 Adım 11: Görsel İyileştirmeler (Opsiyonel)

### 11.1 2D Lights (URP)

```
Hierarchy > Sağ tıkla > Light > 2D > Global Light 2D
Intensity: 1
Color: Slight blue tint (Cyber-noir için)
```

### 11.2 Post-Processing

```
Main Camera > Add Component > Volume
Profile: New

Add Override:
  - Bloom (Neon efekti için)
  - Color Adjustments (Saturation hafif düşür)
  - Vignette (Kenarları koyulaştır)
```

### 11.3 Particle Systems (Kan Efekti)

```
Hierarchy > Right Click > Effects > Particle System
İsim: "BloodSplatter"

Inspector:
  Duration: 0.5
  Start Lifetime: 0.3
  Start Speed: 3
  Start Size: 0.1
  Start Color: Red
  Emission > Rate over Time: 20
  Shape > Cone

Prefab yap > Assets/_Game/Prefabs/VFX/
```

ObjectPooler'a ekle:
```
GameManager > ObjectPooler
Pools:
  Tag: "BloodSplatter"
  Prefab: [Drag BloodSplatter]
  Size: 20
```

---

## 📖 Adım 12: Dokümantasyonu Okuma

Kurulum tamamlandı! Şimdi:

1. **README.md** - Genel oyun bilgisi
2. **ARCHITECTURE.md** - Teknik detaylar
3. **Bu dosya** - Setup kılavuzu

---

## ✅ Son Kontrol Listesi

- [ ] Unity 2021.3 LTS kurulu
- [ ] Gerekli paketler yüklü (Input System, 2D Animation, TMP)
- [ ] Klasör yapısı oluşturuldu
- [ ] GameManager sahnede ve script'ler bağlı
- [ ] Player prefab çalışıyor
- [ ] Enemy prefab çalışıyor
- [ ] Input sistemi aktif
- [ ] Animasyon controller bağlı
- [ ] ScriptableObject'ler oluşturuldu
- [ ] Ilk test başarılı

---

## 🎓 Sonraki Adımlar

1. **Animasyonları Ekle**: Gerçek sprite sheet'ler veya skeletal rig
2. **Ses Ekle**: Müzik ve SFX dosyaları
3. **Level Tasarımı**: Arka plan, platformlar, prop'lar
4. **Boss Karakteri**: İlk boss AI'sını kur
5. **UI/HUD**: Can barı, combo sayacı, stil derecesi

---

## 🚀 Başarılar!

Artık **Neon Syndicate: Retribution** geliştirmeye hazırsınız!

Sorularınız için issue açabilir veya community'ye sorabilirsiniz.

**Happy Coding!** 🎮

