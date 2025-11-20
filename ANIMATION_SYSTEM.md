# 🎬 EVENT-DRIVEN ANIMATION SYSTEM

**Frame-Perfect Combat** için profesyonel animasyon mimarisi.

Crazy Flasher tarzı oyunlar için **kritik önem** taşır - vuruşların tam zamanında gerçekleşmesi gerekir!

---

## 📋 İçindekiler

- [Sistem Özellikleri](#-sistem-özellikleri)
- [Mimari](#-mimari)
- [Kurulum](#-kurulum)
- [Animation Event Ekleme](#-animation-event-ekleme)
- [Kullanım Örnekleri](#-kullanım-örnekleri)
- [Spine Entegrasyonu](#-spine-entegrasyonu)

---

## ✨ Sistem Özellikleri

### 1. **Frame-Perfect Combat** 🎯
```
Hitbox tam vuruş karesinde aktif olur
Animation event ile kontrol edilir
Milimetrik timing precision
```

### 2. **Event-Driven Architecture** 📡
```
Mantık kodu animasyondan bağımsız
Event-based communication
Loose coupling
```

### 3. **Teknoloji Bağımsız** 🔄
```
Unity Animator → CharacterAnimator.cs
Spine 2D → CharacterAnimatorSpine.cs
Aynı API, farklı backend!
```

### 4. **Cancel Windows** ⏱️
```
AE_OpenComboWindow event'i
Oyuncu bu andan itibaren kombo yapabilir
AE_CloseComboWindow ile kapanır
```

### 5. **I-Frame Control** 🛡️
```
AE_IFrameStart → Invulnerable
AE_IFrameEnd → Vulnerable
Dodge/Roll animasyonları için
```

---

## 🏗️ Mimari

### Katmanlar (Layers)

```
[Controller/State Machine]
         ↓ (PlayAnimation)
[CharacterAnimator Wrapper]
         ↓ (CrossFade/SetAnimation)
[Unity Animator / Spine]
         ↓ (Animation Events)
[AnimationEventReceiver]
         ↓ (C# Events)
[Combat/Movement Scripts]
         ↓ (Hitbox/VFX/Sound)
```

### 3 Ana Component

#### **1. AnimData.cs** - Constants
```csharp
String hatalarını önler
public const string ATTACK_1 = "Attack_Light_1";

✅ animator.Play(AnimData.ATTACK_1);
❌ animator.Play("Attack1"); // Typo riski!
```

#### **2. AnimationEventReceiver.cs** - Event Hub
```csharp
Unity Animation Event → C# Event dönüşümü
public event UnityAction OnHitboxEnable;

Animation timeline'dan çağrılır
```

#### **3. CharacterAnimator.cs** - Wrapper
```csharp
Mantık ile animator arasında köprü
public void PlayAnimation(string state, bool isCombat);

Blend/Mix yönetimi
```

---

## 🛠️ Kurulum

### Adım 1: Component Ekleme

```
Player GameObject:
  ├── Rigidbody2D
  ├── Animator ← Zaten var
  ├── SpriteRenderer
  ├── CharacterAnimator ← YENİ!
  │   └── Event Receiver: [Animator GameObject'i]
  └── AnimationEventReceiver ← YENİ! (Animator ile aynı objede)
```

**Önemli**: AnimationEventReceiver, **Animator ile aynı GameObject'te** olmalı!

---

### Adım 2: Inspector Ayarları

#### CharacterAnimator
```
Default Blend Duration: 0.15
  (Normal geçişler için)

Combat Blend Duration: 0.05
  (Dövüş için - daha keskin)

Instant Blend Duration: 0
  (Anında geçiş)

Show Debug Info: ☑ (Test için)
```

#### AnimationEventReceiver
```
Log Events: ☑ (Test için)
```

---

## 🎨 Animation Event Ekleme

### Unity Animation Window'da Event Ekleme

#### Adım 1: Animation Window Aç
```
Window > Animation > Animation
```

#### Adım 2: Animasyon Seç
```
Attack_Light_1 animasyonunu seç
```

#### Adım 3: Event Ekle

**Vuruş Karesinde** (örn: Frame 3):
```
Timeline'da sağ tık > Add Animation Event

Function: AE_EnableHitbox
Time: 0.15 (örnek - yumruğun en ileri olduğu an)
```

**Vuruş Sonrası** (örn: Frame 6):
```
Function: AE_DisableHitbox
Time: 0.30
```

**Combo Window** (örn: Frame 4):
```
Function: AE_OpenComboWindow
Time: 0.20
```

**Animasyon Sonu** (Son frame):
```
Function: AE_AnimationFinish
Time: 0.48 (animasyon uzunluğuna göre)
```

---

### Örnek Timeline

```
Attack_Light_1 (0.5 saniye, 12 frame @ 24fps)

Frame 0:    [Start]
Frame 2:    AE_OpenComboWindow (combo yapılabilir)
Frame 3:    AE_EnableHitbox (VURUŞ!)
Frame 5:    AE_DisableHitbox
Frame 8:    AE_CloseComboWindow
Frame 12:   AE_AnimationFinish [End]
```

---

## 🎯 Kullanım Örnekleri

### Örnek 1: Basic Attack (Event-Driven)

#### PlayerCombatAnimated.cs
```csharp
void Start()
{
    // Event'lere abone ol
    characterAnimator.EventReceiver.OnHitboxEnable += EnableHitbox;
    characterAnimator.EventReceiver.OnHitboxDisable += DisableHitbox;
}

public void OnAttackInput()
{
    // Animasyonu başlat
    characterAnimator.PlayAnimation(AnimData.ATTACK_1, isCombatAction: true);
}

// Animation event tarafından çağrılır
private void EnableHitbox()
{
    punchHitbox.Activate(transform);
    Debug.Log("Hitbox OPEN - Frame perfect!");
}

private void DisableHitbox()
{
    punchHitbox.Deactivate();
}
```

**Sonuç**: Hitbox tam vuruş karesinde açılır! 🎯

---

### Örnek 2: Combo System (Event-Driven Cancel Windows)

#### Setup
```csharp
void Start()
{
    characterAnimator.EventReceiver.OnComboWindowOpen += OpenWindow;
    characterAnimator.EventReceiver.OnComboWindowClose += CloseWindow;
}

private bool canCombo = false;

private void OpenWindow()
{
    canCombo = true; // Oyuncu şimdi kombo yapabilir
}

private void CloseWindow()
{
    canCombo = false; // Artık yapamaz
}

void Update()
{
    if (Input.GetKeyDown(KeyCode.Z) && canCombo)
    {
        ContinueCombo(); // Zinciri devam ettir
    }
}
```

**Sonuç**: Timing skill gerektiren combo sistemi! ⏱️

---

### Örnek 3: I-Frame System (Dodge)

```csharp
void Start()
{
    characterAnimator.EventReceiver.OnIFrameStart += StartInvulnerability;
    characterAnimator.EventReceiver.OnIFrameEnd += EndInvulnerability;
}

public void OnDodgeInput()
{
    characterAnimator.PlayAnimation(AnimData.DODGE, isCombatAction: true);
    // I-Frame otomatik animation event ile aktif olacak!
}

private void StartInvulnerability()
{
    playerController.SetInvulnerable(true);
    Debug.Log("I-FRAME ACTIVE");
}

private void EndInvulnerability()
{
    playerController.SetInvulnerable(false);
    Debug.Log("I-FRAME ENDED");
}
```

**Dodge Animation Events**:
```
Frame 1: AE_IFrameStart
Frame 2-8: Invulnerable
Frame 9: AE_IFrameEnd
```

---

### Örnek 4: VFX Spawn (Dust, Blood, vb.)

```csharp
void Start()
{
    characterAnimator.EventReceiver.OnSpawnVFX += SpawnEffect;
}

private void SpawnEffect(string effectName)
{
    ObjectPooler.Instance.SpawnFromPool(effectName, transform.position, Quaternion.identity);
}
```

**Dash Animation**:
```
Frame 1: AE_SpawnVFX (String: "DustCloud")
Frame 5: AE_SpawnVFX (String: "DustCloud")
```

---

## 🎮 Pratik Örnek: 3-Hit Combo

### Asset Setup (ScriptableObject)

Önceki ProComboSystem ile kombine kullanılabilir!

### Animation Event Setup

#### Attack_Light_1:
```
0.00s: [Start]
0.10s: AE_OpenComboWindow
0.15s: AE_EnableHitbox ← VURUŞ!
0.25s: AE_DisableHitbox
0.40s: AE_CloseComboWindow
0.50s: AE_AnimationFinish [End]
```

#### Attack_Light_2:
```
0.00s: [Start]
0.12s: AE_OpenComboWindow
0.18s: AE_EnableHitbox ← VURUŞ!
0.28s: AE_DisableHitbox
0.45s: AE_CloseComboWindow
0.55s: AE_AnimationFinish [End]
```

#### Attack_Light_3 (Finisher):
```
0.00s: [Start]
0.20s: AE_EnableHitbox ← VURUŞ!
0.30s: AE_DisableHitbox
0.40s: AE_CameraShake (Float: 0.5)
0.60s: AE_AnimationFinish [End]

(Combo window YOK - finisher!)
```

---

## 🔧 Integration Guide

### CharacterAnimator + ProComboSystem

Her iki sistemi birlikte kullanabilirsiniz!

```csharp
public class PlayerCombatHybrid : MonoBehaviour
{
    [SerializeField] private CharacterAnimator characterAnim;
    [SerializeField] private ProComboSystem proCombo;

    void Start()
    {
        // Animation event'leri dinle
        characterAnim.EventReceiver.OnHitboxEnable += () => {
            // ProComboSystem'den damage al
            float damage = proCombo.CurrentMove.damage;
            punchHitbox.Activate(transform, damage);
        };
    }
}
```

**Sonuç**: Data-driven combo + Frame-perfect timing! 🎯

---

## 🎨 Spine Kullanımı

### Spine Setup

#### 1. Spine Runtime Yükle
```
Window > Package Manager
Spine-Unity Runtime import et
```

#### 2. CharacterAnimatorSpine Kullan
```
Player GameObject:
  ├── SkeletonAnimation (Spine component)
  ├── CharacterAnimatorSpine ← Spine version
  └── AnimationEventReceiver
```

#### 3. Spine Animator'da Event Ekle

Spine Editor (Esoteric Software):
```
1. Animation seç (örn: Punch1)
2. Event track ekle
3. Event oluştur:
   Name: "HitFrame"
   Time: 0.15s (vuruş karesı)
```

Unity'de otomatik dinlenir!

---

## 📊 Performans

### String vs Hash

```csharp
// ❌ Yavaş (string comparison)
animator.SetBool("IsWalking", true);

// ✅ Hızlı (int comparison)
animator.SetBool(AnimData.Hash.IsWalking, true);

// Performans farkı: ~10x daha hızlı
```

### Event Count

```
Animasyon başına max 6-8 event önerilir
Çok fazla event frame drop'a sebep olabilir
```

---

## 🐛 Troubleshooting

### Problem 1: Event Çalışmıyor
```
Sebep: AnimationEventReceiver yanlış objede

Çözüm:
✓ Animator ile AYNI GameObject'te olmalı
✓ Genelde "PlayerSprite" child objesi
✓ Hierarchy doğru mu kontrol et
```

### Problem 2: Hitbox Açılmıyor
```
Sebep: Event subscription eksik

Çözüm:
✓ Start()'ta subscribe edilmiş mi?
✓ OnDestroy()'da unsubscribe edilmiş mi?
✓ Event name doğru mu? (AE_EnableHitbox)
```

### Problem 3: Combo Window Çalışmıyor
```
Sebep: Event timing yanlış

Çözüm:
✓ OpenComboWindow erken frame'de
✓ CloseComboWindow geç frame'de
✓ İkisi arasında yeterli süre var mı?
```

### Problem 4: Spine Event Çalışmıyor
```
Sebep: Event name mismatch

Çözüm:
✓ Spine'da: "HitFrame"
✓ OnSpineEvent switch'te: case "HitFrame"
✓ Tam eşleşmeli!
```

---

## 📚 API Reference

### CharacterAnimator

#### Methods:
```csharp
PlayAnimation(string state, bool isCombat, float blend)
  // Animasyon oynat

PlayAnimationInstant(string state)
  // Anında oynat (blend yok)

SetPlaybackSpeed(float speed)
  // Hız ayarla (hitstop için)

SetMovementSpeed(float speed)
  // Blend tree speed parametresi

SetBool/Float/Int/Trigger(...)
  // Parameter ayarlama

IsPlayingState(string state)
  // State kontrolü

GetNormalizedTime()
  // Animasyon progress (0-1)
```

---

### AnimationEventReceiver

#### Events:
```csharp
// Combat
OnHitboxEnable
OnHitboxDisable
OnComboWindowOpen
OnComboWindowClose
OnAnimationComplete

// Movement
OnFootstep
OnJumpStart
OnLand

// VFX
OnSpawnVFX(string effectName)
OnTrailStart
OnTrailStop

// Camera
OnCameraShake(float intensity)

// Invulnerability
OnIFrameStart
OnIFrameEnd
```

---

### AnimData.cs

#### State Names:
```csharp
AnimData.IDLE
AnimData.WALK
AnimData.ATTACK_LIGHT_1
AnimData.ATTACK_HEAVY
AnimData.DODGE
AnimData.DEATH
// ... ve daha fazlası
```

#### Parameter Hashes:
```csharp
AnimData.Hash.IsWalking
AnimData.Hash.Speed
AnimData.Hash.Attack
// ... (performans optimizasyonu)
```

---

## 🎯 Best Practices

### 1. Event Naming Convention
```
AE_ prefix (Animation Event)
  AE_EnableHitbox
  AE_SpawnVFX
  AE_Footstep
```

### 2. Event Timing
```
Vuruş Kareleri:
  Light Attack: Frame 3-4
  Heavy Attack: Frame 5-7
  
Combo Windows:
  Open: Frame 2-3
  Close: Frame 8-10
  
I-Frames:
  Start: Frame 1
  End: Last frame
```

### 3. Subscription Pattern
```csharp
void Start()
{
    // Subscribe
    receiver.OnEvent += Handler;
}

void OnDestroy()
{
    // Unsubscribe (Memory leak önleme!)
    receiver.OnEvent -= Handler;
}
```

### 4. Null Check
```csharp
// ✅ İyi
eventReceiver?.OnHitboxEnable?.Invoke();

// ❌ Kötü
eventReceiver.OnHitboxEnable.Invoke(); // NullRef riski!
```

---

## 🎬 Workflow

### Typical Flow

```
1. Designer:
   Unity'de animasyon oluşturur
   Event'leri ekler (frame-perfect timing)
   
2. Programmer:
   Event handler kodlar
   Combat/Movement logic yazar
   
3. Test:
   Play mode
   Debug log ile event'leri kontrol et
   Timing ayarlaması (iterate)
```

---

## 💡 İleri Seviye

### Custom Events

Yeni event eklemek:

#### 1. AnimationEventReceiver'a event ekle:
```csharp
public event UnityAction OnCustomEvent;

public void AE_CustomEvent()
{
    OnCustomEvent?.Invoke();
}
```

#### 2. Animation'a ekle:
```
Function: AE_CustomEvent
```

#### 3. Subscribe et:
```csharp
receiver.OnCustomEvent += MyHandler;
```

---

### Multi-Parameter Events

```csharp
// AnimationEventReceiver.cs
public event UnityAction<string, float> OnComplexEvent;

public void AE_ComplexEvent(string name)
{
    OnComplexEvent?.Invoke(name, 1.0f);
}

// Unity Animation Event:
Function: AE_ComplexEvent
String: "EffectName"
```

**Not**: Unity event'ler max 1 parametre alır. Çoklu parametre için wrapper kullan.

---

### State Machine Integration

```csharp
// PlayerAttackState.cs
public override void Enter()
{
    characterAnimator.PlayAnimation(AnimData.ATTACK_1, true);
    
    // Animation complete'te Idle'a dön
    characterAnimator.EventReceiver.OnAnimationComplete += () => {
        stateMachine.ChangeState(stateMachine.IdleState);
    };
}
```

---

## 📈 Comparison

### Eski Sistem (Timer-Based)
```csharp
❌ float attackTimer = 0;
❌ if (timer > 0.15f) EnableHitbox();
❌ if (timer > 0.25f) DisableHitbox();

Sorunlar:
- Animasyon değişirse kod değiştirilmeli
- Frame-perfect değil (delta time hataları)
- Maintainance zor
```

### Yeni Sistem (Event-Driven)
```csharp
✅ Animation Event: Frame 3 → AE_EnableHitbox
✅ Animation Event: Frame 6 → AE_DisableHitbox

Avantajlar:
- Animasyon değişirse event pozisyonu kaydırılır
- Frame-perfect precision
- Designer'lar kod yazmadan ayarlayabilir
```

---

## 🎓 Öğrenme Kaynakları

### Unity Dokümanları
- [Animation Events](https://docs.unity3d.com/Manual/script-AnimationWindowEvent.html)
- [Animator.CrossFade](https://docs.unity3d.com/ScriptReference/Animator.CrossFade.html)

### Spine Dokümanları
- [Spine-Unity Runtime](http://esotericsoftware.com/spine-unity)
- [Spine Events](http://esotericsoftware.com/spine-unity-events)

### Fighting Game Tutorials
- GDC Talks: "Animation Driven Combat"
- Street Fighter V: Technical Analysis

---

## ✅ Checklist

### Kurulum
- [ ] CharacterAnimator component eklendi
- [ ] AnimationEventReceiver eklendi (Animator objede!)
- [ ] Inspector ayarları yapıldı
- [ ] Event subscription'lar yazıldı

### Animation Setup
- [ ] Tüm attack animasyonları var
- [ ] Event'ler eklendi (vuruş kareleri)
- [ ] Combo window event'leri eklendi
- [ ] Footstep event'leri eklendi (walk/run)

### Testing
- [ ] Event'ler fire oluyor (console log)
- [ ] Hitbox frame-perfect açılıyor
- [ ] Combo timing doğru
- [ ] VFX spawn oluyor

---

## 🔥 Sonuç

**EVENT-DRIVEN ANIMATION SYSTEM** ile:

- ✅ **Frame-Perfect Combat** (Crazy Flasher kalitesi!)
- ✅ **Designer-Friendly** (kod yazmadan timing ayarlama)
- ✅ **Maintainable** (animasyon değişirse event pozisyonu kaydır)
- ✅ **Scalable** (Spine'a geçiş kolay)
- ✅ **Professional** (AAA oyun standartları)

**Artık Street Fighter / Devil May Cry kalitesinde frame-perfect combat yapabilirsiniz!** 🥊

---

**Happy Animating!** 🎬⚔️

