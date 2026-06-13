# BEAN BOY — Sprout Valley Vertical Slice Specification

## 1. Overview

This specification covers the implementation of a complete, playable Sprout Valley level — the first of three planned levels in Bean Boy. The slice includes a fully featured player character, two enemy types, collectibles, hazards, a mini-boss, and a switch/gate puzzle. Everything is built in Godot 4.6.3 with C# scripts attached to `.tscn` scenes.

The scope is limited to Sprout Valley only. Levels 2 and 3, the Root King, dialogue, menus, audio, and save systems are deferred.

**Design constraints:**
- Viewport: 128×128 pixels
- Tile size: 8×8 pixels
- Palette: PICO-8 16-colour

Physics values are tuned around a **walk speed of 5 tiles/s** and a **jump reaching 3 tiles high** (~1.5s full cycle). Project gravity (`physics/2d/default_gravity`) should be set to **85 px/s²** in `project.godot`. All components read this value.

---

## 2. Modules

### 2.1 PlayerController

Attached to `scenes/bean-boy.tscn` (root `CharacterBody2D`).

#### Data structures

```csharp
enum PlayerState
{
    OnGround,
    Jumping,
    Falling,
    DoubleJumping,
    WallSliding,
    Dashing
}
```

Export parameters on the script:

| Field | Type | Default | Notes |
|-------|------|---------|-------|
| `MoveSpeed` | `float` | `40.0` | 5 tiles/s |
| `JumpVelocity` | `float` | `-64.0` | 3 tiles high |
| `DoubleJumpVelocity` | `float` | `-52.0` | ~2 tiles high |
| `WallSlideSpeed` | `float` | `15.0` | ~2 tiles/s fall cap while sliding |
| `DashSpeed` | `float` | `96.0` | ~12 tiles/s burst |
| `DashDuration` | `float` | `0.15` | Dash travel time in seconds |
| `DashCooldown` | `float` | `2.0` | Seconds before dash can be used again |

Gravity is read from project settings: `(float)ProjectSettings.GetSetting("physics/2d/default_gravity")`

Dependencies (child node references, cached in `_Ready()`):
- `AnimatedSprite2D` (named `AnimatedSprite2D`)
- `CollisionShape2D` (named `CollisionShape2D`)
- `Timer@DashCooldownTimer` (child `Timer` node, one-shot)

#### Function signatures

```csharp
public partial class PlayerController : CharacterBody2D
```

| Signature | Description |
|-----------|-------------|
| `void _Ready()` | Cache node references. Validate required children. Start idle animation. |
| `void _PhysicsProcess(double delta)` | Main game loop: read input, apply gravity, handle state transitions, call `MoveAndSlide()`, update animation. |
| `void HandleInput()` | Poll `Input.GetAxis("move_left", "move_right")`, `Input.IsActionJustPressed("jump")`, `Input.IsActionJustPressed("dash")`. Mutate velocity and transition state. |
| `void ApplyGravity(double delta)` | Apply `gravity * delta` to `Velocity.Y` unless wall sliding. Cap `Velocity.Y` at terminal velocity (500 px/s). |
| `void Jump(float velocity)` | Set `Velocity.Y = velocity`. Reset double-jump flag. Transition to `Jumping`. |
| `void DoubleJump()` | Only if `_hasDoubleJump` is true. Set `Velocity.Y = DoubleJumpVelocity`. Set `_hasDoubleJump = false`. Transition to `DoubleJumping`. |
| `void Dash()` | Only if cooldown timer is stopped. Set `_isDashing = true` for `DashDuration` seconds. Apply horizontal burst in facing direction. Start cooldown timer. |
| `void UpdateAnimation()` | Map `PlayerState` + local state to animation name. |
| `bool IsOnWall()` | Check for wall collision (left or right, not floor). Used to enter/exit `WallSliding`. |

#### PlayerState transitions

