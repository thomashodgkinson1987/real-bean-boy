# BEAN BOY

## Game Design Document (GDD)

### Overview

**Title:** Bean Boy

**Genre:** 2D Platformer Adventure

**Engine:** Godot Engine

**Platform:** PC (Windows/Linux)

**Target Playtime:** 20–30 minutes

**Art Style:** Bright pixel art with whimsical vegetables and food-themed environments.

**Core Goal:**
Guide Bean Boy through three unique levels to find the legendary Golden Onion before it falls into the hands of the evil Root King.

---

# Story

Bean Boy is an ordinary bean living peacefully in Sprout Valley until a mysterious theft shakes the land.

The sacred Golden Onion, said to grant endless harvests, has vanished.

Following clues across forests, caves, and the Root King's fortress, Bean Boy must overcome obstacles, enemies, and puzzles to recover the Onion and save the valley.

---

# Core Gameplay

### Movement

* Move Left/Right
* Jump
* Double Jump
* Wall Slide

### Interactions

* Collect beans (currency/score)
* Push boxes
* Activate switches
* Open gates
* Collect hidden secrets

### Win Condition

Reach the Golden Onion at the end of Level 3.

---

# Player Character

## Bean Boy

### Abilities

#### Jump

Standard platformer jump.

#### Double Jump

Unlocked from the start to keep movement fun.

#### Wall Slide

Slows falling against walls.

#### Bean Dash

Short horizontal burst.

Cooldown: 2 seconds.

Useful for crossing gaps and dodging enemies.

---

# Collectibles

## Green Beans

Common collectible.

Used only for score.

Total in game: 100.

## Golden Bean

Rare hidden collectible.

Three total (one per level).

Collecting all three unlocks a special ending.

## Golden Onion

Final objective.

Located in Level 3.

---

# Enemies

## Turnip Trooper

Basic walking enemy.

* Patrols platforms
* Defeated by jumping on head

## Angry Carrot

Charges when player enters range.

* Faster movement
* Cannot climb ledges

## Onion Ghost

Appears in Level 3.

* Floats through walls
* Requires avoidance

---

# Levels

---

## Level 1: Sprout Valley

### Theme

Sunny farmland and gardens.

### Objective

Find the entrance to the underground cave.

### Features

* Introductory platforming
* Basic enemies
* Simple switch puzzle

### Hazards

* Small pits
* Rolling pumpkins

### Boss Encounter

Mini-boss:

**Giant Beetle**

* Jumps around arena
* Three hits to defeat

Reward:
Cave Key

---

## Level 2: Root Caves

### Theme

Underground tunnels filled with roots and glowing mushrooms.

### Objective

Locate the map leading to the Root King's fortress.

### Features

* Vertical platforming
* Moving platforms
* Hidden secret areas

### Hazards

* Falling rocks
* Spike roots
* Narrow passages

### Boss Encounter

**Mole Miner**

Attacks:

* Burrow charge
* Falling dirt attack

Defeat:
Jump on head three times.

Reward:
Fortress Map

---

## Level 3: Root King's Fortress

### Theme

Dark vegetable castle built from giant roots and stone.

### Objective

Recover the Golden Onion.

### Features

* Hardest platforming
* Multiple enemy combinations
* Timed gate sequence

### Hazards

* Spikes
* Lava soup pits
* Onion Ghost patrols

### Final Boss

## Root King

A giant onion-root monster.

### Phase 1

* Stomps ground
* Throws root spikes

### Phase 2

* Summons enemies
* Faster attacks

### Defeat Condition

Hit exposed onion core three times.

---

# Ending

## Normal Ending

Bean Boy defeats the Root King and returns the Golden Onion to Sprout Valley.

Celebration begins and harvests return.

---

## Secret Ending

Requirements:

* Collect all 3 Golden Beans

Additional scene:

The Golden Onion reveals a hidden golden garden where Bean Boy becomes the Guardian of Harvests.

---

# Art Requirements

## Player

* Idle
* Run
* Jump
* Double Jump
* Wall Slide
* Dash
* Victory

## Enemies

* Patrol animation
* Attack animation
* Defeat animation

## Environment

* Tilesets for all three biomes
* Decorative vegetables
* Animated plants
* Background layers

---

# Audio

## Music

Level 1:
Cheerful farm theme

Level 2:
Mystical cave theme

Level 3:
Dark fortress theme

Boss:
Fast-paced battle music

## Sound Effects

* Jump
* Dash
* Collect item
* Enemy defeat
* Switch activation
* Boss attacks
* Victory fanfare

---

# Technical Scope (Godot)

### Scenes

* Main Menu
* Level 1
* Level 2
* Level 3
* Boss Arena
* Ending Scene

### Systems

* Player Controller
* Enemy AI
* Health System
* Collectible System
* Save Progress
* Dialogue System
* Boss System

### Estimated Development Time

Solo Developer:
4–8 weeks

Team of 2:
2–4 weeks

Scope intentionally remains small and achievable while still delivering a complete adventure experience.

---

# Elevator Pitch

"Bean Boy is a charming 2D platformer where players jump, dash, and explore three handcrafted levels in search of the legendary Golden Onion, battling vegetable-themed enemies and the mighty Root King along the way."
