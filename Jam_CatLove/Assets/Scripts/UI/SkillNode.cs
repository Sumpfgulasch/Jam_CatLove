using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Represents a single skill node in the skill tree
    /// </summary>
    [Serializable]
    public class SkillNode
    {
        [Header("Identification")] public string id;
        public string skillName;
        [TextArea(3, 5)] public string description;

        [Header("Visual")] public Sprite icon;
        public Color nodeColor = Color.white;

        [Header("Levels")] [Tooltip("Define each level of this skill. Array length determines maxLevel.")]
        public SkillLevel[] levels = new SkillLevel[] { new SkillLevel() };

        [Header("Requirements")] public List<string> prerequisiteIds = new List<string>();

        [Header("Position")] public Vector2 position;

        [Header("State")] public bool isUnlocked = false;
        public int currentLevel = 0;

        [Header("Tier")] public int tier = 0; // Used for organizing skills into tiers/rows

        /// <summary>
        /// Maximum level is determined by the length of levels array
        /// </summary>
        public int maxLevel => levels != null && levels.Length > 0 ? levels.Length : 1;

        /// <summary>
        /// Get the required points for the next level
        /// </summary>
        public int GetRequiredPointsForNextLevel()
        {
            if (currentLevel >= maxLevel)
                return 0;

            // Ensure array is valid
            if (levels == null || levels.Length == 0)
                return 1; // Default fallback

            // Get the cost for the current level (which is the next level to unlock)
            return levels[currentLevel].requiredPoints;
        }

        /// <summary>
        /// Get the SkillLevel data for a specific level (0-based index)
        /// </summary>
        public SkillLevel GetLevel(int levelIndex)
        {
            if (levels == null || levelIndex < 0 || levelIndex >= levels.Length)
                return null;

            return levels[levelIndex];
        }

        /// <summary>
        /// Get the current level's data
        /// </summary>
        public SkillLevel GetCurrentLevel()
        {
            if (currentLevel <= 0)
                return null;

            return GetLevel(currentLevel - 1);
        }

        public bool CanUnlock(int availablePoints, HashSet<string> unlockedSkills)
        {
            int requiredPoints = GetRequiredPointsForNextLevel();
            return availablePoints >= requiredPoints && CanUnlockIgnoringPoints(unlockedSkills);
        }

        public bool CanUnlockIgnoringPoints(HashSet<string> unlockedSkills)
        {
            return currentLevel < maxLevel && ArePrerequisitesMet(unlockedSkills);
        }

        private bool ArePrerequisitesMet(HashSet<string> unlockedSkills)
        {
            return prerequisiteIds.All(unlockedSkills.Contains);
        }

        public bool HasPrerequisites()
        {
            return prerequisiteIds != null && prerequisiteIds.Count > 0;
        }
    }
}