```
OnGround  --[no floor]-->  Falling
OnGround  --[jump]-->      Jumping
Jumping   --[no upward vel]-->  Falling
Jumping   --[double jump]--> DoubleJumping
Falling   --[landed]-->   OnGround
Falling   --[against wall + input]--> WallSliding
Falling   --[double jump]--> DoubleJumping
DoubleJumping --[landed]--> OnGround
DoubleJumping --[no upward vel]--> Falling
WallSliding --[no wall]--> Falling
WallSliding --[jump]--> Jumping  (wall jump: push away from wall)
WallSliding --[landed]--> OnGround
Dashing   --[timer expired]--> Falling (or OnGround if landed during dash)
```

---

### 2.2 Health

Attached as a child `Node` named "Health" on the player scene.

#### Data structures

```csharp
// Exported so max HP is adjustable in-editor.
```

| Field | Type | Default | Notes |
|-------|------|---------|-------|
| `MaxHp` | `int` | `3` | Maximum and starting HP |
| `InvincibilityDuration` | `float` | `1.0` | Seconds of invincibility after taking damage |
| `CurrentHp` | `int` | (readonly) | Set to `MaxHp` in `_Ready()` |
| `IsInvincible` | `bool` | (readonly) | True during invincibility frames |

#### Signals

| Signal | Parameters | Emitted when |
|--------|------------|--------------|
| `Damaged` | `int newHp` | Player takes damage |
| `Died` | (none) | HP reaches 0 |
| `Healed` | `int newHp` | Player gains HP (future use) |

#### Function signatures

```csharp
public partial class Health : Node
```

| Signature | Description |
|-----------|-------------|
| `void TakeDamage(int amount)` | If `IsInvincible`, return early. Subtract `amount` from `CurrentHp`, clamp to 0. Emit `Damaged`. If `CurrentHp == 0`, emit `Died`. Start invincibility timer. |
| `void Heal(int amount)` | Add `amount` to `CurrentHp`. Clamp to `MaxHp`. Emit `Healed`. |
| `void _OnInvincibilityTimerTimeout()` | Set `IsInvincible = false`. |

---

### 2.3 GameState (Autoload)

A singleton `Node` named `GameState` added to the project's autoload list.

#### Data structures

```csharp
public partial class GameState : Node
```

| Field | Type | Default | Notes |
|-------|------|---------|-------|
| `GreenBeansCollected` | `int` | `0` | Total green beans collected |
| `GoldenBeansCollected` | `int` | `0` | Total golden beans collected (future) |
| `HasCaveKey` | `bool` | `false` | Set to true when Cave Key is picked up |
| `TotalGreenBeans` | `int` | `100` | Total green beans in the game (constant) |

#### Function signatures

```csharp
public partial class GameState : Node
```

| Signature | Description |
|-----------|-------------|
| `void CollectGreenBean()` | Increment `GreenBeansCollected` by 1. |
| `void CollectGoldenBean()` | Increment `GoldenBeansCollected` by 1. |
| `void AwardCaveKey()` | Set `HasCaveKey = true`. |
| `void ResetSession()` | Reset all fields to defaults. |

---

### 2.4 GreenBean

An `Area2D` scene placed in the level. Each pickup increments the green bean counter on `GameState`.

#### Data structures

```csharp
public partial class GreenBean : Area2D
```

No exported fields.

#### Signals

| Signal | Parameters | Emitted when |
|--------|------------|--------------|
| `Collected` | (none) | Bean has been picked up |

#### Function signatures

```csharp
public partial class GreenBean : Area2D
```

| Signature | Description |
|-----------|-------------|
| `void _Ready()` | Connect `BodyEntered` to `OnBodyEntered`. Start `"spin"` animation. |
| `void OnBodyEntered(Node2D body)` | If `body is PlayerController`: call `GameState.CollectGreenBean()`. Emit `Collected`. Play a brief pickup tween (scale → 0 over 0.2s), then `QueueFree()`. |

#### Scene layout

```
GreenBean (Area2D)
├── CollisionShape2D (RectangleShape2D, size=6x6)
└── AnimatedSprite2D
    └── sprite_frames: bean-coin (2 frames, "spin" animation, autoplay)
```

---

### 2.5 CaveKey

