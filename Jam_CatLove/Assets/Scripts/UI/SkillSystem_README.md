# Skill System Documentation

## Overview
The skill system has been extended to support multiple levels per skill with individual costs and effects for each level.

## Core Components

### 1. SkillNode
The main skill data structure. Each skill now contains:
- **levels**: Array of `SkillLevel` objects defining each level's cost and effect
- **maxLevel**: Automatically derived from the `levels` array length
- **currentLevel**: Current unlocked level (0 = not unlocked)

### 2. SkillLevel
Defines a single level of a skill:
- **requiredPoints**: Points needed to unlock this specific level
- **effect**: Reference to a `SkillLevelEffect` ScriptableObject
- **levelDescription**: Optional description for this level

### 3. SkillLevelEffect (Abstract)
Base class for all skill effects. Must be implemented as ScriptableObjects.
- **Apply(int level)**: Called when the level is unlocked
- **Remove(int level)**: Called when the skill is reset or downgraded
- **effectDescription**: Text description of what the effect does

### 4. SkillTreeManager
Manages the skill tree and handles:
- Unlocking skills and applying effects
- Resetting skills and removing effects
- Displaying tooltips with per-level costs
- Validating prerequisites

## How to Use

### Creating a Skill Effect

1. Create a new C# script inheriting from `SkillLevelEffect`:

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "NewDamageBoost", menuName = "Skills/Damage Boost Effect")]
public class DamageBoostEffect : SkillLevelEffect
{
    public float damageIncrease = 0.15f;
    
    public override void Apply(int level)
    {
        // Apply the effect to your game
        PlayerStats.Instance.damageMultiplier += damageIncrease;
        Debug.Log($"Damage boost applied! Level {level}");
    }
    
    public override void Remove(int level)
    {
        // Remove the effect
        PlayerStats.Instance.damageMultiplier -= damageIncrease;
        Debug.Log($"Damage boost removed! Level {level}");
    }
}
```

2. In Unity Editor:
   - Right-click in Project window
   - Create > Skills > [Your Effect Name]
   - Configure the effect parameters

### Setting Up a Multi-Level Skill

1. In your `SkillTreeData` asset, select a skill
2. Set the **levels** array size (e.g., 3 for a 3-level skill)
3. For each level:
   - Set **requiredPoints** (e.g., 1, 2, 3 for increasing costs)
   - Assign a **SkillLevelEffect** ScriptableObject
   - Optionally add a **levelDescription**

Example configuration:
```
Skill: "Power Strike"
├─ Level 1: 1 point  → +10% damage
├─ Level 2: 2 points → +20% damage  
└─ Level 3: 3 points → +30% damage
```

## API Reference

### SkillNode Methods

- `int GetRequiredPointsForNextLevel()` - Returns cost for the next level
- `SkillLevel GetLevel(int levelIndex)` - Gets level data by index (0-based)
- `SkillLevel GetCurrentLevel()` - Gets the currently unlocked level data
- `bool CanUnlock(int availablePoints, HashSet<string> unlockedSkills)` - Checks if next level can be unlocked

### SkillTreeManager Methods

- `void UnlockSkill(string skillId)` - Unlocks the next level of a skill and applies its effect
- `void ResetSkillTree()` - Resets all skills and removes all effects
- `bool CanUnlockSkill(string skillId)` - Checks if a skill's next level can be unlocked

## Migration from Old System

The old system used:
- `int requiredPoints` - Single cost value
- `int maxLevel` - Manually set max level

The new system uses:
- `SkillLevel[] levels` - Array defining each level
- `int maxLevel` (property) - Automatically derived from array length

To migrate existing skills:
1. Create a `levels` array with size equal to old `maxLevel`
2. Set each level's `requiredPoints` to the old `requiredPoints` value
3. Create and assign appropriate `SkillLevelEffect` assets

## Example: Complete Skill Setup

```csharp
// 1. Create the effect ScriptableObject
[CreateAssetMenu(menuName = "Skills/Health Boost")]
public class HealthBoostEffect : SkillLevelEffect
{
    public int healthIncrease = 20;
    
    public override void Apply(int level)
    {
        PlayerStats.Instance.maxHealth += healthIncrease;
        PlayerStats.Instance.currentHealth += healthIncrease;
    }
    
    public override void Remove(int level)
    {
        PlayerStats.Instance.maxHealth -= healthIncrease;
    }
}

// 2. In Unity Editor:
// - Create 3 HealthBoostEffect assets with different healthIncrease values
// - Assign them to a skill's levels array
// - Set requiredPoints for each level (e.g., 1, 2, 3)

// 3. The skill will now have 3 levels with different costs and effects!
```

## Notes

- Effects are applied immediately when a level is unlocked
- Effects are removed in reverse order when resetting
- Each level can have a different effect or the same effect with different parameters
- The `level` parameter in Apply/Remove is 1-based (level 1, 2, 3, etc.)
