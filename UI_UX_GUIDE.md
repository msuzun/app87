# 🎨 UI/UX SYSTEM - Complete Guide

**Kinetic, Reactive, Cyberpunk-Noir UI**

Crazy Flasher'ın vahşi arcade hissini modern estetikte! Her vuruşta UI da tepki verir!

---

## 📋 İçindekiler

- [Görsel Tasarım Dili](#-görsel-tasarım-dili)
- [Ekran Tasarımları](#-ekran-tasarımları)
- [Kurulum](#-kurulum)
- [Component Rehberi](#-component-rehberi)
- [DOTween Integration](#-dotween-integration)

---

## 🎨 Görsel Tasarım Dili

### Renk Paleti

#### Ana Renkler
```
Asfalt Grisi:  #1a1a1a (Arka plan)
Kan Kırmızısı: #ff0033 (Tehlike, HP)
Beyaz Duman:   #e0e0e0 (Text)
```

#### Neon Vurgu
```
Siber Camgöbeği: #00f3ff (Vurgu, glow)
Toksik Yeşil:    #39ff14 (Stamina, pickup)
Elektrik Mavi:   #0080ff (Rage bar)
```

#### Combo Renkleri
```
D (0-30):     #808080 (Gri)
C (30-60):    #ffffff (Beyaz)
B (60-100):   #ffff00 (Sarı)
A (100-150):  #ff8800 (Turuncu)
S (150-200):  #ff0000 (Kırmızı)
SS (200-300): #ff00ff (Mor)
SSS (300+):   #00ffff (Camgöbeği) GODLIKE!
```

---

### Font Seçimleri

#### Başlıklar
```
Font: "Road Rage" / "Commando" / "Bebas Neue Bold"
Style: Fırça darbeli, agresif
Use: Menu titles, "WAVE 1", "BOSS"
```

#### Sayılar (HP, Damage)
```
Font: "Bebas Neue" / "Impact"
Style: Kalın, okunabilir
Use: Health numbers, combo counter
```

#### Body Text
```
Font: "Roboto Condensed" / "Exo 2"
Style: Futuristik sans-serif
Use: Açıklamalar, menü itemleri
```

---

### Dokular ve Efektler

```
✓ Yırtılmış metal (UI arka planlar)
✓ Kan lekeleri (düşük HP'de)
✓ Glitch (bozulma) efektleri
✓ Scanline (tüplü ekran çizgileri)
✓ VHS noise
✓ Neon glow (shader)
```

---

## 🖼️ Ekran Tasarımları

### A. Ana Menü (Main Menu)

#### Layout
```
┌─────────────────────────────────────┐
│                                     │
│  [Animated Background]              │
│  Yağmurlu sokak + Axel sırtı dönük  │
│                                     │
│            NEON SYNDICATE           │
│             RETRIBUTION             │
│           [Neon Glow Effect]        │
│                                     │
│                    │  STORY MODE    │
│                    │  SURVIVAL 🔒   │
│                    │  SETTINGS      │
│                    │  EXIT          │
│                                     │
└─────────────────────────────────────┘
```

#### Interaksiyon
```
Hover → Metalik ses + Kırmızı renk + Kan damlası
Click → Axel yüzünü döner
Select → Fade out transition
```

---

### B. HUD (Heads-Up Display)

#### Layout
```
┌─────────────────────────────────────┐
│ [Axel Portrait]    COMBO: 15  🎯   │
│ ████████░░ HP      HITS!            │
│ ████░░░░░░ Rage    Rank: A          │
│                                     │
│                                     │
│          [GAMEPLAY AREA]            │
│                                     │
│                                     │
│ ════════════════ Boss HP ═════════  │
└─────────────────────────────────────┘
```

#### Reaktif Özellikler
```
HP Bar:
  - Can azalınca → Portre kanlanır
  - Kritik (<30%) → Yanıp söner
  - Smooth animation (lerp/DOTween)

Rage Bar:
  - Dolarken → Renk değişimi (sarı→kırmızı)
  - Full → Alev efekti + pulse

Combo Counter:
  - Her hit → Punch scale animation
  - Renk değişimi (combo arttıkça)
  - Milestone: "GODLIKE!" text
```

---

### C. Damage Numbers (Hasar Yazıları)

#### Tipler

**Normal Hit**
```
Text: "10"
Color: Beyaz
Size: 0.8
Movement: Yukarı, hafif rastgele
Physics: Gravity + friction
```

**Critical Hit**
```
Text: "25!"
Color: Sarı
Size: 1.5
Movement: Sağa yukarı fırlar
Animation: Punch scale + shake
```

**Player Hurt**
```
Text: "15"
Color: Kırmızı
Size: 1.0
Movement: Aşağı dökülen
Effect: Blood drip
```

---

### D. Pause Menu

#### Layout
```
┌─────────────────────────────────────┐
│ [VHS Glitch Effect]                 │
│ [Scanline Overlay]                  │
│ [Frozen Gameplay - B&W]             │
│                                     │
│            PAUSED                   │
│                                     │
│          ▶ RESUME                   │
│            MOVE LIST                │
│            RESTART                  │
│            MAIN MENU                │
│                                     │
└─────────────────────────────────────┘
```

#### Efektler
```
Open → VHS glitch (flicker)
Background → Siyah-beyaz + scanlines
Buttons → Glow on hover
Sound → Metalik click
```

---

## 🛠️ Kurulum

### Adım 1: Canvas Hierarchy

```
Hierarchy > Create > UI > Canvas

Canvas:
  - Render Mode: Screen Space - Overlay
  - Canvas Scaler:
      UI Scale Mode: Scale With Screen Size
      Reference Resolution: 1920x1080
      Match: 0.5 (Width/Height)

Hierarchy:
Canvas_HUD
├── PlayerStatus (Sol üst)
│   ├── Portrait_Frame (Image)
│   │   └── Portrait (Image)
│   ├── HealthBar_BG (Image)
│   │   └── HealthBar_Fill (Image, Filled)
│   ├── RageBar_BG (Image)
│   │   └── RageBar_Fill (Image, Filled)
│   └── StaminaBar_BG
│       └── StaminaBar_Fill
│
├── ComboDisplay (Sağ üst)
│   ├── Combo_BG (Image)
│   ├── Combo_Number (TextMeshPro)
│   └── Combo_Label (TextMeshPro: "HITS")
│
├── StyleRank (Sağ üst, combo'nun üstünde)
│   ├── Rank_BG
│   └── Rank_Text (TextMeshPro: "SSS")
│
├── BossHealth (Alt, full width)
│   ├── BossHP_BG (Image)
│   ├── BossHP_Fill (Image, Filled)
│   └── BossName_Text (TextMeshPro)
│
└── Effects (Full screen overlays)
    ├── FlashOverlay (Image, Color: White, Alpha: 0)
    ├── GlitchOverlay (Image, Alpha: 0)
    ├── ScanlineOverlay (Image, Tiling)
    └── VignetteOverlay (Image, Radial gradient)
```

---

### Adım 2: Component Ekleme

```
Canvas_HUD:
  - HUDManager.cs ← Ana script
  - UIEffects.cs ← Efektler

Assign References:
  HP Bar Fill: [Drag HealthBar_Fill]
  Rage Bar Fill: [Drag RageBar_Fill]
  Combo Text: [Drag Combo_Number]
  ...
```

---

### Adım 3: DOTween Kurulumu (Opsiyonel ama ÖNERİLİR!)

#### DOTween Asset Yükleme
```
1. Asset Store'dan "DOTween (HOTween v2)" indir (ÜCRETSİZ!)
2. Import
3. Tools > Demigiant > DOTween Utility Panel
4. Setup DOTween
```

#### Scripting Define Symbol Ekleme
```
Edit > Project Settings > Player > Scripting Define Symbols
Ekle: DOTWEEN_ENABLED

Sonuç: Tüm DOTween kodları aktif olur!
```

**DOTween Yoksa**: Kod yine çalışır, sadece basit animasyonlar kullanılır.

---

## 📐 Component Rehberi

### HUDManager.cs

#### Public Methods:
```csharp
UpdateHealth(float current, float max)
  // HP barını günceller, portrait değişir

UpdateRage(float current, float max)
  // Rage barını günceller

UpdateStamina(float current, float max)
  // Stamina barını günceller

AddCombo()
  // Kombo sayacını artırır, kinetic animation

ResetCombo()
  // Komboyu sıfırlar

ShowBossHealth(string name, float max)
  // Boss health bar'ı gösterir

UpdateBossHealth(float current, float max)
  // Boss HP günceller
```

#### Kullanım:
```csharp
// PlayerController'da:
void TakeDamage(float damage)
{
    currentHealth -= damage;
    HUDManager.Instance.UpdateHealth(currentHealth, maxHealth);
}

// PlayerCombat'ta:
void OnHitEnemy()
{
    HUDManager.Instance.AddCombo();
}
```

---

### DamagePopupUI.cs

#### Setup Method:
```csharp
Setup(int damage, bool isCritical, bool isPlayerDamage)
```

#### Kullanım:
```csharp
// Hitbox.cs'de:
void OnTriggerEnter2D(Collider2D other)
{
    // Hasar ver
    damageable.TakeDamage(damage, ...);
    
    // Damage popup spawn
    GameObject popup = ObjectPooler.Instance.SpawnFromPool(
        "DamagePopup",
        hitPosition,
        Quaternion.identity
    );
    
    popup.GetComponent<DamagePopupUI>().Setup(
        (int)damage,
        isCritical: damage > 20,
        isPlayerDamage: false
    );
}
```

---

### UIEffects.cs

#### Public Methods:
```csharp
ShakeScreen(float intensity, float duration)
  // Ekranı sarsar

FlashScreen(Color? color, float duration)
  // Ekranı flashlar (beyaz/kırmızı)

GlitchEffect(float duration)
  // VHS glitch efekti

SetVignetteIntensity(float intensity)
  // Kenarları karartır (can azalınca)

ActivateRageMode()
  // Rage mode visual efektleri

DeactivateRageMode()
  // Rage mode efektlerini kaldır
```

#### Kullanım:
```csharp
// Hasar alındığında:
UIEffects.Instance.ShakeScreen(0.5f, 0.2f);
UIEffects.Instance.FlashScreen(Color.red, 0.1f);

// Critical hit:
UIEffects.Instance.FlashScreen(Color.yellow, 0.15f);
UIEffects.Instance.GlitchEffect(0.2f);

// Can azaldı:
UIEffects.Instance.SetVignetteIntensity(0.5f);
```

---

## 🎯 UI Prefab Yapısı

### UI_HUD_Panel.prefab

```
UI_HUD_Panel (RectTransform)
├── PlayerStatus_Container
│   ├── Portrait_Frame (160x160)
│   │   ├── Portrait_Mask
│   │   └── Portrait_Image
│   ├── Bars_Container
│   │   ├── HP_Bar (Horizontal Layout)
│   │   │   ├── BG (Image: Bar_BG sprite)
│   │   │   ├── Fill (Image: Filled, Red)
│   │   │   └── Icon (Image: Heart icon)
│   │   ├── Rage_Bar
│   │   │   ├── BG
│   │   │   ├── Fill (Image: Yellow→Red gradient)
│   │   │   └── Icon (Fire icon)
│   │   └── Stamina_Bar
│   │       ├── BG
│   │       ├── Fill (Green)
│   │       └── Icon (Lightning icon)
│   └── Stats_Text (Level, Money - opsiyonel)
│
├── ComboDisplay_Container
│   ├── Combo_BG (Image: Semi-transparent)
│   ├── Combo_Number (TMP: "999")
│   ├── Combo_Label (TMP: "HITS")
│   └── Glow_Effect (Image: Radial blur)
│
├── StyleRank_Container
│   ├── Rank_BG
│   ├── Rank_Text (TMP: "SSS")
│   └── Rank_Glow
│
└── BossHealth_Container
    ├── Boss_BG (Full width bar)
    ├── Boss_Fill (Red, Filled)
    ├── BossName_Text (TMP: "IRON HEAD")
    └── Boss_Skull_Icon
```

---

### UI_DamagePopup.prefab

```
World Space Canvas
├── DamageText (TextMeshPro)
│   Font: Bebas Neue Bold
│   Size: Dynamic (normal: 0.8, crit: 1.5)
│   Alignment: Center
│   Sorting Layer: UI
│   Order: 100
└── DamagePopupUI.cs
```

**Object Pooler Setup:**
```
Pools:
  Tag: "DamagePopup"
  Prefab: [UI_DamagePopup]
  Size: 30
```

---

### UI_MainMenu.prefab

```
Canvas_MainMenu
├── Background_Animated
│   └── Axel_Character (Animator)
├── Title_Container
│   ├── Title_Text (TMP: "NEON SYNDICATE")
│   ├── Subtitle (TMP: "RETRIBUTION")
│   └── Glow_Effect
├── Menu_Buttons (Vertical Layout)
│   ├── Btn_StoryMode
│   ├── Btn_Survival (Locked)
│   ├── Btn_Settings
│   └── Btn_Exit
└── Effects
    └── Scanline_Overlay
```

---

### UI_PauseMenu.prefab

```
Canvas_Pause
├── Freeze_Background (Image: B&W screenshot)
├── Glitch_Overlay (Flicker effect)
├── Scanline_Overlay (Scrolling)
├── Menu_Panel
│   ├── Title (TMP: "PAUSED")
│   ├── Buttons
│   │   ├── Btn_Resume
│   │   ├── Btn_MoveList
│   │   ├── Btn_Restart
│   │   └── Btn_MainMenu
│   └── MoveList_Panel (Collapsible)
│       └── MoveList_Text
└── Vignette
```

---

## 🎮 DOTween Integration

### Neden DOTween?

```
✅ Smooth animations (1 satır kod!)
✅ Easing functions (30+ adet)
✅ Chain & sequence support
✅ Performance optimized
✅ Industry standard

❌ Unity Animator: UI için hantal
❌ Manual lerp: Çok fazla kod
✅ DOTween: Perfect!
```

### Örnekler

#### Health Bar Animation
```csharp
// ❌ Manuel lerp (20 satır kod)
IEnumerator LerpHP(float target) { ... }

// ✅ DOTween (1 satır!)
hpBar.DOFillAmount(target, 0.3f).SetEase(Ease.OutCirc);
```

#### Combo Punch Effect
```csharp
// ❌ Manuel
transform.localScale = Vector3.one * 1.5f;
// + Coroutine ile küçült...

// ✅ DOTween
transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 5);
```

#### Fade In/Out
```csharp
// ❌ Manuel (canvas group alpha loop)
while (alpha < 1f) { ... }

// ✅ DOTween
canvasGroup.DOFade(1f, 0.5f);
```

---

## 🎨 Visual Effects Rehberi

### 1. Screen Shake (Ekran Sarsma)

```csharp
// Hafif (normal hit)
UIEffects.Instance.ShakeScreen(0.3f, 0.1f);

// Orta (critical hit)
UIEffects.Instance.ShakeScreen(0.7f, 0.2f);

// Güçlü (boss attack)
UIEffects.Instance.ShakeScreen(1.5f, 0.4f);
```

**Kullanım Yeri**:
- Player hasar alınca
- Critical hit yapınca
- Boss saldırısında
- Explosion efektlerinde

---

### 2. Flash Effect (Ekran Parlama)

```csharp
// Beyaz flash (hit confirm)
UIEffects.Instance.FlashScreen(Color.white, 0.1f);

// Kırmızı flash (damage)
UIEffects.Instance.FlashScreen(Color.red, 0.15f);

// Sarı flash (critical)
UIEffects.Instance.FlashScreen(Color.yellow, 0.1f);
```

**Kullanım Yeri**:
- Her vuruş (hafif)
- Critical hit (güçlü)
- Player damage (kırmızı)
- Execution move (slow-mo + flash)

---

### 3. Glitch Effect (VHS Bozulma)

```csharp
UIEffects.Instance.GlitchEffect(0.2f);
```

**Kullanım Yeri**:
- Pause açılınca
- Execution move
- Boss phase transition
- Critical moment

---

### 4. Vignette (Kenar Karartma)

```csharp
// Can durumuna göre
float vignetteIntensity = 1f - (currentHP / maxHP);
UIEffects.Instance.SetVignetteIntensity(vignetteIntensity * 0.5f);
```

**Kullanım Yeri**:
- HP azaldıkça artar
- Rage mode'da kırmızı vignette
- Boss fight'ta atmosfer

---

## 📊 HUD Update Flow

### Her Frame:
```
PlayerController.Update()
  ↓
currentHealth değişti mi?
  ↓ (Evet)
HUDManager.Instance.UpdateHealth(current, max)
  ↓
HP bar smooth animation
Portrait değişimi
Can kritikse warning effect
```

### Hit Confirm:
```
Hitbox.OnHit()
  ↓
HUDManager.Instance.AddCombo()
  ↓
Combo counter artış
Punch scale animation
Renk değişimi
Milestone check
  ↓
DamagePopup spawn
  ↓
Fizik bazlı animation
Fade out
```

---

## 🎯 Kinetic UI Prensipleri

### 1. **Reactivity** (Tepkisellik)
```
Her game event → UI feedback
  - Hit → Combo punch
  - Damage → Screen shake
  - Critical → Flash + glitch
  - Rage full → Glow pulse
```

### 2. **Juice** (Canlılık)
```
Basit bir sayı artışı → Animasyonlu artış
  - Scale punch
  - Color shift
  - Shake
  - Particle
```

### 3. **Feedback** (Geri Bildirim)
```
Her aksiyon → Ses + Visual
  - Button hover → Sound + Color
  - HP loss → Shake + Red flash
  - Combo milestone → Text + Sound
```

### 4. **Clarity** (Netlik)
```
Bilgi hiyerarşisi:
  1. Player HP (En önemli) → Sol üst, büyük
  2. Combo (Geçici) → Sağ üst, sadece varken
  3. Boss HP (Contextual) → Alt, boss varken
```

---

## 🐛 Troubleshooting

### Problem 1: HUD Görünmüyor
```
Sebep: Canvas ayarları yanlış

Çözüm:
✓ Canvas > Render Mode: Screen Space Overlay
✓ Canvas Scaler kurulmuş mu?
✓ Event System var mı? (GameObject > UI > Event System)
```

### Problem 2: Combo Counter Animasyon Yok
```
Sebep: DOTween yüklü değil veya define symbol yok

Çözüm:
✓ DOTween asset'i yükle
✓ Scripting Define: "DOTWEEN_ENABLED" ekle
✓ Veya useAdvancedAnimations = false yap (basit animasyon)
```

### Problem 3: Damage Popup Spawn Olmuyor
```
Sebep: Object Pooler'da tanımlı değil

Çözüm:
✓ ObjectPooler > Pools listesine ekle:
  Tag: "DamagePopup"
  Prefab: [UI_DamagePopup]
  Size: 30
```

### Problem 4: Portrait Değişmiyor
```
Sebep: Portrait sprites assign edilmemiş

Çözüm:
✓ HUDManager > Portrait States:
  [0] Healthy sprite
  [1] Hurt sprite
  [2] Critical sprite
  [3] Dying sprite
```

---

## 🎨 Styling Guide

### Bar Styling (HP, Rage, Stamina)

#### Image Component:
```
Image Type: Filled
Fill Method: Horizontal
Fill Origin: Left
Fill Amount: 1.0 (başlangıç)
```

#### Color Gradient:
```
HP Bar:
  Full (100%):    #00ff00 (Yeşil)
  Half (50%):     #ffff00 (Sarı)
  Critical (20%): #ff0000 (Kırmızı) + Pulse

Rage Bar:
  Empty (0%):     #ffaa00 (Turuncu)
  Full (100%):    #ff0000 (Kırmızı) + Flame effect

Stamina:
  Full:           #39ff14 (Neon yeşil)
  Empty:          #808080 (Gri)
```

---

### Text Styling (TextMeshPro)

#### Combo Counter:
```
Font: Bebas Neue Bold
Font Size: 120
Color: Dynamic (combo'ya göre)
Outline: 5px, siyah
Glow: 10px, renk ile aynı
```

#### Boss Name:
```
Font: Road Rage / Impact
Font Size: 72
Color: #ff0033 (Kırmızı)
Outline: Heavy, siyah
Letter Spacing: 10
```

---

## 💡 Pro Tips

### 1. Texture Atlasing
```
Tüm UI sprite'larını bir atlas'ta birleştir
Performance: Çok daha iyi (draw call azalır)
```

### 2. Unscaled Time
```
Pause menüde animasyonlar için:
.SetUpdate(true) // Unscaled time kullan

Örnek:
canvasGroup.DOFade(1f, 0.3f).SetUpdate(true);
```

### 3. DOTween Recycling
```
Aynı objeye çok animasyon → Kill önce

transform.DOKill(); // Önceki tween'leri temizle
transform.DOScale(1f, 0.3f); // Yeni tween
```

### 4. Sprite Slicing
```
9-Slice kullan (panel, bar BG için)
Unity'de Sprite Editor > Slice Type: 9-Slice
Sonuç: Ölçek değişse bile köşeler bozulmaz
```

---

## 📋 Checklist

### Temel HUD
- [ ] Canvas kuruldu
- [ ] HUDManager component eklendi
- [ ] HP/Rage/Stamina bars bağlandı
- [ ] Portrait sprites assign edildi
- [ ] Combo container ayarlandı

### Effects
- [ ] UIEffects component eklendi
- [ ] Flash overlay var
- [ ] Glitch overlay var
- [ ] Vignette overlay var

### Damage Popup
- [ ] Prefab oluşturuldu
- [ ] TextMeshPro ayarlandı
- [ ] Object Pooler'a eklendi
- [ ] Hitbox'tan spawn ediliyor

### Menu
- [ ] Main Menu scene var
- [ ] Pause Menu prefab var
- [ ] Butonlar bağlandı
- [ ] Hover effects çalışıyor

### DOTween (Opsiyonel)
- [ ] Asset yüklendi
- [ ] Setup yapıldı
- [ ] DOTWEEN_ENABLED tanımlandı
- [ ] Animasyonlar test edildi

---

## 🎓 Öğrenme Kaynakları

### DOTween
- [DOTween Documentation](http://dotween.demigiant.com/documentation.php)
- [DOTween Video Tutorials](https://www.youtube.com/results?search_query=dotween+unity)

### UI Design
- [Game UI Database](https://www.gameuidatabase.com/)
- [UI Animation Principles](https://uxdesign.cc/ui-animation-principles)

### TextMeshPro
- [TMP Documentation](https://docs.unity3d.com/Packages/com.unity.textmeshpro@latest)
- [TMP Styling Guide](https://learn.unity.com/tutorial/textmesh-pro)

---

## 🔥 Sonuç

**KINETIC UI/UX SYSTEM** artık hazır!

### ✅ Features
- Reaktif HUD (smooth animations)
- Damage popups (physics-based)
- Main menu (animated)
- Pause menu (VHS glitch)
- Screen effects (shake, flash, glitch)
- Boss health bar
- Style rank display
- DOTween powered (optional)

### 🎮 Crazy Flasher Feel
- Her vuruşta UI tepki verir
- Combo counter kinetic
- Screen shake feedback
- Critical hit flash
- Arcade atmosphere

**Artık AAA kalitesinde UI/UX yapabilirsiniz!** 🎨

---

**Happy UI Design!** 🎮🎨⚔️

