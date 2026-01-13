using UnityEngine;

namespace UI.Examples
{
    /// <summary>
    /// Example implementation of a skill effect.
    /// Create your own by inheriting from SkillLevelEffect and implementing Apply/Remove.
    /// To create in Unity: Right-click in Project > Create > Skills > Example Skill Effect
    /// </summary>
    [CreateAssetMenu(fileName = "NewCatLove1SkillEffect", menuName = "Skills/Cat Love 1 Skill Effect")]
    public class CatLove1SkillEffect : SkillLevelEffect
    {
        public float pettingMultiplier = 1.1f;
        
        public override void Apply(int level)
        {
            PlayerSkillTreeState.Instance.PettingMultiplier *= pettingMultiplier;
            var meowSpawnRate = 1 + ((1 - PlayerSkillTreeState.Instance.PettingMultiplier) / 2f);
            AudioManager.Instance.SetGlobalParameter(FmodParameter.MEOW_SPAWN_RATE, meowSpawnRate);
        }
        
        public override void Remove(int level)
        {
            PlayerSkillTreeState.Instance.PettingMultiplier = 1f;
        }
    }
}
