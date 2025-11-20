# 🔀 HYBRID SYSTEM - Best of Both Worlds

Bu doküman, **Neon Syndicate: Retribution** projesindeki **Hibrit Sistemi** açıklar.

---

## 🎯 Hibrit Sistem Nedir?

İki farklı implementasyon yaklaşımının **güçlü yönlerini birleştiren** mimari:

### 🅰️ Class-Based FSM (Enterprise Yaklaşım)
```
✅ Modüler state sınıfları
✅ SOLID prensipleri
✅ Kolay test edilebilir
✅ Büyük projelerde maintainable
```

### 🅱️ Coroutine-Based Actions (Pragmatik Yaklaşım)
```
✅ Aksiyon odaklı işlemler için ideal
✅ Timing ve async işlemler kolay
✅ Unity workflow'una uygun
✅ Daha az boilerplate kod
```

---

## 🏗️ Mimari Yapısı

### State Machine (Class-Based)

Karakter davranışları **temiz state sınıfları** ile yönetilir:

```
PlayerStateMachine
  ├── IdleState
  ├── WalkState
  ├── AttackState
  ├── JumpState
  ├── DodgeState
  ├── HurtState
  └── DeathState
```

**Avantaj**: Her state'in kendi dosyası var, logic ayrı ve temiz.

---

### Actions (Coroutine-Based)

**Zaman bazlı aksiyonlar** PlayerController'da coroutine ile implement edilir:

```csharp
// PlayerController.cs
public IEnumerator DashCoroutine() { ... }
public IEnumerator JumpCoroutine() { ... }
```

**Avantaj**: Timing, lerp, async işlemler doğal bir şekilde yazılır.

---

## 🔄 Nasıl Çalışır?

### Örnek: Jump State

**1. State Enter (Class-Based)**
```csharp
// PlayerJumpState.cs
public override void Enter() 
{
    playerSM.Animator.SetTrigger("Jump");
    SoundManager.Instance?.PlaySFX("Jump");
    
    // Coroutine'i başlat
    playerSM.Controller.StartActionCoroutine(JumpWithCallback());
}
```

**2. Coroutine Execution (Coroutine-Based)**
```csharp
// PlayerController.cs
public IEnumerator JumpCoroutine() 
{
    isGrounded = false;
    float elapsed = 0f;
    
    while (elapsed < jumpDuration) 
    {
        elapsed += Time.deltaTime;
        // Parabolic arc hesaplama
        float height = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
        // Pozisyon güncelleme
        yield return null;
    }
    
    isGrounded = true;
}
```

**3. State Update (Class-Based)**
```csharp
// PlayerJumpState.cs
public override void Update() 
{
    // Havada saldırı kontrolü
    if (InputHandler.Instance.IsAttackPressed) 
    {
        playerSM.ChangeState(playerSM.AttackState);
    }
    
    // Coroutine bittiğinde Idle'a dön
    if (jumpComplete) 
    {
        playerSM.ChangeState(playerSM.IdleState);
    }
}
```

---

## ✨ Hibrit Sistemin Avantajları

### 1. **Temiz State Transitions**

Class-based FSM sayesinde state geçişleri net:

```csharp
if (input.magnitude > 0.1f) 
{
    stateMachine.ChangeState(stateMachine.WalkState);
}
```

### 2. **Doğal Timing Operations**

Coroutine sayesinde zaman bazlı işlemler kolay:

```csharp
yield return new WaitForSeconds(dashDuration);
```

### 3. **Interrupt Mekanizması**

State değiştiğinde coroutine'ler temiz bir şekilde durur:

```csharp
public void StopCurrentAction() 
{
    if (currentActionCoroutine != null) 
    {
        StopCoroutine(currentActionCoroutine);
    }
}
```

### 4. **Modüler Eklemeler**

Yeni bir aksiyon eklemek çok basit:

```csharp
// 1. PlayerController'a coroutine ekle
public IEnumerator WallRunCoroutine() { ... }

// 2. WallRunState oluştur
public class PlayerWallRunState : StateBase 
{
    public override void Enter() 
    {
        playerSM.Controller.StartActionCoroutine(WallRunWithCallback());
    }
}

// 3. Bitti!
```

