# Skill Tree System Documentation

## Overview
A complete skill tree implementation with visual UI, node connections, prerequisites, and progression tracking.

## Features
- ✅ Visual skill tree with nodes and connection lines
- ✅ Prerequisite system (skills can require other skills)
- ✅ Multi-level skills (skills can have multiple upgrade levels)
- ✅ Skill point system
- ✅ Interactive tooltips showing skill details
- ✅ Visual feedback (hover effects, unlock animations)
- ✅ Color-coded states (locked, available, unlocked, max level)
- ✅ Scrollable skill tree view
- ✅ Editor tool for easy UI setup

## Quick Start

### 1. Build the UI
1. Open Unity Editor
2. Go to `Tools > Skill Tree > Build UI`
3. Drag your **SkillTreePanel** GameObject into the field
4. Click **"Build Complete Skill Tree UI"**
5. Click **"Create Sample Skill Tree Data"** to generate example data

### 2. Assign the Data
1. Find the newly created `SampleSkillTreeData.asset` in `Assets/Data/`
2. Select your **SkillTreePanel** GameObject in the scene
3. In the **SkillTreeManager** component, assign the `SampleSkillTreeData` to the **Tree Data** field

### 3. Test in Play Mode
- Click on skill nodes to unlock them
- Hover over nodes to see tooltips
- Skills will be color-coded based on their state

## Components

### SkillNode (Data Class)
Represents a single skill in the tree.

**Key Properties:**
- `id` - Unique identifier
- `skillName` - Display name
- `description` - Skill description
- `icon` - Visual icon sprite
- `requiredPoints` - Cost to unlock
- `prerequisiteIds` - List of required skill IDs
- `position` - Position in the tree
- `tier` - Organizational tier/row
- `maxLevel` - Maximum upgrade level
- `currentLevel` - Current upgrade level

### SkillTreeData (ScriptableObject)
Container for all skill tree configuration.

**Create New:** Right-click in Project > `Create > Game > Skill Tree Data`

**Key Properties:**
- `skills` - List of all skills
- `nodeSpacing` - Horizontal spacing between nodes
- `tierSpacing` - Vertical spacing between tiers
- `connectionLineColor` - Color of connection lines
- `defaultSkillIcon` - Fallback icon

### SkillNodeUI (MonoBehaviour)
UI component for individual skill nodes.

**Features:**
- Hover scale animation
- Click handling
- Visual state updates
- Unlock animation

### SkillTreeManager (MonoBehaviour)
Main controller for the skill tree system.

**Key Methods:**
- `BuildSkillTree()` - Generates UI from data
- `UnlockSkill(string id)` - Unlocks a skill
- `CanUnlockSkill(string id)` - Checks if skill can be unlocked
- `ResetSkillTree()` - Resets all progress (for testing)

**Public Properties:**
- `availableSkillPoints` - Current skill points available

## Creating Custom Skill Trees

### Method 1: Using the Editor
1. Create a new SkillTreeData asset: `Create > Game > Skill Tree Data`
2. Add skills to the list
3. Configure each skill's properties
4. Set positions manually or use a grid pattern
5. Assign prerequisite IDs to create dependencies

### Method 2: Programmatically
```csharp
SkillTreeData data = ScriptableObject.CreateInstance<SkillTreeData>();

SkillNode skill1 = new SkillNode
{
    id = "skill_001",
    skillName = "Basic Attack",
    description = "Increases attack damage",
    position = new Vector2(0, 200),
    tier = 0,
    requiredPoints = 1,
    maxLevel = 3
};

SkillNode skill2 = new SkillNode
{
    id = "skill_002",
    skillName = "Power Strike",
    description = "Unlocks special attack",
    position = new Vector2(0, 0),
    tier = 1,
    requiredPoints = 2,
    maxLevel = 1,
    prerequisiteIds = new List<string> { "skill_001" }
};

data.skills.Add(skill1);
data.skills.Add(skill2);
```

## Visual States

### Node Colors
- **Gray** - Locked (prerequisites not met)
- **Yellow** - Available (can be unlocked)
- **White** - Unlocked (active)
- **Green** - Max Level (fully upgraded)

