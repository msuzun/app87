# 📁 Neon Syndicate: Retribution - Project Structure

Bu doküman, projenin detaylı klasör yapısını ve her dosyanın amacını açıklar.

---

## 🗂️ Root Directory

```
NeonSyndicateRetribution/
├── Assets/
│   └── _Game/              # Ana oyun klasörü
├── Packages/               # Unity paket yönetimi
├── ProjectSettings/        # Proje ayarları
├── README.md              # Proje hakkında
├── ARCHITECTURE.md        # Teknik mimari
├── SETUP_GUIDE.md         # Kurulum kılavuzu
├── PROJECT_STRUCTURE.md   # Bu dosya
└── .gitignore             # Git ignore kuralları
```

---

## 📂 Assets/_Game/ Detaylı Yapısı

### 📝 Scripts/

#### **Core/** - Oyunun temel sistemleri
```
Core/
├── GameManager.cs              # Singleton - Oyun akışı, skor, para
├── InputHandler.cs             # Input System entegrasyonu
├── ObjectPooler.cs             # Performans - Object pooling
├── SoundManager.cs             # Ses ve müzik yönetimi
└── Interfaces/
    ├── IDamageable.cs          # Hasar alabilir objeler
    └── IKnockbackable.cs       # Geri savrulan objeler
```

**Kullanım Örneği**:
```csharp
GameManager.Instance.AddScore(100);
InputHandler.Instance.MovementInput;
ObjectPooler.Instance.SpawnFromPool("BloodSplatter", pos, rot);
SoundManager.Instance.PlaySFX("Hit_Impact");
```

---

#### **Characters/** - Ortak karakter sistemi
```
Characters/
└── CharacterBase.cs            # Abstract base class (Player & Enemy)
```

**İçerik**:
- HP, Damage, Speed özellikleri
- IDamageable implementasyonu
- Invulnerability sistemi
- Sprite flip utility

---

#### **StateMachine/** - FSM yapısı
```
StateMachine/
├── StateBase.cs                # Abstract state class
└── StateMachineController.cs   # State geçiş yönetimi
```

**Prensip**: Her state'in Enter/Update/FixedUpdate/Exit metotları var.

---

#### **Player/** - Oyuncu sistemleri
```
Player/
├── PlayerController.cs         # Ana oyuncu kontrolcüsü
├── PlayerCombat.cs            # Saldırı mekanikleri
├── PlayerStateMachine.cs      # State yöneticisi
└── PlayerStates/
    ├── PlayerIdleState.cs     # Durma durumu
    ├── PlayerWalkState.cs     # Hareket durumu
    ├── PlayerAttackState.cs   # Saldırı durumu
    ├── PlayerJumpState.cs     # Zıplama durumu
    ├── PlayerDodgeState.cs    # Kaçınma durumu
    ├── PlayerHurtState.cs     # Hasar alma durumu
    └── PlayerDeathState.cs    # Ölüm durumu
```

**State Akışı**:
```
Idle ↔ Walk → Jump
  ↓       ↓
Attack  Dodge → Idle
  ↓
Hurt → Death
```

---

#### **Enemy/** - Düşman AI sistemleri
```
Enemy/
├── EnemyController.cs          # Düşman base controller
├── EnemyAI.cs                 # Behavior sistemi
└── AITokenManager.cs          # Token sistemi (singleton)
```

**Token Sistemi**: Aynı anda maksimum 2 düşman saldırabilir.

**AI States**:
- Idle: Bekleme
- Chase: Kovalama
- Attack: Saldırı
- Retreat: Geri çekilme (Gunner)
- Stunned: Sersemleme

---

#### **Combat/** - Dövüş mekanikleri
```
Combat/
├── Hitbox.cs                  # Hasar veren bölge
├── Hurtbox.cs                 # Hasar alan bölge
└── ComboSystem.cs             # Combo yönetimi
```

**Combo Sistemi**: ScriptableObject tabanlı, data-driven.

---

#### **ScriptableObjects/** - Veri yapıları
```
ScriptableObjects/
├── ComboData.cs               # Combo bilgileri
├── EnemyStats.cs              # Düşman istatistikleri
├── LevelData.cs               # Level ve dalga bilgileri
└── PlayerStats.cs             # Oyuncu istatistikleri
```

**Avantaj**: Kodlara dokunmadan balance ayarları.

---

#### **Utils/** - Yardımcı araçlar
```
Utils/
├── SortingOrderController.cs  # 2.5D derinlik sıralaması
├── RagdollController.cs       # Fizik bazlı ragdoll
├── DamageNumber.cs            # Floating damage text
└── CameraFollow.cs            # Kamera takip sistemi
```

---

### 🎮 Prefabs/