---

## 🎮 Kullanılan Özellikler

### **Fake Height Jump (2.5D)**

Crazy Flasher tarzı zıplama:

```csharp
// Gerçek fizik yok, görsel offset var
float heightOffset = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
transform.position = startPos + Vector3.up * heightOffset;

// Collider ve shadow yerde kalır!
```

**Neden?** 2.5D oyunlarda gerçek fizik bazlı zıplama collision sorunları yaratır.

---

### **Coroutine Momentum**

Saldırı sırasında karaktere momentum:

```csharp
private void ApplyAttackMomentum() 
{
    Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
    rb.velocity = direction * 2f; // Hafif ileri hareket
}
```

**Sonuç**: Crazy Flasher'daki o "akıcı" hareket hissi.

---

### **Run/Sprint Sistemi**

Shift tuşu ile koşma:

```csharp
// PlayerController.cs
isRunning = Input.GetKey(KeyCode.LeftShift) && staminaCurrent > 0;

// Stamina tüketimi
if (isRunning && moving) 
{
    staminaCurrent -= 5f * Time.deltaTime;
}

// WalkState'te hız artışı
float currentSpeed = playerSM.Controller.MoveSpeed;
if (playerSM.Controller.isRunning) 
{
    currentSpeed *= playerSM.Controller.RunSpeedMultiplier; // x1.6
}
```

---

### **Responsive Attack System**

Combo penceresi sistemi:

```csharp
// Saldırı sırasında tekrar tuşa basılırsa
if (InputHandler.Instance.IsAttackPressed && Combat.CanContinueCombo()) 
{
    Enter(); // Combo'nun sonraki adımına geç
}
```

**Sonuç**: "Button mashing" yerine **ritmik saldırı** yapılabiliyor.

---

### **Air Attack Support**

Havada saldırı sistemi:

```csharp
if (!controller.isGrounded) 
{
    ExecuteAirAttack();
    
    // Havada asılı kalma (Gravity defying)
    controller.Rb.velocity = Vector2.zero;
}
```

**Kullanım**: Düşmanı havaya kaldır (Launcher) → Zıpla → Havada kombo yap!

---

## 📋 Kullanım Örnekleri

### Yeni State Ekleme

```csharp
// 1. State sınıfını oluştur
public class PlayerWallSlideState : StateBase 
{
    public override void Enter() 
    {
        // Görsel feedback
        playerSM.Animator.SetBool("IsWallSliding", true);
        
        // Eğer coroutine gerekiyorsa
        playerSM.Controller.StartActionCoroutine(WallSlideRoutine());
    }
    
    public override void Update() 
    {
        // Input kontrolü
        if (InputHandler.Instance.IsJumpPressed) 
        {
            playerSM.ChangeState(playerSM.JumpState);
        }
    }
}

// 2. Coroutine gerekiyorsa PlayerController'a ekle
public IEnumerator WallSlideCoroutine() 
{
    while (wallSliding) 
    {
        rb.velocity = new Vector2(0, -2f); // Yavaş kayma
        yield return null;
    }
}
```

---

### Yeni Aksiyon Ekleme (Coroutine-only)

```csharp
// PlayerController.cs
public IEnumerator RollCoroutine() 
{
    SetInvulnerable(0.5f); // I-frame
    
    for (float t = 0; t < 0.5f; t += Time.deltaTime) 
    {
        rb.velocity = rollDirection * rollSpeed;
        yield return null;
    }
    
    rb.velocity = Vector2.zero;
}

// State'den çağır
playerSM.Controller.StartActionCoroutine(playerSM.Controller.RollCoroutine());
```

---

## ⚖️ Trade-offs (Ödünler)

### Avantajlar ✅
1. **Esneklik**: Her iş için en uygun yöntemi kullan
2. **Maintainability**: State logic ayrı, action timing ayrı
3. **Performance**: Coroutine'ler verimli (proper kullanımda)
4. **Readability**: Timing işlemleri okunabilir