An `Area2D` scene placed at the end of the level. Pickup awards the Cave Key on `GameState`.

#### Data structures

```csharp
public partial class CaveKey : Area2D
```

No exported fields.

#### Signals

| Signal | Parameters | Emitted when |
|--------|------------|--------------|
| `Collected` | (none) | Key has been picked up |

#### Function signatures

```csharp
public partial class CaveKey : Area2D
```

| Signature | Description |
|-----------|-------------|
| `void _Ready()` | Connect `BodyEntered` to `OnBodyEntered`. |
| `void OnBodyEntered(Node2D body)` | If `body is PlayerController`: call `GameState.AwardCaveKey()`. Emit `Collected`. Play pickup tween, `QueueFree()`. |

#### Scene layout

```
CaveKey (Area2D)
├── CollisionShape2D (RectangleShape2D, size=6x6)
└── AnimatedSprite2D (requires art)
```

---

### 2.6 TurnipTrooper

A `CharacterBody2D` enemy that patrols left and right, reverses at walls/ledges, and can be stomped from above.

#### Data structures

```csharp
public partial class TurnipTrooper : CharacterBody2D
```

| Field | Type | Default | Notes |
|-------|------|---------|-------|
| `PatrolSpeed` | `float` | `24.0` | 3 tiles/s |
| `Direction` | `int` | `1` | 1 = right, -1 = left |

Gravity is read from project settings.

Dependencies:
- `AnimatedSprite2D` child
- `CollisionShape2D` child
- `Area2D@StompDetector` — child Area2D positioned above the enemy
- `RayCast2D@LedgeDetector` — forward-downward raycast to detect ledge edges
- `RayCast2D@WallDetector` — forward raycast to detect walls

#### Signals

| Signal | Parameters | Emitted when |
|--------|------------|--------------|
| `Defeated` | (none) | Enemy is stomped and destroyed |

#### Function signatures

```csharp
public partial class TurnipTrooper : CharacterBody2D
```

| Signature | Description |
|-----------|-------------|
| `void _Ready()` | Cache node references. Connect `StompDetector.BodyEntered` to `OnStomped`. Set initial direction from `Direction`. |
| `void _PhysicsProcess(double delta)` | Apply gravity, patrol in current direction, check wall/ledge raycasts, reverse direction on hit, call `MoveAndSlide()`, update animation (`"patrol"` while alive). |
| `void OnStomped(Node2D body)` | If `body is PlayerController player && player.Velocity.Y > 0`: play squash animation, emit `Defeated`, set collision layer to 0, `QueueFree()` after 0.3s delay. Call `player.Jump()` with `JumpVelocity * 0.7` (stomp bounce). |
| `void ReverseDirection()` | Multiply `Direction` by -1. Flip `AnimatedSprite2D` scale.x. |

#### Patrolling logic

- Walk at `PatrolSpeed` in `Direction` until `WallDetector` collides or `LedgeDetector` detects no floor ahead.
- On wall/ledge detected: `ReverseDirection()`.

#### Scene layout

```
TurnipTrooper (CharacterBody2D)
├── CollisionShape2D (RectangleShape2D, size=6x5)
├── AnimatedSprite2D (requires art)
├── StompDetector (Area2D)
│   └── CollisionShape2D (RectangleShape2D, size=8x2, positioned at top)
├── LedgeDetector (RayCast2D)  — angled down-forward, length ~4px
└── WallDetector (RayCast2D)   — horizontal forward, length ~4px
```

---

### 2.7 AngryCarrot

A `CharacterBody2D` enemy that idles until the player enters its line-of-sight, then charges horizontally.

#### Data structures

```csharp
public partial class AngryCarrot : CharacterBody2D
```

| Field | Type | Default | Notes |
|-------|------|---------|-------|
| `ChargeSpeed` | `float` | `64.0` | 8 tiles/s while charging |
| `DetectionRange` | `float` | `48.0` | 6 tiles — raycast length for player detection |
| `ChargeDuration` | `float` | `1.0` | Seconds before resetting to idle |
| `CooldownDuration` | `float` | `2.0` | Seconds of idle after a charge misses |

