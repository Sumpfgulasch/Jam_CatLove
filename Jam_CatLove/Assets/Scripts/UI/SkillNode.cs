using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Represents a single skill node in the skill tree
    /// </summary>
    [Serializable]
    public class SkillNode
    {
        [Header("Identification")]
        public string id;
        public string skillName;
        [TextArea(3, 5)]
        public string description;
        
        [Header("Visual")]
        public Sprite icon;
        public Color nodeColor = Color.white;
        
        [Header("Requirements")]
        public int requiredPoints = 1;
        public List<string> prerequisiteIds = new List<string>();
        
        [Header("Position")]
        public Vector2 position;
        
        [Header("State")]
        public bool isUnlocked = false;
        public int currentLevel = 0;
        public int maxLevel = 1;
        
        [Header("Tier")]
        public int tier = 0; // Used for organizing skills into tiers/rows
        
        public bool CanUnlock(int availablePoints, HashSet<string> unlockedSkills)
        {
            // Check if already at max level
            if (currentLevel >= maxLevel)
                return false;
            
            // Check if enough points
            if (availablePoints < requiredPoints)
                return false;
            
            // Check prerequisites
            foreach (var prereqId in prerequisiteIds)
            {
                if (!unlockedSkills.Contains(prereqId))
                    return false;
            }
            
            return true;
        }
        
        public bool HasPrerequisites()
        {
            return prerequisiteIds != null && prerequisiteIds.Count > 0;
        }
    }
}
