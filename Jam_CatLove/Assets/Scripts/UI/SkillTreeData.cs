using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// ScriptableObject that holds all skill tree configuration data
    /// </summary>
    [CreateAssetMenu(fileName = "SkillTreeData", menuName = "Game/Skill Tree Data")]
    public class SkillTreeData : ScriptableObject
    {
        [Header("Skill Tree Configuration")]
        public string treeName = "Skill Tree";
        public List<SkillNode> skills = new List<SkillNode>();
        
        [Header("Visual Settings")]
        public float nodeSpacing = 150f;
        public float tierSpacing = 200f;
        public Color connectionLineColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        public float connectionLineWidth = 3f;
        
        [Header("Default Icons")]
        public Sprite defaultSkillIcon;
        public Sprite lockedOverlayIcon;
        
        /// <summary>
        /// Get a skill by its ID
        /// </summary>
        public SkillNode GetSkillById(string id)
        {
            return skills.Find(s => s.id == id);
        }
        
        /// <summary>
        /// Get all skills in a specific tier
        /// </summary>
        public List<SkillNode> GetSkillsByTier(int tier)
        {
            return skills.FindAll(s => s.tier == tier);
        }
        
        /// <summary>
        /// Validate the skill tree data
        /// </summary>
        public bool Validate(out string errorMessage)
        {
            errorMessage = "";
            
            // Check for duplicate IDs
            HashSet<string> ids = new HashSet<string>();
            foreach (var skill in skills)
            {
                if (string.IsNullOrEmpty(skill.id))
                {
                    errorMessage = $"Skill '{skill.skillName}' has no ID";
                    return false;
                }
                
                if (ids.Contains(skill.id))
                {
                    errorMessage = $"Duplicate skill ID: {skill.id}";
                    return false;
                }
                ids.Add(skill.id);
            }
            
            // Check that all prerequisites exist
            foreach (var skill in skills)
            {
                foreach (var prereqId in skill.prerequisiteIds)
                {
                    if (!ids.Contains(prereqId))
                    {
                        errorMessage = $"Skill '{skill.skillName}' has invalid prerequisite ID: {prereqId}";
                        return false;
                    }
                }
            }
            
            return true;
        }
    }
}
