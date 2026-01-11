using UnityEngine;

namespace UI.Examples
{
    /// <summary>
    /// Example implementation of a skill effect.
    /// Create your own by inheriting from SkillLevelEffect and implementing Apply/Remove.
    /// To create in Unity: Right-click in Project > Create > Skills > Example Skill Effect
    /// </summary>
    [CreateAssetMenu(fileName = "NewExampleSkillEffect", menuName = "Skills/Example Skill Effect")]
    public class ExampleSkillEffect : SkillLevelEffect
    {
        [Header("Example Settings")]
        public float damageMultiplier = 1.1f;
        public int healthBonus = 10;
        
        public override void Apply(int level)
        {
            Debug.Log($"[ExampleSkillEffect] Applying effect at level {level}");
            Debug.Log($"  - Damage multiplier: {damageMultiplier}");
            Debug.Log($"  - Health bonus: {healthBonus}");
            
            // TODO: Implement your actual game logic here
            // Example: GameManager.Instance.player.damageMultiplier *= damageMultiplier;
            // Example: GameManager.Instance.player.maxHealth += healthBonus;
        }
        
        public override void Remove(int level)
        {
            Debug.Log($"[ExampleSkillEffect] Removing effect at level {level}");
            
            // TODO: Implement your actual game logic here to reverse the effect
            // Example: GameManager.Instance.player.damageMultiplier /= damageMultiplier;
            // Example: GameManager.Instance.player.maxHealth -= healthBonus;
        }
    }
}