### Dezavantajlar ❌
1. **Complexity**: İki farklı pattern öğrenmek gerekir
2. **Debugging**: Coroutine debug bazen zor olabilir
3. **Learning Curve**: Yeni developer'lar için biraz karışık

---

## 🎯 Ne Zaman Hangisi?

### Class-Based State Kullan:
- ✅ Input'a göre davranış değişimi
- ✅ Birden fazla state geçişi
- ✅ Karmaşık condition'lar
- ✅ Test edilmesi gereken logic

**Örnek**: Walk, Idle, Attack gibi temel state'ler

---

### Coroutine Kullan:
- ✅ Zaman bazlı işlemler (duration, delay)
- ✅ Lerp/interpolation
- ✅ Async operasyonlar
- ✅ Animation synchronization

**Örnek**: Jump, Dash, Roll gibi timed aksiyonlar

---

### İkisini Birlikte Kullan:
- ✅ State logic + timing gereken durumlar
- ✅ Interrupt edilebilir aksiyonlar
- ✅ Callback gereken işlemler

**Örnek**: Jump State (state logic) + Jump Coroutine (parabolic arc timing)

---

## 🚀 Best Practices

### 1. Coroutine Tracking
```csharp
private Coroutine currentActionCoroutine;

public void StartActionCoroutine(IEnumerator coroutine) 
{
    StopCurrentAction(); // Öncekini durdur
    currentActionCoroutine = StartCoroutine(coroutine);
}
```

### 2. Callback Pattern
```csharp
private IEnumerator JumpWithCallback() 
{
    yield return controller.JumpCoroutine();
    jumpComplete = true; // State'e bildir
}
```

### 3. State Check
```csharp
// Coroutine içinde state kontrolü yap
while (elapsed < duration) 
{
    if (stateMachine.CurrentState != this) 
    {
        yield break; // State değiştiyse dur
    }
    yield return null;
}
```

### 4. Cleanup
```csharp
public override void Exit() 
{
    controller.StopCurrentAction(); // Coroutine'i temizle
    // Diğer cleanup işlemleri
}
```

---

## 🎓 Öğrenme Kaynakları

### State Machine
- [Game Programming Patterns - State](http://gameprogrammingpatterns.com/state.html)
- Unity FSM tutorials

### Coroutines
- [Unity Coroutine Documentation](https://docs.unity3d.com/Manual/Coroutines.html)
- [Coroutine Best Practices](https://www.youtube.com/watch?v=ciDD6Wl-Evk)

---

## 🔍 Debugging Tips

### State Machine Debug
```csharp
[SerializeField] private string currentStateName; // Inspector'da gör

void Update() 
{
    currentStateName = CurrentState?.GetType().Name;
}
```

### Coroutine Debug
```csharp
public IEnumerator JumpCoroutine() 
{
    Debug.Log("Jump Started");
    
    while (...) 
    {
        Debug.DrawRay(transform.position, Vector3.up, Color.green);
        yield return null;
    }
    
    Debug.Log("Jump Ended");
}
```

---

## 📊 Performans Notları

### Coroutine Performance
- ✅ **İyi**: Birkaç frame boyunca çalışan işlemler
- ⚠️ **Dikkat**: Her frame yeni coroutine başlatma
- ❌ **Kötü**: Binlerce simultaneous coroutine

### Optimization
```csharp
// ❌ Kötü - Her frame yeni coroutine
void Update() 
{
    StartCoroutine(DoSomething());
}

// ✅ İyi - Tek sefer başlat
void Start() 
{
    StartCoroutine(DoSomethingContinuous());
}
```

---

## ✅ Sonuç

**Hibrit Sistem**, projenize:
- 🏗️ **Yapı** (class-based FSM)
- ⚡ **Esneklik** (coroutine actions)
- 🎮 **Game Feel** (responsive timing)

katar!

**Crazy Flasher'ın responsive hissini** modern bir mimaride yakalamak için ideal yaklaşım! 🚀

---

**Happy Coding!** 🎮

