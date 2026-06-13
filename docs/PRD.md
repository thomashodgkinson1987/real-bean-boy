# BEAN BOY — Product Requirements Document

## Problem Statement

The repository is a bare Godot 4.6.3 C# project with a test level and a player character scene, but the game has no gameplay. The player cannot move, jump, dash, or interact with anything. No enemies, collectibles, hazards, or level progression exist. The GDD describes a complete 3-level platformer, but no game systems have been implemented yet.

## Solution

Build a playable vertical slice of Bean Boy — enough to prove the core loop of movement, combat, and collection across a single representative level (Sprout Valley). This slice will contain:

- A complete **player controller** with jump, double jump, wall slide, and bean dash
- **Green Bean** collectibles scattered through the level
- **Turnip Trooper** enemies that patrol and can be stomped
- An **Angry Carrot** enemy that charges on sight
- A **Giant Beetle** mini-boss fight with a 3-hit defeat
- A **switch/gate** puzzle sequence
- A **Crumbling Block** hazard
- A **Spike Ball** hazard
- **Spike** floor hazards
- A **pushable box** for simple puzzles
- The **Cave Key** reward as a visible endpoint

All within a properly designed Level 1 (Sprout Valley) scene.

## Implementation Decisions

### Player Controller
A single C# script on the `CharacterBody2D` scene. The controller reads `Input.get_axis` for horizontal movement and `Input.is_action_just_pressed` for jump/dash. No state machine library — a simple enum-based FSM with states: `OnGround, Jumping, Falling, DoubleJumping, WallSliding, Dashing`. The dash has a 2-second cooldown tracked by a `Timer` node child.

### Input Map
Define Godot Input Map actions: `move_left`, `move_right`, `jump`, `dash`. This keeps input rebindable and cleanly separated from controller logic.

### Scene Hierarchy Per Level
A level scene groups layers (Sky, Ground, Decorations, Foreground) under a root `Node2D`. Interactive elements (enemies, collectibles, switches, gates, the player start) are direct children of the level root. The existing `test.tscn` will be replaced/renamed to `levels/sprout-valley.tscn`.

### Collectibles
Green Beans and Golden Beans are `Area2D` scenes with a `Collectible` C# script that emits a signal on body-entered and frees itself. A `GameState` autoload (static `Node` singleton) tracks bean count, golden bean count, and level progression.

### Enemies
Turnip Trooper: `CharacterBody2D` with `AnimatedSprite2D` and `CollisionShape2D`. Simple patrol AI that reverses direction at ledge edges or walls. Has a `StompDetector` Area2D on top to detect player landing on its head. On stomp, plays a squash animation and queues free. On touching the player from the side, damages the player.

Angry Carrot: Similar structure but idle until the player enters a detection `Area2D`, then charges horizontally toward the player. Cannot climb ledges — reverses at walls.

### Damage / Health
Player has 3 health points (HP). A `Health` C# component tracks HP, invincibility frames (1-second `Timer` after hit), and emits `damaged` and `died` signals. On death, the level reloads. Enemies deal 1 HP per contact. Spikes also deal 1 HP.

### Switch/Gate Puzzle
`Switch` (`Area2D`, animated) and `Gate` (`StaticBody2D` or `TileMapLayer`-based) communicate via signals. A switch exposes a `toggled(bool)` signal. Gates listen and toggle their collision layer / visibility. Both are designed as reusable scenes.

### Crumbling Block
A `StaticBody2D` using the `crumbling-block` `SpriteFrames`. When the player stands on it, a short timer starts, then plays the `crumble` animation and frees itself. Respawns on level reload.

### Pushable Box
A `RigidBody2D` with a small collision shape and friction so the player can push it by walking into it.

### Giant Beetle Mini-Boss
A dedicated `CharacterBody2D` scene that jumps around an arena in a pattern (choosing a random landing spot each jump, with a brief telegraph). Player must jump on its head 3 times. Tracks hit count internally. On defeat, drops the Cave Key (a `Collectible`-like scene).

### Level End
The Cave Key pickup sets a flag on `GameState` and transitions to a "Level Complete" overlay or the next scene (placeholder for now).

## Out of Scope

- Levels 2 and 3 (Root Caves, Root King's Fortress)
- Onion Ghost enemy
- Mole Miner and Root King bosses
- Dialogue system
- Main menu scene
- Music and sound effects (placeholder only — can add `.import`-ready files later)
- Save/load system
- Secret ending / Golden Beans
- Wall-jump (only wall slide)
- All 3 biome tilesets — Sprout Valley tiles only

## Further Notes

The existing sprite atlas has already been sliced for Turnip Trooper assets? Let me check what's on the spritesheet — the current `test.tres` tile set references tiles on rows 0–3 of the 8x8 grid. The spritesheet (64x32 = 8 cols × 4 rows) only has 32 tiles. The bean-boy player occupies rows 0–1, and the remaining tiles are environment tiles. Enemy sprites, collectible beans, and the Giant Beetle do not exist on the current spritesheet yet. Art asset creation or extension of the spritesheet is a prerequisite for several of these items.