```
Prefabs/
├── Characters/
│   ├── Player_Axel.prefab     # Ana karakter
│   └── Enemies/
│       ├── Enemy_Thug.prefab
│       ├── Enemy_Biker.prefab
│       ├── Enemy_KnifeJack.prefab
│       ├── Enemy_FatBoy.prefab
│       └── Enemy_Gunner.prefab
│
├── Combat/
│   ├── Hitbox_Punch.prefab
│   ├── Hitbox_Kick.prefab
│   └── Weapon_Bat.prefab
│
├── VFX/
│   ├── BloodSplatter.prefab
│   ├── HitSpark.prefab
│   └── DustWalk.prefab
│
└── UI/
    ├── DamageNumber.prefab
    └── HealthBar.prefab
```

**Prefab Kullanımı**:
```csharp
// Doğrudan instantiate etme
// ObjectPooler kullan
ObjectPooler.Instance.SpawnFromPool("Enemy_Thug", spawnPos, Quaternion.identity);
```

---

### 🎨 Animations/

```
Animations/
├── Controllers/
│   ├── Player_Animator.controller
│   └── Enemy_Thug_Animator.controller
│
└── Clips/
    ├── Player/
    │   ├── Player_Idle.anim
    │   ├── Player_Walk.anim
    │   ├── Player_Punch_L.anim
    │   ├── Player_Punch_R.anim
    │   ├── Player_Kick.anim
    │   ├── Player_Jump.anim
    │   ├── Player_Dodge.anim
    │   └── Player_Death.anim
    │
    └── Enemies/
        ├── Thug_Idle.anim
        ├── Thug_Walk.anim
        └── Thug_Attack.anim
```

**Animator Parameters**:
```
Bool: IsWalking, IsAttacking, IsDead
Trigger: Hit, Attack1, Attack2, Jump, Dodge
```

---

### 🖼️ Art/

```
Art/
├── Sprites/
│   ├── Characters/
│   │   ├── Axel/
│   │   │   ├── Head.png
│   │   │   ├── Body.png
│   │   │   ├── Arm_L.png
│   │   │   └── Arm_R.png
│   │   │
│   │   └── Enemies/
│   │       └── Thug_Spritesheet.png
│   │
│   ├── Environment/
│   │   ├── Level1_Slums/
│   │   │   ├── Ground.png
│   │   │   ├── Building_BG.png
│   │   │   └── Props/
│   │   │       ├── Crate.png
│   │   │       └── TrashCan.png
│   │   │
│   │   └── Level2_Subway/
│   │
│   └── UI/
│       ├── HealthBar_Frame.png
│       ├── RageBar_Fill.png
│       └── Buttons/
│
├── Materials/
│   ├── Lit_Character.mat      # 2D Lit material
│   └── Unlit_Background.mat
│
└── Shaders/
    └── FlashWhite.shader       # Hasar alınca beyaz parlaması
```

**Sprite Import Settings**:
```
Texture Type: Sprite (2D and UI)
Pixels Per Unit: 100
Filter Mode: Point (Pixel Art için)
Compression: None
Max Size: 2048
```

---

### 📊 Data/

```
Data/
├── Player/
│   └── Axel_Stats.asset
│
├── Enemies/
│   ├── Thug_Stats.asset
│   ├── Biker_Stats.asset
│   └── Boss_IronHead_Stats.asset
│
├── Combos/
│   ├── Axel_BasicCombo.asset
│   └── Axel_AdvancedCombo.asset
│
└── Levels/
    ├── Level_01_Slums.asset
    ├── Level_02_NeonMarket.asset
    └── Level_03_Subway.asset
```

**ScriptableObject Oluşturma**:
```
Right Click > Create > Neon Syndicate > [Type]
```

---

### 🔊 Audio/

```
Audio/
├── Music/
│   ├── MainMenu_Theme.mp3
│   ├── Level1_BGM.mp3
│   ├── Boss_Theme.mp3
│   └── Victory_Theme.mp3
│
├── SFX/
│   ├── Combat/
│   │   ├── Punch_Hit_01.wav
│   │   ├── Punch_Hit_02.wav
│   │   ├── Bone_Crack.wav
│   │   └── Whoosh_Attack.wav
│   │
│   ├── Voices/
│   │   ├── Axel_Attack_01.wav
│   │   ├── Axel_Hurt.wav
│   │   └── Enemy_Death.wav
│   │
│   └── UI/
│       ├── Menu_Select.wav
│       └── Button_Click.wav
│
└── Mixers/
    └── MainMixer.mixer
        ├── Master
        ├── Music (80%)
        └── SFX (100%)
```

**Audio Import Settings**:
```
Load Type: 
  - Music: Streaming
  - SFX: Decompress On Load
Sample Rate: 44100 Hz
```

---

### ✨ VFX/

```
VFX/
├── Particles/
│   ├── BloodSplatter.prefab
│   ├── HitSpark.prefab
│   ├── DustWalk.prefab
│   └── ExplosionSmoke.prefab
│
└── Shaders/
    ├── FlashWhite.shader       # Hit feedback
    └── GlowEffect.shader       # Neon objeler
```

