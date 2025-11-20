# 🎮 NEON SYNDICATE: RETRIBUTION

**Crazy Flasher Inspired 2D Beat 'em up**

Siber-noir atmosferde, Crazy Flasher serisinin akıcı dövüş hissiyle modern oyun tasarımı prensiplerini birleştiren 2D side-scrolling beat 'em up oyunu.

---

## 📋 İçindekiler

- [Oyun Hakkında](#oyun-hakkında)
- [Özellikler](#özellikler)
- [🔥 Hibrit Sistem](#-hibrit-sistem)
- [Teknik Mimari](#teknik-mimari)
- [Kurulum](#kurulum)
- [Kullanım](#kullanım)
- [Klasör Yapısı](#klasör-yapısı)
- [Geliştirme Notları](#geliştirme-notları)

---

## 🎯 Oyun Hakkında

**Neon Syndicate: Retribution**, Axel adlı bir eski suikastçının intikam hikayesini anlatır. The Syndicate adlı suç örgütü tarafından ihanete uğrayan Axel, sibernetik bir kolla donatılmış halde örgütün dört liderini indirmek için sokakların karanlık yolculuğuna çıkar.

### Hikaye
- **Tema**: Cyber-Noir, Distopik Sokaklar, Yeraltı Dünyası
- **Başkahraman**: Axel - Sibernetik kollu eski tetikçi
- **Düşman**: The Syndicate - 4 lider, onlarca mafya üyesi

### Görsel Stil
- Karanlık, yağmurlu neon-lit ortamlar
- Detaylı 2D karakterler (Skeletal Animation)
- Kan efektleri ve ragdoll fizik
- 2.5D derinlik hissi (Y-axis sorting)

---

## ✨ Özellikler

### 🥊 Dövüş Sistemi (PRO-LEVEL!)
- **🔥 ProComboSystem**: Data-driven, branching combos, cancel windows
- **Input Buffering**: 0.2s buffer (lag hissi yok!)
- **Cancel Windows**: Timing-based combo sistemi (skill gerektiren)
- **Hit Stop Effect**: Vuruş anında milisaniyelik duraklama (tatmin edici!)
- **Combo Branching**: Z→Z→Z veya Z→Z→X farklı sonuçlar (kod yazmadan!)
- **ScriptableObject**: Tüm kombolar asset olarak (designer-friendly)
- **Stil Puanı**: D'den SSS'ye kadar 7 seviye derecelendirme
- **Rage Meter**: Dolduğunda "Execution Move" yapılabilir

### 🎮 Karakter Mekanikleri
- **Z-Axis Movement**: Derinlikli 2.5D hareket
- **Run/Sprint**: Shift ile koşma (stamina tüketir)
- **Fake Height Jump**: Coroutine bazlı parabolic jump (shadow yerde kalır)
- **Dodge/Dash**: I-frame (invulnerability) ile kaçınma + stamina cost
- **Air Control**: Havada hareket ve saldırı yapabilme
- **Grab System**: Düşmanları yakalayıp fırlatma
- **Weapon Pickup**: Sopa, bıçak gibi silahları kullanma
- **Stamina Barı**: Sprint ve dodge için dayanıklılık sistemi

### 🤖 Düşman AI (3 Unique Types!)
- **Basic Brawler**: Kalabalık halinde tehlikeli, basit kombolar, patrol davranışı
- **Fast Dodger**: Hit-and-run taktikleri, %40 dodge şansı, kite yapan sinir bozucu düşman
- **Heavy Tank**: Boss-like mini-tank, super armor, charge attacks, berserker mode
- **Token System**: Aynı anda en fazla 2 düşman saldırır (adil oyun)
- **Behavior Trees**: Her AI'nın kendi decision making sistemi
- **Attack Patterns**: 10+ farklı saldırı pattern'i (Coroutine-based)
- **Boss Mekanikleri**: 3 fazlı boss savaşları (planlı)

### 📊 Progression Sistemi
- Para kazanma ve harcama (Black Market)
- Stat yükseltmeleri (HP, STR, AGI)
- Yeni kombo unlockları
- Silah uzmanlığı

---

## 🔥 Hibrit Sistem

**Best of Both Worlds!** Bu proje, **Class-Based FSM** ve **Coroutine-Based Actions** yaklaşımlarını birleştirir:

### Class-Based FSM (State Management)
```csharp
✅ Modüler state sınıfları (7 ayrı dosya)
✅ Temiz state geçişleri
✅ SOLID prensipleri
✅ Test edilebilir kod
```

### Coroutine-Based Actions (Timing Operations)
```csharp
✅ Akıcı jump/dash mekanikleri
✅ Fake height jump (2.5D)
✅ Doğal timing ve lerp işlemleri
✅ Interrupt edilebilir aksiyonlar
```

**Detaylı Bilgi**: [HYBRID_SYSTEM.md](HYBRID_SYSTEM.md) dosyasını okuyun! 📖

---

## 🏗️ Teknik Mimari

### Kullanılan Teknolojiler
- **Engine**: Unity 2D (2021.3+)
- **Language**: C#
- **Architecture**: Hybrid System (FSM + Coroutines)
- **Design Patterns**: 
  - Finite State Machine (Class-Based)
  - Coroutine-Based Actions
  - Object Pooling
  - Singleton (Managers için)
  - ScriptableObject (Data)

### Core Sistemler

#### 1. **State Machine**
```
StateBase (Abstract)
  ├── PlayerIdleState
  ├── PlayerWalkState
  ├── PlayerAttackState
  ├── PlayerJumpState
  ├── PlayerDodgeState
  ├── PlayerHurtState
  └── PlayerDeathState
```

#### 2. **Combat System**
- `Hitbox`: Hasar veren bölgeler (animasyon eventleri ile aktif)
- `Hurtbox`: Hasar alan bölgeler
- `ComboSystem`: Zincir saldırı yönetimi
- `IDamageable`: Interface (tüm hasar alabilir objeler)

#### 3. **AI System**
- `EnemyAI`: Davranış kontrolcüsü
- `AITokenManager`: Saldırı sırası yöneticisi
- `EnemyController`: Düşman karakteri base class

#### 4. **Manager Sistemi**
- `GameManager`: Oyun akışı (Pause, Score, Money)
- `InputHandler`: Input System entegrasyonu
- `ObjectPooler`: Performans optimizasyonu
- `SoundManager`: Ses ve müzik yönetimi

---

## 📦 Kurulum

### Gereksinimler
- Unity 2021.3 veya üzeri
- Unity 2D URP (Universal Render Pipeline)
- TextMeshPro paketi

### Adımlar

1. **Unity Projesi Oluştur**
```
Unity Hub > New Project > 2D URP Template
```

2. **Dosyaları İçe Aktar**
```bash
# Bu repo'yu klonla veya dosyaları kopyala
git clone [repo-url]
# Tüm Assets/_Game klasörünü projenin Assets/ içine kopyala
```

3. **Gerekli Paketleri Yükle**
```
Window > Package Manager
  - Input System (2.0+)
  - 2D Animation (9.0+)
  - TextMeshPro
```

4. **Input System Ayarları**
```
Edit > Project Settings > Player
Active Input Handling: Input System Package (New)
```

---

## 🎯 Kullanım

### Oyuncu Kontrolleri

| Tuş | Aksiyon |
|-----|---------|
| **WASD / Arrow Keys** | Hareket (2.5D) |
| **Z / Left Mouse** | Light Attack (Hafif Saldırı) |
| **X / Right Mouse** | Heavy Attack (Ağır Saldırı) |
| **Space** | Jump (Zıplama) |
| **Shift (Hold)** | Run/Sprint |
| **Shift (Tap)** | Dodge/Dash (i-frame) |
| **C** | Grab (Yakalama) |
| **ESC** | Pause |

**Detaylı Kontroller**: [CONTROLS.md](CONTROLS.md) dosyasını okuyun!

### 🔥 Combo Sistemi Örnekleri

**Basic Combo**: Z → Z → Z (3-hit punch combo)  
**Launcher Combo**: Z → Z → X (havaya kaldır)  
**Juggle Combo**: Z → Z → X → Space → Z → Z (havada dövme)  
**Branch Example**: Z → Z → Z (finisher) veya Z → Z → X (launcher)

**Detaylı Combo Rehberi**: [PRO_COMBO_GUIDE.md](PRO_COMBO_GUIDE.md)

### Combo Örnekleri

**Basic Combo**: J → J → J → K (Launcher) → Space → J → K

**Wall Bounce Combo**: K (Heavy) → Duvara çarpma → J → J → J

**Air Juggle**: K (Launcher) → Space → J → J → K (Smash)

---

## 📂 Klasör Yapısı

```
Assets/_Game/
│
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs
│   │   ├── InputHandler.cs
│   │   ├── ObjectPooler.cs
│   │   ├── SoundManager.cs
│   │   └── Interfaces/
│   │       ├── IDamageable.cs
│   │       └── IKnockbackable.cs
│   │
│   ├── Characters/
│   │   └── CharacterBase.cs
│   │
│   ├── StateMachine/
│   │   ├── StateBase.cs
│   │   └── StateMachineController.cs
│   │
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   ├── PlayerCombat.cs
│   │   ├── PlayerStateMachine.cs
│   │   └── PlayerStates/
│   │       ├── PlayerIdleState.cs
│   │       ├── PlayerWalkState.cs
│   │       ├── PlayerAttackState.cs
│   │       ├── PlayerJumpState.cs
│   │       ├── PlayerDodgeState.cs
│   │       ├── PlayerHurtState.cs
│   │       └── PlayerDeathState.cs
│   │
│   ├── Enemy/
│   │   ├── EnemyController.cs
│   │   ├── EnemyAI.cs
│   │   └── AITokenManager.cs
│   │
│   ├── Combat/
│   │   ├── Hitbox.cs
│   │   ├── Hurtbox.cs
│   │   └── ComboSystem.cs
│   │
│   ├── ScriptableObjects/
│   │   ├── ComboData.cs
│   │   ├── EnemyStats.cs
│   │   ├── LevelData.cs
│   │   └── PlayerStats.cs
│   │
│   └── Utils/
│       ├── SortingOrderController.cs
│       ├── RagdollController.cs
│       ├── DamageNumber.cs
│       └── CameraFollow.cs
│
├── Prefabs/          (Karakterler, Düşmanlar, UI)
├── Animations/       (Animator Controllers, Clipler)
├── Art/              (Sprite'lar, Materyaller)
├── Data/             (ScriptableObject instance'ları)
├── Audio/            (Müzik, SFX)
├── VFX/              (Particle Sistemler, Shader'lar)
└── Scenes/           (Level sahneleri)
```

---

## 🛠️ Geliştirme Notları

### Scene Kurulumu

#### 1. **Ana Sahne (Main Scene)**
```
Hierarchy:
  - GameManager (Empty GameObject)
      ├── GameManager.cs
      ├── InputHandler.cs
      ├── ObjectPooler.cs
      ├── SoundManager.cs
      └── AITokenManager.cs
  
  - Main Camera
      └── CameraFollow.cs
  
  - Player
      ├── PlayerController.cs
      ├── PlayerStateMachine.cs
      ├── PlayerCombat.cs
      ├── ComboSystem.cs
      ├── Rigidbody2D
      ├── Animator
      ├── SpriteRenderer
      └── SortingOrderController.cs
  
  - Environment (Zemin, Duvarlar, Props)
  
  - EnemySpawners (Empty GameObjects)
```

### ScriptableObject Oluşturma

1. **Combo Data**
```
Assets klasöründe sağ tık
  > Create > Neon Syndicate > Combo Data
  > İsim: "Axel_BasicCombo"
  > Combo adımlarını ekle
```

2. **Enemy Stats**
```
Create > Neon Syndicate > Enemy Stats
  > İsim: "Thug_Stats"
  > İstatistikleri ayarla
```

3. **Level Data**
```
Create > Neon Syndicate > Level Data
  > İsim: "Level_01_Slums"
  > Dalga yapılarını kur
```

### Animasyon Eventi Kurulumu

Animator'de saldırı animasyonlarına event ekle:

```
Punch animasyonunda:
  Frame 3: ActivatePunchHitbox()
  Frame 6: DeactivateHitboxes()

Kick animasyonunda:
  Frame 4: ActivateKickHitbox()
  Frame 8: DeactivateHitboxes()
```

### Ragdoll Kurulumu

1. Karakteri parçalara ayır (Kafa, Gövde, Kollar, Bacaklar)
2. Her parçaya:
   - `Rigidbody2D` (Kinematic)
   - `Collider2D`
   - `HingeJoint2D` (Eklem noktaları için)
3. Ana GameObject'e `RagdollController.cs` ekle
4. Inspector'da "Auto-Setup Ragdoll Parts" butonuna bas

---

## 🎨 Stil Kılavuzu

### Kod Standartları
- **Naming**: PascalCase (public), camelCase (private)
- **Comments**: XML documentation (public API'ler için)
- **Serialization**: `[SerializeField]` ile private field'ları Inspector'da göster
- **Organization**: Region kullan (#region/#endregion)

### Performans İpuçları

1. **Object Pooling Kullan**
```csharp
// ❌ Kötü
Instantiate(bulletPrefab, position, rotation);

// ✅ İyi
ObjectPooler.Instance.SpawnFromPool("Bullet", position, rotation);
```

2. **String Hash Kullan**
```csharp
// ❌ Kötü
animator.SetBool("IsWalking", true);

// ✅ İyi
static readonly int IsWalking = Animator.StringToHash("IsWalking");
animator.SetBool(IsWalking, true);
```

3. **Component Caching**
```csharp
// ❌ Kötü
GetComponent<Rigidbody2D>().velocity = Vector2.zero;

// ✅ İyi
private Rigidbody2D rb;
void Awake() { rb = GetComponent<Rigidbody2D>(); }
rb.velocity = Vector2.zero;
```

---

## 📝 TODO Listesi

### Temel Sistemler
- [x] State Machine
- [x] Combat System (Hitbox/Hurtbox)
- [x] Player Controller
- [x] Enemy AI & Token System
- [x] ScriptableObject Data Yapısı
- [ ] UI Manager (HUD, Menu)
- [ ] Level Progression System
- [ ] Boss AI

### Efektler
- [ ] Kan Splatter Particle System
- [ ] Hit Spark VFX
- [ ] Screen Shake Efekti
- [ ] Slow-Motion (Execution Move)
- [ ] Motion Blur

### İçerik
- [ ] 4 Level Tasarımı
- [ ] 5 Düşman Tipi Prefab
- [ ] 4 Boss Karakteri
- [ ] 10+ Combo Animasyonu
- [ ] Müzik ve SFX

### Sistemler
- [ ] Save/Load Sistemi
- [ ] Upgrade Shop
- [ ] Achievement System
- [ ] Controller Desteği

---

## 🐛 Bilinen Sorunlar

- Ragdoll bazen duvarlara saplandı kalabiliyor (Collider ayarı gerekli)
- Çok hızlı combo yapılırsa animasyon cancel olabiliyor (Buffer sistemi eklenecek)
- Token sistemi bazen 3 düşmanın saldırmasına izin veriyor (Fix yapılacak)

---

## 📄 Lisans

Bu proje eğitim amaçlıdır. Crazy Flasher serisine saygıyla.

---

## 🙏 Teşekkürler

- **Crazy Flasher Series** - İlham kaynağı
- **Dead Cells & Streets of Rage 4** - Modern beat 'em up referansları
- **Unity Community** - Araçlar ve feedback

---

## 📞 İletişim

Sorular, öneriler veya bug raporları için issue açabilirsiniz.

**Happy Coding! 🎮**

