using System;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Represents a single level of a skill with its cost and effect
    /// </summary>
    [Serializable]
    public class SkillLevel
    {
        [Tooltip("Points required to unlock this level")]
        public int requiredPoints = 1;
        
        [Tooltip("Effect applied when this level is unlocked")]
        public SkillLevelEffect effect;
        
        [TextArea(2, 3)]
        [Tooltip("Description of what this level does (optional, can also use effect description)")]
        public string levelDescription;
    }
}