Gravity is read from project settings.

Dependencies:
- `AnimatedSprite2D` child
- `CollisionShape2D` child
- `RayCast2D@LeftDetector` — raycast pointing left
- `RayCast2D@RightDetector` — raycast pointing right

#### States

```
Idle ──[player detected by raycast]──> Telegraph (0.3s, visual cue)
Telegraph ──[timer fires]──> Charging (move at ChargeSpeed toward player)
Charging ──[timer fires OR hits wall]──> Cooldown
Cooldown ──[timer fires]──> Idle
```

#### Function signatures

```csharp
public partial class AngryCarrot : CharacterBody2D
```

| Signature | Description |
|-----------|-------------|
| `void _Ready()` | Cache references. Start in `Idle`. |
| `void _PhysicsProcess(double delta)` | Apply gravity. Cast left and right raycasts. If player detected and state is `Idle`, transition to `Telegraph` and face the player. If `Charging`, move at `ChargeSpeed` toward player. Call `MoveAndSlide()`. |
| `void OnWallHit()` | In `_PhysicsProcess`, check `GetLastSlideCollision()`. If collided with wall and state is `Charging`, transition to `Cooldown`. |

Same stomp mechanic as `TurnipTrooper` — `StompDetector` with `player.Velocity.Y > 0` check.

#### Scene layout

```
AngryCarrot (CharacterBody2D)
├── CollisionShape2D (RectangleShape2D)
├── AnimatedSprite2D (requires art — idle, telegraph, charge animations)
├── StompDetector (Area2D)
│   └── CollisionShape2D (RectangleShape2D, top of enemy)
├── LeftDetector (RayCast2D, target_position = Vector2(-48, 0))
└── RightDetector (RayCast2D, target_position = Vector2(48, 0))
```

---

### 2.8 CrumblingBlock

A `StaticBody2D` that crumbles when the player stands on it.

#### Data structures

```csharp
public partial class CrumblingBlock : StaticBody2D
```

| Field | Type | Default | Notes |
|-------|------|---------|-------|
| `CrumbleDelay` | `float` | `0.5` | Seconds after player stands on it before crumbling starts |

Dependencies:
- `AnimatedSprite2D` child with `crumbling-block` `SpriteFrames`

#### Function signatures

```csharp
public partial class CrumblingBlock : StaticBody2D
```

| Signature | Description |
|-----------|-------------|
| `void _Ready()` | Cache `AnimatedSprite2D`. Play `"default"` animation. |
| `void OnBodyEntered(Node2D body)` | If `body is PlayerController`: start `CrumbleDelay` timer (one-shot). |
| `void OnBodyExited(Node2D body)` | If `body is PlayerController` and timer is running: stop timer (player stepped off). |
| `void OnCrumbleTimerTimeout()` | Play `"crumble"` animation. After animation ends, `QueueFree()`. |

#### Scene layout

```
CrumblingBlock (StaticBody2D)
├── CollisionShape2D (RectangleShape2D, size=8x8)
└── AnimatedSprite2D
    └── sprite_frames: crumbling-block
    └── autoplay: "default"
    └── centered: false
```

---

### 2.9 SpikeBall

A moving hazard that patrols a short horizontal path. Movement is a placeholder for a future `AnimationPlayer`-driven path system.

#### Data structures

```csharp
public partial class SpikeBall : CharacterBody2D
```

| Field | Type | Default | Notes |
|-------|------|---------|-------|
| `Speed` | `float` | `32.0` | 4 tiles/s |
| `Range` | `float` | `32.0` | Half-width of patrol range from starting position |

Gravity is read from project settings.

Dependencies:
- `AnimatedSprite2D` child with `spike-ball` `SpriteFrames`
- `CollisionShape2D` child

#### Function signatures

```csharp
public partial class SpikeBall : CharacterBody2D
```