**Particle System Settings**:
```
Duration: 0.5-1s (Kısa efektler için)
Looping: false
Prewarm: false
Max Particles: 50
```

---

### 🎮 Scenes/

```
Scenes/
├── MainMenu.unity              # Giriş ekranı
├── Loading.unity               # Yükleme ekranı
├── Levels/
│   ├── Level_01_Slums.unity
│   ├── Level_02_NeonMarket.unity
│   ├── Level_03_Subway.unity
│   └── Level_04_SyndicateTower.unity
│
└── TestScenes/
    ├── Test_Gym.unity          # Whiteboxing test sahnesi
    └── Test_Combat.unity       # Combat sistem testi
```

**Scene Hierarchy Örneği**:
```
Level_01_Slums
├── GameManager (DontDestroyOnLoad)
├── Main Camera
├── Lighting
│   └── Global Light 2D
├── Environment
│   ├── Background
│   ├── MidGround
│   ├── Ground
│   └── Props
├── Player
├── EnemySpawners
│   ├── Wave1_Spawner
│   └── Wave2_Spawner
└── UI
    └── HUD_Canvas
```

---

## 🔍 Dosya İsimlendirme Kuralları

### C# Scripts
```
PascalCase:
  - Classes: PlayerController, EnemyAI
  - Methods: TakeDamage(), ApplyKnockback()
  - Properties: MaxHealth, IsAlive

camelCase:
  - Private fields: currentHealth, moveSpeed
  - Local variables: targetPosition, damage
```

### Assets
```
PascalCase + Underscore:
  - Prefabs: Player_Axel, Enemy_Thug
  - ScriptableObjects: Axel_Stats, Level_01_Data
  - Sprites: Character_Idle, Environment_Ground
```

### Scenes
```
PascalCase + Number:
  - Levels: Level_01_Slums
  - Test: Test_Combat
```

---

## 📈 Dosya Boyutu Kılavuzu

```
Texture (Sprite):
  Character: Max 1024x1024
  Environment: Max 2048x2048
  UI: Max 512x512

Audio:
  Music: MP3, 192kbps, Stereo
  SFX: WAV, 44.1kHz, Mono

Animations:
  Clip Length: 0.2-1.0 saniye
  Framerate: 24 FPS
```

---

## 🔗 Dependencies (Asset Bağımlılıkları)

```
Player Prefab Dependencies:
  ├── PlayerController.cs
  │   └── PlayerStats.asset
  ├── PlayerCombat.cs
  │   ├── Axel_BasicCombo.asset
  │   └── Hitbox Prefabs
  └── PlayerStateMachine.cs
      └── State Scripts (7 adet)

Enemy Prefab Dependencies:
  ├── EnemyController.cs
  │   └── EnemyStats.asset
  └── EnemyAI.cs
      └── AITokenManager (Scene'de olmalı)
```

---

## 🚀 Build Çıktısı

```
Builds/
├── Windows/
│   ├── NeonSyndicate.exe
│   └── NeonSyndicate_Data/
├── Android/
│   └── NeonSyndicate.apk
└── WebGL/
    └── index.html
```

**Build Settings**:
```
Platform: PC, Android, WebGL
Architecture: x86_64 (PC), ARM64 (Android)
Compression: LZ4 (Hızlı yükleme)
```

---

## 📝 Notlar

### Performance Tips
1. **Texture Atlasing**: Sprite'ları atlas'la (Unity Sprite Packer)
2. **Audio Compression**: Music için MP3, SFX için WAV
3. **Object Pooling**: Tüm frequently spawned objeler için

### Organization Tips
1. Her sistemi kendi namespace'inde tut
2. #region kullan (uzun scriptlerde)
3. XML comments ekle (public API'ler için)

---

## ✅ Quick Reference

### Yeni Karakter Ekleme
```
1. Prefabs/Characters/ altında prefab oluştur
2. Data/ altında Stats ScriptableObject oluştur
3. Animations/Controllers/ altında Animator Controller oluştur
4. CharacterBase'den türet ve ilgili script'leri ekle
```

### Yeni Level Ekleme
```
1. Scenes/Levels/ altında yeni sahne oluştur
2. Data/Levels/ altında LevelData ScriptableObject oluştur
3. Build Settings'e sahneyi ekle
4. GameManager'da level transition logic'i güncelle
```

### Yeni Combo Ekleme
```
1. Data/Combos/ altında ComboData oluştur
2. Combo adımlarını tanımla
3. Animator'da ilgili trigger'ları ekle
4. Animasyon kliplerini oluştur
5. PlayerCombat'a yeni combo reference'ı ekle
```

---

Bu yapı, projenin modüler, ölçeklenebilir ve bakımı kolay olmasını sağlar! 🎮

