using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

[CreateAssetMenu(fileName = "ZonesSkillEffect", menuName = "Skills/ZonesSkillEffect")]
public class ZonesSkillEffect : SkillLevelEffect
{
    public List<string> newCatZones;
    
    public override void Apply(int level)
    {
        GameplayManager.Instance.Cat.Zones.UnlockZones(newCatZones);
    }

    public override void Remove(int level)
    {
        GameplayManager.Instance.Cat.Zones.LockZones(newCatZones);
    }
}