| Signature | Description |
|-----------|-------------|
| `void _Ready()` | Cache references. Store `_startX = Position.X`. Play `"spin"` animation. |
| `void _PhysicsProcess(double delta)` | Apply gravity. Patrol horizontally between `_startX - Range` and `_startX + Range`. Reverse direction at bounds. Call `MoveAndSlide()`. |
| `void OnBodyEntered(Node2D body)` | If `body is PlayerController`: call `body.Health?.TakeDamage(1)`. |

#### Scene layout

```
SpikeBall (CharacterBody2D)
├── CollisionShape2D (RectangleShape2D, size=7x7)
└── AnimatedSprite2D
    └── sprite_frames: spike-ball
    └── autoplay: "spin"
```

---

### 2.10 Spikes

A scene tile that is placed via a `TileMapLayer`. The `Spikes.tscn` is registered as a scene tile in the TileSet, allowing it to be painted onto a `Spikes` `TileMapLayer` in the level.

Each cell of the TileMapLayer instantiates this scene:

```
Spikes (Area2D)
├── CollisionPolygon2D (triangle shape over the spike tile)
└── AnimatedSprite2D
    └── texture: spikes atlas texture
    └── z_index: -1 (renders behind the player)
└── Spikes.cs
```

#### Function signatures

```csharp
public partial class Spikes : Area2D
```

| Signature | Description |
|-----------|-------------|
| `void _Ready()` | Connect `BodyEntered` to `OnBodyEntered`. |
| `void OnBodyEntered(Node2D body)` | If `body is PlayerController`: call `body.Health?.TakeDamage(1)`. |

---

### 2.11 Switch / Gate Puzzle

A reusable pair of scenes.

#### Switch

```csharp
public partial class Switch : Area2D
```

| Field | Type | Default | Notes |
|-------|------|---------|-------|
| `IsPressed` | `bool` | `false` | Current state |
| `StaysPressed` | `bool` | `true` | If true, switch stays down after activation |

#### Signals

| Signal | Parameters | Emitted when |
|--------|------------|--------------|
| `Toggled` | `bool isPressed` | Switch state changes |

#### Function signatures

```csharp
public partial class Switch : Area2D
```

| Signature | Description |
|-----------|-------------|
| `void OnBodyEntered(Node2D body)` | If `body is PlayerController` or `body is PushableBox`: toggle `IsPressed` (or set true if `StaysPressed`). Update animation. Emit `Toggled(IsPressed)`. |
| `void OnBodyExited(Node2D body)` | If not `StaysPressed` and body was the activator: toggle `IsPressed`. Emit `Toggled(IsPressed)`. |

#### Scene layout

```
Switch (Area2D)
├── CollisionShape2D (RectangleShape2D, size=8x4)
└── AnimatedSprite2D (two frames: up/down)
```

#### Gate

```csharp
public partial class Gate : StaticBody2D
```

| Field | Type | Default | Notes |
|-------|------|---------|-------|
| `IsOpen` | `bool` | `false` | Current state |

#### Function signatures

```csharp
public partial class Gate : StaticBody2D
```

| Signature | Description |
|-----------|-------------|
| `void OnSwitchToggled(bool isPressed)` | Set `IsOpen = isPressed`. Update collision layer (0 when open, original when closed). Update visibility. |
| `void _Ready()` | Find the switch via exported `NodePath`, connect to its `Toggled` signal. |

#### Scene layout

```
Gate (StaticBody2D)
├── CollisionShape2D (RectangleShape2D, size=8x16)
└── Sprite2D / AnimatedSprite2D (open/closed visuals)
```

---

### 2.12 PushableBox

A `RigidBody2D` that the player can push by walking into it.

#### Scene properties

| Property | Value |
|----------|-------|
| `GravityScale` | `1.0` |
| `Mass` | `5.0` |
| `Friction` | `0.8` |
| `Bounce` | `0.0` |
| `MaxContactsReported` | `1` |
| `LockRotation` | `true` |
| `ContinuousCD` | `true` |

#### Scene layout

```
PushableBox (RigidBody2D)
├── CollisionShape2D (RectangleShape2D, size=8x8)
└── Sprite2D (box texture from spritesheet)
```

