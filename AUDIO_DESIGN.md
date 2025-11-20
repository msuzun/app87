# 🔊 AUDIO DESIGN SYSTEM - Complete Guide

**Crazy Flasher tarzı tatmin edici ses tasarımı!**

"Ses, oyunun vuruş hissinin %50'sidir!" - Crazy Flasher Developers

---

## 📋 İçindekiler

- [Ses Tasarım Kılavuzu](#-ses-tasarım-kılavuzu)
- [Teknik Sistem](#-teknik-sistem)
- [Kurulum](#-kurulum)
- [Layered Audio](#-layered-audio)
- [Audio Mixer Setup](#-audio-mixer-setup)
- [Sound Library](#-sound-library)

---

## 🎨 Ses Tasarım Kılavuzu

### Temel Prensip

```
❌ Gerçekçi ses: Boring!
✅ Abartılı ses: Satisfying!

Crazy Flasher tarzı:
- Tok, derin, bas ağırlıklı
- Layered (katmanlı)
- Varyasyonlu (robotik değil)
```

---

## 🥊 SFX Tasarımı (Layering Technique)

### 1. **Punch & Light Attacks** (Hafif Vuruşlar)

#### 3-Layer System:
```
Layer 1 (Air):    Whoosh (yumruk havayı yarıyor)
  ↓
Layer 2 (Body):   Slap/Snap (et sesi)
  ↓
Layer 3 (Detail): Kumaş gıcırtısı (ceket)

Frekans: Mid-High (2kHz-8kHz)
Duration: 0.1-0.2s (kısa ve keskin)
```

#### Örnek Kombinasyon:
```
PlayLayeredSound(
    "Whoosh_Light",    // Hava
    "Impact_Flesh",    // Et
    "Cloth_Rustle"     // Detay
);
```

---

### 2. **Kick & Heavy Attacks** (Ağır Vuruşlar)

#### 4-Layer System:
```
Layer 1 (Whoosh):  Heavy swing (daha derin whoosh)
  ↓
Layer 2 (Impact):  Kick drum / Sub-bass (vücuda çarpma)
  ↓
Layer 3 (Bone):    Kereviz kırma (kemik kırılma)
  ↓
Layer 4 (Detail):  Deri ceket gıcırtısı

Frekans: Low-End (60Hz-500Hz) ← SUB-BASS!
Duration: 0.2-0.4s (daha uzun, echo)
```

#### Örnek:
```
PlayPunchCombo(
    "Whoosh_Heavy",     // Swing
    "Impact_Bone",      // Kemik
    "Impact_SubBass"    // Sub-bass
);
```

**Sonuç**: Subwoofer titrer! 🔊💥

---

### 3. **Hit Spark** (Vuruş Kıvılcımı)

#### Cyberpunk Style:
```
Metalik çınlama + Elektrik cızırtısı

Sound Mix:
  - Metal_Clang.wav (örn: kılıç sesi)
  - Electric_Zap.wav
  - Glass_Tinkle.wav (kritik vuruşlarda)

Frekans: High (4kHz-12kHz)
Effect: Reverb (metalik echo)
```

---

### 4. **Footsteps** (Adım Sesleri)

#### Anti-Machine Gun Effect:
```
Problem: 
  Aynı ses loop → Robot gibi duyulur

Çözüm:
  - 5-6 farklı adım sesi varyasyonu
  - Random pitch (±10%)
  - Random volume (±5%)
```

#### Zemin Tipleri:
```
Concrete (Beton):     Tok, sert
Water Puddle (Su):    Splish-splash, wet
Metal Grate (Izgara): Metalik, echo
```

---

## 🎵 Müzik Tasarımı

### Level Themes

#### Level 1: The Slums
```
Genre: Dark Synthwave
BPM: 90-100 (mid-tempo)
Mood: Gergin, kasvetli
Instruments:
  - Deep bass synth
  - Industrial drums
  - Ambient pads
  - Distant sirens (background)
```

#### Level 2: Neon Market
```
Genre: Cyberpunk Electronica
BPM: 110-120
Mood: Enerji, tehlike
Instruments:
  - Neon synth leads
  - Asian percussion
  - Rain samples
  - Glitch effects
```

#### Level 3: Subway
```
Genre: Industrial Techno
BPM: 130-140 (hızlı!)
Mood: Klostrofobik, tempo
Instruments:
  - Machine sounds
  - Metal percussion
  - Train rhythms
  - Electric buzzing
```

#### Level 4: Tower
```
Genre: Aggressive Phonk / Metal-Step
BPM: 140-150
Mood: Epic, final battle
Instruments:
  - Distorted guitar
  - Heavy drums
  - Orchestral hits
  - Choir (boss'ta)
```

---

### Boss Theme

#### Structure:
```
Intro (0:00-0:15):    Dramatic build-up
Phase 1 (0:15-1:30):  Aggressive, fast
Phase 2 (1:30-2:30):  Boss HP < 50% → Choir enters!
Loop (2:30+):         Main loop

BPM: 140+
Key: Minor (dark, menacing)
```

---

### Rage Mode Audio

#### Effect Chain:
```
Normal Music
  ↓
Low-Pass Filter (500 Hz cutoff) → Boğuk ses
  ↓
Pitch Down (0.8x) → Yavaşlamış his
  ↓
+ Heartbeat Loop (80 BPM) → Kalp atışı
  ↓
+ Tinnitus Ringing (8kHz sine wave) → Çınlama

Sonuç: Zaman yavaşlamış, adrenalin yükselmiş his!
```

---

## 🏗️ Teknik Sistem

### ProAudioManager Features

```
✅ Object Pooling (20 AudioSource pool)
✅ Dictionary Lookup (O(1) hız)
✅ Multi-clip support (varyasyon)
✅ Randomization (pitch, volume)
✅ Layered audio (3-4 layer)
✅ Music crossfade (smooth)
✅ Rage mode effects
✅ Spam prevention
✅ 3D spatial audio
✅ Mixer integration
```

---

## 🛠️ Kurulum

### Adım 1: Audio Mixer Oluşturma

```
Project > Right Click
Create > Audio Mixer

İsim: "MainMixer"
```

#### Mixer Hierarchy:
```
MainMixer
├── Master (Output)
│   ├── Music
│   │   └── RageMode (snapshot için)
│   ├── SFX
│   │   ├── Combat
│   │   ├── Movement
│   │   └── UI
│   └── Ambience
```

#### Mixer Effects:
```
Music Group:
  + Low Pass Filter (Cutoff: 22000 Hz)
  + Reverb (Small room)

Combat SFX:
  + Compressor (Punch harder!)

Ambience:
  + Reverb (Large room)
```

---

### Adım 2: ProAudioManager Setup

```
Hierarchy > Create Empty
Name: "ProAudioManager"

Add Component:
  - ProAudioManager.cs
```

#### Inspector:
```
Pool Size: 20

Sound Effects: (Boyutu artır, örn: 20)
  [0]:
    Name: "Punch_Light"
    Clips: [3 adet punch sesi]
    Volume: 0.7
    Pitch: 1.0
    Volume Variance: 0.1
    Pitch Variance: 0.1
    Mixer Group: Combat
    Layered Sounds: ["Whoosh_Light"]
  
  [1]:
    Name: "Whoosh_Light"
    Clips: [Whoosh_01, Whoosh_02]
    Volume: 0.5
    Pitch: 1.2
    ...

Music Tracks: (Boyutu artır, örn: 6)
  [0]:
    Name: "Level1_Theme"
    Clips: [Slums_BGM.mp3]
    Volume: 0.7
    Loop: ☑ true
    Mixer Group: Music

Audio Mixer: [Drag MainMixer]
SFX Group: [Drag SFX from mixer]
Music Group: [Drag Music from mixer]
```

---

### Adım 3: AudioEventHelper Setup

```
Player GameObject:
  Add Component:
    - AudioEventHelper.cs

Inspector:
  Punch Whoosh Sounds: ["Whoosh_Light", "Whoosh_Medium"]
  Kick Whoosh Sounds: ["Whoosh_Heavy"]
  Impact Sounds: ["Impact_Flesh", "Impact_Bone"]
  Footstep Sounds: ["Step_01", "Step_02", "Step_03"]
```

---

## 🎯 Kullanım Örnekleri

### Örnek 1: Basit SFX

```csharp
// Tek ses
ProAudioManager.Instance.PlaySFX("Punch_Light");

// 3D pozisyonda
ProAudioManager.Instance.PlaySFXAtPosition("Explosion", explosionPos);
```

---

### Örnek 2: Layered Attack Sound

```csharp
// Heavy punch (3 katman)
ProAudioManager.Instance.PlayLayeredSound(
    "Whoosh_Heavy",     // Swing
    "Impact_Flesh",     // Body
    "Impact_Bone"       // Bone crack
);

// Veya pre-configured:
ProAudioManager.Instance.PlayPunchCombo(
    "Whoosh_Heavy",
    "Impact_Flesh",
    "Impact_Bone"
);
```

**Sonuç**: Crazy Flasher tarzı tatmin edici "THUD!" 💥

---

### Örnek 3: Music Control

```csharp
// Level başında
ProAudioManager.Instance.PlayMusic("Level1_Theme", fadeDuration: 2f);

// Boss fight
ProAudioManager.Instance.PlayMusic("Boss_Theme", fadeDuration: 1f);

// Rage mode
ProAudioManager.Instance.ToggleRageMode(true);
```

---

### Örnek 4: Animation Event Integration

#### Animation Timeline:
```
Punch Animation (0.5s):

Frame 0:   [Start]
Frame 2:   AE_PlayPunchWhoosh()    ← Whoosh önce
Frame 3:   AE_EnableHitbox()
Frame 4:   AE_PlayImpactSound()    ← Impact (hitbox active)
Frame 6:   AE_DisableHitbox()
Frame 12:  [End]
```

#### Heavy Punch Animation:
```
Frame 0:   [Start]
Frame 3:   AE_PlayKickWhoosh()
Frame 5:   AE_EnableHitbox()
Frame 6:   AE_PlayHeavyPunchLayered()  ← Layered!
Frame 8:   AE_DisableHitbox()
Frame 15:  [End]
```

---

## 📊 Sound Library (Önerilen)

### Combat SFX (30-40 dosya)

#### Whoosh (Swing Sounds)
```
Whoosh_Light_01.wav    (0.1s, 4kHz, -6dB)
Whoosh_Light_02.wav
Whoosh_Light_03.wav
Whoosh_Medium_01.wav   (0.15s, 3kHz, -4dB)
Whoosh_Heavy_01.wav    (0.2s, 2kHz, 0dB)
Whoosh_Kick_01.wav     (0.25s, 1.5kHz, +2dB)
```

#### Impact (Hit Sounds)
```
Impact_Flesh_01.wav    (Slap sound)
Impact_Flesh_02.wav
Impact_Bone_01.wav     (Celery snap)
Impact_Bone_02.wav
Impact_SubBass.wav     (Kick drum, 60Hz)
Impact_Metal.wav       (Clang, for armored enemies)
```

#### Special
```
Bone_Crack_01.wav      (Critical hits)
Blood_Splatter.wav     (Gore sound)
Hit_Spark.wav          (Metallic zing)
Glass_Break.wav        (Finishers)
```

---

### Movement SFX (15-20 dosya)

```
Step_Concrete_01-05.wav
Step_Water_01-03.wav
Jump_Whoosh.wav
Land_Impact.wav
Dash_Whoosh.wav
Dodge_Roll.wav
```

---

### UI SFX (10 dosya)

```
Menu_Hover.wav         (Metalik scrape)
Menu_Select.wav        (Heavy click)
Menu_Click.wav         (Light click)
Menu_Open.wav          (Whoosh + click)
Menu_Close.wav         (Reverse whoosh)
UI_ComboMilestone.wav  ("GODLIKE!" için)
```

---

### Music Tracks (6-8 dosya)

```
MainMenu_Theme.mp3     (3:00, loop)
Level1_Slums.mp3       (2:30, loop)
Level2_Market.mp3      (2:45, loop)
Level3_Subway.mp3      (3:00, loop)
Level4_Tower.mp3       (3:15, loop)
Boss_Theme.mp3         (2:00, loop with intensity change)
Victory_Theme.mp3      (0:45, one-shot)
GameOver_Theme.mp3     (0:30, one-shot)
```

---

## 🎚️ Audio Mixer Setup

### Mixer Groups

```
Master (0 dB)
├── Music (-6 dB)
│   └── Effects:
│       - Low Pass (Cutoff: 22000 Hz) ← Rage mode için
│       - Reverb (Room size: 0.2)
│
├── SFX (0 dB)
│   ├── Combat (+3 dB) ← Boost (önemli sesler)
│   │   └── Effects:
│   │       - Compressor (Punch harder!)
│   │
│   ├── Movement (-3 dB)
│   └── UI (-6 dB)
│
└── Ambience (-12 dB)
    └── Effects:
        - Reverb (Room size: 0.8)
```

---

### Snapshots (Audio States)

#### Normal Snapshot:
```
Music: Full volume, no filter
SFX: Normal
Ambience: Low volume
```

#### RageMode Snapshot:
```
Music: 
  - Volume: -6 dB
  - Low Pass: 500 Hz (boğuk!)
  - Pitch: 0.8x (yavaşlamış)

SFX:
  - Combat: +6 dB (daha güçlü!)
  
Extra:
  - Heartbeat loop added
  - Tinnitus ringing (8kHz)
```

#### PauseMenu Snapshot:
```
Music: -20 dB (çok hafif)
SFX: Muted
UI: Normal
```

---

## 🎯 Kullanım Örnekleri

### Örnek 1: Combat Audio (Frame-Perfect)

```csharp
// Hitbox.cs - OnTriggerEnter2D
void OnTriggerEnter2D(Collider2D other)
{
    IDamageable target = other.GetComponent<IDamageable>();
    if (target != null)
    {
        // Hasar ver
        target.TakeDamage(damage, direction);
        
        // LAYERED AUDIO!
        ProAudioManager.Instance.PlayLayeredSound(
            "Impact_Flesh",
            "Impact_Bone",
            "Hit_Spark"
        );
        
        // Critical hit?
        if (isCritical)
        {
            ProAudioManager.Instance.PlaySFX("Bone_Crack");
        }
    }
}
```

---

### Örnek 2: Animation-Driven Audio

```csharp
// AnimationEventReceiver'a ek metodlar

public void AE_PlayPunchSound()
{
    audioEventHelper.AE_PlayPunchWhoosh();
}

public void AE_PlayImpact()
{
    audioEventHelper.AE_PlayImpactSound();
}

// Heavy attack için:
public void AE_PlayHeavyImpact()
{
    audioEventHelper.AE_PlayHeavyPunchLayered();
}
```

**Animation Timeline:**
```
Punch Animation:
  Frame 2: AE_PlayPunchSound()   ← Whoosh
  Frame 4: AE_PlayImpact()       ← Impact
```

---

### Örnek 3: Dynamic Music

```csharp
// LevelManager.cs
void Start()
{
    // Level müziği başlat
    ProAudioManager.Instance.PlayMusic("Level1_Slums", 2f);
}

void StartBossFight()
{
    // Boss müziğine geç (smooth crossfade)
    ProAudioManager.Instance.PlayMusic("Boss_Theme", 1.5f);
}

void OnBossPhaseChange()
{
    // Boss canı %50'ye düştü
    // Müziğe intensity ekle (veya farklı loop)
    ProAudioManager.Instance.PlayMusic("Boss_Theme_Intense", 1f);
}
```

---

### Örnek 4: Rage Mode

```csharp
// PlayerController.cs
void ActivateRageMode()
{
    // Visual effects
    // ...
    
    // Audio effects
    ProAudioManager.Instance.ToggleRageMode(true);
    
    // Sonuç:
    // - Müzik boğuklaşır (low-pass)
    // - Pitch düşer (0.8x)
    // - Kalp atış sesi loop başlar
    // - Tinnitus ringing (high pitch)
}

void DeactivateRageMode()
{
    ProAudioManager.Instance.ToggleRageMode(false);
}
```

---

## 🎚️ Audio Mixing Guidelines

### Frequency Allocation

```
Sub-Bass (20-60 Hz):     Heavy impacts, bass drops
Bass (60-250 Hz):        Punch impacts, kick drum
Low-Mid (250-500 Hz):    Body sounds
Mid (500-2kHz):          Voice, ambient
High-Mid (2-4kHz):       Whoosh, movement
High (4-8kHz):           Details, sparkle
Air (8-20kHz):           Shimmer, breath
```

### Volume Balance

```
Music:          -6 dB to -3 dB
Combat SFX:     0 dB to +3 dB (en önemli!)
Movement SFX:   -3 dB to 0 dB
UI SFX:         -6 dB
Ambience:       -12 dB to -9 dB (background)
```

### Ducking (Automatic Volume)

```
Combat SFX çaldığında:
  Music: -6 dB (geçici)
  
Boss saldırısında:
  Music: -9 dB
  Combat SFX: +3 dB (vurgu)
```

---

## 📐 Inspector Setup Örneği

### SoundData Configuration

#### Punch_Light:
```
Name: "Punch_Light"
Clips: [Punch_01, Punch_02, Punch_03]
Volume: 0.7
Pitch: 1.0
Volume Variance: 0.1
Pitch Variance: 0.1
Loop: false
Is 3D: false
Min Repeat Delay: 0
Mixer Group: Combat
Layered Sounds: ["Whoosh_Light"]
```

#### Impact_Flesh:
```
Name: "Impact_Flesh"
Clips: [Slap_01, Slap_02]
Volume: 0.8
Pitch: 0.9
Volume Variance: 0.15
Pitch Variance: 0.2 (daha fazla varyasyon!)
Mixer Group: Combat
Layered Sounds: [] (empty)
```

#### Whoosh_Heavy:
```
Name: "Whoosh_Heavy"
Clips: [HeavySwing_01]
Volume: 0.6
Pitch: 0.8 (daha derin)
Mixer Group: Combat
```

---

## 🎮 Game Feel Optimization

### 1. **Hit Confirm Audio**

```
Good Hit Feel =
  Visual (Hit spark + blood) +
  Haptic (Screen shake + hitstop) +
  Audio (Layered punch sound)

Crazy Flasher formula:
  Whoosh (0.1s before) +
  Impact (exact hit frame) +
  Detail (0.05s after)
```

---

### 2. **Combo Audio Feedback**

```
Hit 1-5:    Normal volume
Hit 6-10:   +3 dB boost
Hit 11-20:  +6 dB boost + pitch up (1.1x)
Hit 21+:    +9 dB + reverb + "GODLIKE!" voice

Sonuç: Combo büyüdükçe ses de epic olur!
```

---

### 3. **Environmental Audio**

```
Slums Level:
  - Distant sirens (loop, -15 dB)
  - Dog barking (random, 10-30s interval)
  - Wind gusts

Market Level:
  - Rain (loop, -9 dB)
  - Crowd chatter (distant)
  - Neon buzzing

Subway Level:
  - Train rumble (constant)
  - Metal scraping
  - Brake screech (transitions)
```

---

## 🐛 Troubleshooting

### Problem 1: Ses Çalmıyor
```
Sebep: Sound map'te yok

Çözüm:
✓ ProAudioManager > Sound Effects listesinde var mı?
✓ Name field doğru yazılmış mı? (case-sensitive!)
✓ Clips assign edilmiş mi?
```

### Problem 2: Robotik Duyuluyor
```
Sebep: Varyasyon yok

Çözüm:
✓ 3+ farklı clip ekle
✓ Pitch Variance: 0.1-0.2
✓ Volume Variance: 0.1
```

### Problem 3: Pool Doluyor
```
Sebep: Çok fazla ses aynı anda

Çözüm:
✓ Pool Size artır (20 → 40)
✓ Min Repeat Delay ekle (spam önleme)
✓ Priority system (önemsiz sesleri skip et)
```

### Problem 4: Music Crossfade Çalışmıyor
```
Sebep: DOTween yok ve coroutine hatası

Çözüm:
✓ DOTween yükle (önerilir!)
✓ Veya coroutine debug et
✓ Console log kontrol et
```

---

## 💡 Pro Tips

### 1. Audio Compression
```
Format: WAV (editing için)
Export: OGG Vorbis (Unity için)
  - Music: 192 kbps
  - SFX: 128 kbps
  - Ambience: 96 kbps

Neden? Dosya boyutu %70 küçülür!
```

### 2. Normalize Audio
```
Peak Amplitude: -3 dB (headroom)
RMS: -12 dB (average)

Tool: Audacity (free!)
  Effect > Normalize > Peak: -3.0 dB
```

### 3. Loop Points
```
Müzikler seamless loop olmalı:
  - Başlangıç ve bitiş aynı faz
  - Fade in/out overlap (0.5s)
  - Test: Loop 10 kez, tıklama var mı?
```

### 4. Spatial Audio
```
3D sesler için:
  - Rolloff Mode: Linear
  - Min Distance: 1
  - Max Distance: 15
  - Spread: 60-90
```

---

## 🎓 Öğrenme Kaynakları

### Audio Design
- [Game Audio 101](https://www.youtube.com/gameaudio)
- [Layered Sound Design](https://www.asoundeffect.com/layering/)
- [Crazy Flasher Audio Analysis](https://youtube.com)

### Unity Audio
- [Audio Mixer Documentation](https://docs.unity3d.com/Manual/AudioMixer.html)
- [Audio Best Practices](https://learn.unity.com/tutorial/audio-best-practices)

### Tools
- **Audacity**: Free audio editor
- **FMOD**: Advanced audio engine (alternative)
- **Wwise**: AAA audio middleware

---

## ✅ Checklist

### Setup
- [ ] ProAudioManager eklendi
- [ ] Audio Mixer oluşturuldu
- [ ] Sound library organize edildi
- [ ] SFX pool boyutu ayarlandı

### Sound Effects
- [ ] Combat sounds (20+ adet)
- [ ] Movement sounds (10+ adet)
- [ ] UI sounds (10+ adet)
- [ ] Layered combinations ayarlandı

### Music
- [ ] Level themes (4-6 track)
- [ ] Boss theme
- [ ] Victory/Defeat music
- [ ] Loop points test edildi

### Integration
- [ ] Animation events bağlandı
- [ ] Hitbox'tan ses çağrılıyor
- [ ] Music crossfade çalışıyor
- [ ] Rage mode audio test edildi

---

## 🔥 Sonuç

**PRO AUDIO SYSTEM** artık hazır!

### ✅ Features
- Object pooling (performance!)
- Layered audio (3-4 layer)
- Randomization (varyasyon)
- Music crossfade (smooth)
- Rage mode effects (epic!)
- Audio Mixer integration
- Animation event support
- 3D spatial audio

### 🎮 Crazy Flasher Feel
- Tatmin edici "THUD!" sesleri
- Layered punch impacts
- Dynamic music
- Combat audio feedback
- Professional quality

**Artık AAA kalitesinde ses tasarımı yapabilirsiniz!** 🔊

---

**Happy Audio Design!** 🎮🔊⚔️