### Custom Colors
Set `nodeColor` on individual skills to apply custom tinting.

## Positioning Guide

### Recommended Layout
- **Tier 0 (Starting):** Y = 200
- **Tier 1 (Mid):** Y = 0
- **Tier 2 (Advanced):** Y = -200
- **Horizontal Spacing:** 150-200 units between nodes

### Example Layout
```
Tier 0:  [-200, 200]  [0, 200]  [200, 200]
Tier 1:  [-200, 0]    [0, 0]    [200, 0]
Tier 2:  [-100, -200] [100, -200]
```

## Integration with Game Logic

### Listening to Skill Unlocks
Modify [`SkillTreeManager.UnlockSkill()`](SkillTreeManager.cs:165) to trigger game events:

```csharp
public void UnlockSkill(string skillId)
{
    // ... existing code ...
    
    // Add your game logic here
    OnSkillUnlocked?.Invoke(skillId);
    ApplySkillEffect(skillId);
}
```

### Saving/Loading Progress
Store the following data:
- `availableSkillPoints`
- For each skill: `currentLevel`, `isUnlocked`

Example:
```csharp
[System.Serializable]
public class SkillTreeSaveData
{
    public int skillPoints;
    public Dictionary<string, int> skillLevels;
}

public SkillTreeSaveData GetSaveData()
{
    var data = new SkillTreeSaveData
    {
        skillPoints = availableSkillPoints,
        skillLevels = new Dictionary<string, int>()
    };
    
    foreach (var skill in treeData.skills)
    {
        data.skillLevels[skill.id] = skill.currentLevel;
    }
    
    return data;
}
```

## Customization

### Changing Prefabs
The editor creates two prefabs:
- `Assets/Prefabs/UI/SkillNodePrefab.prefab`
- `Assets/Prefabs/UI/ConnectionLinePrefab.prefab`

You can modify these prefabs to change the visual appearance.

### Tooltip Positioning
Modify [`SkillTreeManager.ShowSkillTooltip()`](SkillTreeManager.cs:234) to change tooltip behavior.

### Animation
Modify [`SkillNodeUI.PlayUnlockAnimation()`](SkillNodeUI.cs:133) to customize unlock effects.

## Troubleshooting

### Nodes Not Appearing
- Ensure SkillTreeData is assigned to SkillTreeManager
- Check that skill positions are within the content area
- Verify prefabs are assigned

### Tooltips Not Showing
- Check that tooltip panel references are assigned
- Ensure EventSystem exists in scene

### Skills Not Unlocking
- Verify prerequisite IDs match exactly
- Check available skill points
- Ensure skill IDs are unique

## API Reference

### SkillNode Methods
- `CanUnlock(int points, HashSet<string> unlocked)` - Check if unlockable
- `HasPrerequisites()` - Check if has prerequisites

### SkillTreeManager Methods
- `BuildSkillTree()` - Build UI from data
- `ClearSkillTree()` - Remove all UI elements
- `OnSkillNodeClicked(string id)` - Handle node click
- `CanUnlockSkill(string id)` - Check unlock eligibility
- `UnlockSkill(string id)` - Unlock a skill
- `UpdateAllNodeVisuals()` - Refresh all node visuals
- `ShowSkillTooltip(SkillNode, Vector3)` - Display tooltip
- `HideSkillTooltip()` - Hide tooltip
- `ResetSkillTree()` - Reset all progress

## File Structure
```
Assets/
├── Scripts/
│   ├── UI/
│   │   ├── SkillNode.cs
│   │   ├── SkillTreeData.cs
│   │   ├── SkillNodeUI.cs
│   │   ├── SkillTreeManager.cs
│   │   └── SkillTree_README.md
│   └── Editor/
│       └── SkillTreeBuilder.cs
├── Prefabs/
│   └── UI/
│       ├── SkillNodePrefab.prefab
│       └── ConnectionLinePrefab.prefab
└── Data/
    └── SampleSkillTreeData.asset
```

## Notes
- The system is designed to be game-logic independent
- All skill effects must be implemented separately
- The UI is fully functional but not connected to gameplay systems
- Skill points must be managed by your game's progression system