No script needed — physics handle everything.

---

### 2.13 GiantBeetle

Mini-boss for Sprout Valley. Fought in a small arena at the end of the level.

#### Data structures

```csharp
public partial class GiantBeetle : CharacterBody2D
```

| Field | Type | Default | Notes |
|-------|------|---------|-------|
| `JumpSpeed` | `float` | `40.0` | 5 tiles/s horizontal jump speed |
| `JumpHeight` | `float` | `-74.0` | ~4 tiles high |
| `HitsToDefeat` | `int` | `3` | Stomps required |
| `TelegraphTime` | `float` | `0.5` | Seconds of telegraph before landing |
| `LandPauseTime` | `float` | `0.8` | Seconds paused after landing |
| `ArenaLeft` | `float` | (export) | Left bound of arena |
| `ArenaRight` | `float` | (export) | Right bound of arena |

Gravity is read from project settings.

#### Signals

| Signal | Parameters | Emitted when |
|--------|------------|--------------|
| `Defeated` | (none) | All hits consumed, boss defeated |
| `Hit` | `int hitsRemaining` | A successful stomp lands |

#### States

```
Idle ──[player enters arena]──> Jumping
Jumping ──[lands on ground]──> Telegraph
Telegraph ──[timer fires]──> Landing
Landing ──[timer fires]──> Jumping
Landing ──[stomped && falls]──> Hit
Hit ──[hits remaining > 0]──> Jumping
Hit ──[hits remaining == 0]──> Defeated
Defeated ──[animation finishes]──> DropCaveKey
```

#### Function signatures

```csharp
public partial class GiantBeetle : CharacterBody2D
```

| Signature | Description |
|-----------|-------------|
| `void _Ready()` | Cache references. Start in `Idle`. Connect `StompDetector` (same pattern as TurnipTrooper). |
| `void _PhysicsProcess(double delta)` | Apply gravity. Delegate to current state handler. Call `MoveAndSlide()`. |
| `void OnPlayerEnteredArena(Node2D body)` | If `body is PlayerController` and state is `Idle`, transition to `Jumping`. |
| `void OnStomped(Node2D body)` | If `body is PlayerController player && player.Velocity.Y > 0`: increment `_hitsTaken`. Emit `Hit`. If `_hitsTaken >= HitsToDefeat`, transition to `Defeated`. Else, transition to `Jumping`. |
| `void DropCaveKey()` | Instantiate a `CaveKey` scene at current position. |
| `void ChooseLandingSpot()` | Pick random X within `[ArenaLeft, ArenaRight]`. Set `Velocity.Y = JumpHeight`, horizontal velocity toward chosen X. |

#### Scene layout

```
GiantBeetle (CharacterBody2D)
├── CollisionShape2D (RectangleShape2D, size=12x10)
├── AnimatedSprite2D (requires art — idle, jump, telegraph, hit, defeat)
├── StompDetector (Area2D)
│   └── CollisionShape2D (RectangleShape2D, size=14x3, top)
└── ArenaDetector (Area2D)
    └── CollisionShape2D (RectangleShape2D, arena entry)
```

---

### 2.14 Input Map

Define the following actions in `Project > Input Map`:

| Action | Key Binding |
|--------|-------------|
| `move_left` | A / Left Arrow |
| `move_right` | D / Right Arrow |
| `jump` | W / Up Arrow / Space |
| `dash` | Shift / X |

---

### 2.15 Stomp priority

The `StompDetector` pattern is shared across `TurnipTrooper`, `AngryCarrot`, and `GiantBeetle`. To avoid activating both the enemy kill and player damage in the same frame:

- **Kill the enemy** if `player.Velocity.Y > 0` (player is falling onto the enemy).
- **Damage the player** if `player.Velocity.Y <= 0` (player is rising, standing, or moving laterally into the enemy).

The checks are mutually exclusive by velocity sign, so there is no race condition.

---

## 3. Data Model

### Directory layout

