using UnityEngine;

namespace UI
{
    /// <summary>
    /// Abstract base class for skill level effects.
    /// Create concrete implementations as ScriptableObjects to define specific skill behaviors.
    /// </summary>
    public abstract class SkillLevelEffect : ScriptableObject
    {
        [TextArea(2, 4)]
        public string effectDescription;
        
        /// <summary>
        /// Apply the effect when the skill level is unlocked
        /// </summary>
        /// <param name="level">The level being applied (1-based)</param>
        public abstract void Apply(int level);
        
        /// <summary>
        /// Remove the effect when the skill is reset or downgraded
        /// </summary>
        /// <param name="level">The level being removed (1-based)</param>
        public abstract void Remove(int level);
    }
}