```
scenes/
├── bean-boy.tscn                  (PlayerController + Health)
├── main.tscn                      (entry scene)
├── levels/
│   └── sprout-valley.tscn         (replaces test.tscn)
├── enemies/
│   ├── turnip-trooper.tscn
│   ├── angry-carrot.tscn
│   └── giant-beetle.tscn
├── hazards/
│   ├── crumbling-block.tscn
│   ├── spike-ball.tscn
│   └── spikes.tscn                (scene tile for TileMapLayer)
├── collectibles/
│   ├── green-bean.tscn
│   └── cave-key.tscn
└── puzzles/
    ├── switch.tscn
    ├── gate.tscn
    └── pushable-box.tscn

scripts/
├── PlayerController.cs
├── Health.cs
├── GameState.cs
├── TurnipTrooper.cs
├── AngryCarrot.cs
├── GiantBeetle.cs
├── CrumblingBlock.cs
├── SpikeBall.cs
├── Spikes.cs
├── GreenBean.cs
├── CaveKey.cs
├── Switch.cs
└── Gate.cs

resources/
└── sprite-frames/
    ├── bean-boy.tres               (already exists)
    ├── crumbling-block.tres         (already exists)
    ├── spike-ball.tres              (already exists)
    └── ... (new sprite frames for enemies, collectibles, etc. as art is created)

├── tile-sets/
    └── sprout-valley.tres           (renamed/evolved from test.tres)

├── atlas-textures/
    ├── bean-boy/                   (already exists)
    ├── bean-can/                   (already exists)
    ├── bean-coin/                  (already exists)
    ├── crumbling-block/            (already exists)
    ├── flowers/                    (already exists)
    ├── grass/                      (already exists)
    ├── spike-ball/                 (already exists)
    ├── ... (new atlas textures for enemies, etc. as art is created)
```

### Scene file naming

- Snake case for scene files: `green-bean.tscn`, `crumbling-block.tscn`
- Pascal case for C# scripts: `PlayerController.cs`, `TurnipTrooper.cs`
- Scene root node name matches the Pascal name: `GreenBean`, `CrumblingBlock`

### Sprout Valley level layout

```
SproutValley (Node2D)
├── CentreMarker (Marker2D) — at (64, 64)
├── Sky (TileMapLayer) — z_index=-4
├── Ground (TileMapLayer) — z_index=-3
├── Decorations (TileMapLayer) — z_index=-2
├── Spikes (TileMapLayer) — z_index=-1 (scene tile: spikes.tscn)
├── Foreground (TileMapLayer) — z_index=1
├── PlayerStart (Marker2D) — spawn point
├── GreenBeans (GreenBean instances) — ~20 scattered
├── TurnipTroopers (TurnipTrooper instances) — 3-5 patrols
├── AngryCarrots (AngryCarrot instances) — 1-2 charge zones
├── CrumblingBlocks (CrumblingBlock instances) — 3-4 placements
├── SpikeBalls (SpikeBall instances) — 2 patrols
├── PuzzleArea
│   ├── Switch
│   ├── Gate
│   └── PushableBox
└── BossArena
    ├── GiantBeetle
    ├── ArenaDetector (Area2D trigger)
    └── Arena walls (Ground tiles)
```

---

## 5. Out of Scope

This specification does not cover:

- **Levels 2 and 3** (Root Caves, Root King's Fortress) — deferred to a future spec
- **Onion Ghost enemy** — Level 3 only
- **Mole Miner boss** — Level 2 only
- **Root King final boss** — Level 3 only
- **Dialogue system** — no NPC conversations in Sprout Valley
- **Main menu scene** — game starts directly in Sprout Valley for this slice
- **Music and sound effects** — no audio files or playback for this slice
- **Save/load system** — no persistence between sessions
- **Golden Beans / secret ending** — requires all 3 levels
- **Wall-jump** — only wall slide is implemented
- **Full biome tilesets** — only Sprout Valley tiles from the existing spritesheet are used
- **Enemy/collectible art assets** — this spec assumes art is created separately or placeholder rectangles are used. The scene hierarchies and scripts are designed to accept `AnimatedSprite2D` / `Sprite2D` when art is ready.
