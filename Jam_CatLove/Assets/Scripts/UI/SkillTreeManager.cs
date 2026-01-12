using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    /// <summary>
    /// Manages the skill tree UI and interactions
    /// </summary>
    public class SkillTreeManager : MonoBehaviour
    {
        [Header("References")]
        public SkillTreeData treeData;
        public RectTransform skillNodesContainer;
        public GameObject tooltipPanel;
        public TextMeshProUGUI tooltipTitle;
        public TextMeshProUGUI tooltipDescription;
        public TextMeshProUGUI tooltipRequirements;
        public GameplayManager gameplayManager;
        
        [Header("Prefabs")]
        public GameObject skillNodePrefab;
        public GameObject connectionLinePrefab;
        
        private Dictionary<string, SkillNodeUI> _skillNodeUIs = new Dictionary<string, SkillNodeUI>();
        private HashSet<string> _unlockedSkills = new HashSet<string>();
        private List<GameObject> _connectionLines = new List<GameObject>();
        
        private void Start()
        {
            if (treeData != null)
            {
                BuildSkillTree();
                ResetSkillTree();
            }
            else
            {
                Debug.LogWarning("SkillTreeData is not assigned to SkillTreeManager!");
            }
            
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// Build the entire skill tree UI from data
        /// </summary>
        public void BuildSkillTree()
        {
            ClearSkillTree();
            
            if (treeData == null || skillNodesContainer == null)
            {
                Debug.LogError("Cannot build skill tree: missing data or container");
                return;
            }
            
            // Validate data
            if (!treeData.Validate(out string errorMessage))
            {
                Debug.LogError($"Skill tree data validation failed: {errorMessage}");
                return;
            }
            
            // Create skill nodes
            foreach (var skillData in treeData.skills)
            {
                CreateSkillNode(skillData);
            }
            
            // Create connection lines
            CreateConnectionLines();
            
            // Update all visuals
            UpdateAllNodeVisuals();
        }
        
        private void CreateSkillNode(SkillNode skillData)
        {
            if (skillNodePrefab == null)
            {
                Debug.LogError("Skill node prefab is not assigned!");
                return;
            }
            
            GameObject nodeObj = Instantiate(skillNodePrefab, skillNodesContainer);
            RectTransform rectTransform = nodeObj.GetComponent<RectTransform>();
            
            // Set position
            rectTransform.anchoredPosition = skillData.position;
            
            // Initialize the UI component
            SkillNodeUI nodeUI = nodeObj.GetComponent<SkillNodeUI>();
            if (nodeUI != null)
            {
                nodeUI.Initialize(skillData, this);
                _skillNodeUIs[skillData.id] = nodeUI;
            }
            else
            {
                Debug.LogError("SkillNodeUI component not found on prefab!");
            }
            
            nodeObj.name = $"SkillNode_{skillData.id}";
        }
        
        private void CreateConnectionLines()
        {
            if (connectionLinePrefab == null)
            {
                Debug.LogWarning("Connection line prefab is not assigned. Skipping line creation.");
                return;
            }
            
            foreach (var skill in treeData.skills)
            {
                if (!skill.HasPrerequisites()) continue;
                
                foreach (var prereqId in skill.prerequisiteIds)
                {
                    var prereqSkill = treeData.GetSkillById(prereqId);
                    if (prereqSkill == null) continue;
                    
                    CreateConnectionLine(prereqSkill.position, skill.position);
                }
            }
        }
        
        private void CreateConnectionLine(Vector2 startPos, Vector2 endPos)
        {
            GameObject lineObj = Instantiate(connectionLinePrefab, skillNodesContainer);
            lineObj.transform.SetAsFirstSibling(); // Draw behind nodes
            
            RectTransform rectTransform = lineObj.GetComponent<RectTransform>();
            Image lineImage = lineObj.GetComponent<Image>();
            
            if (lineImage != null)
            {
                lineImage.color = treeData.connectionLineColor;
            }
            
            // Calculate line position, rotation, and length
            Vector2 direction = endPos - startPos;
            float distance = direction.magnitude;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            rectTransform.anchoredPosition = startPos + direction * 0.5f;
            rectTransform.sizeDelta = new Vector2(distance, treeData.connectionLineWidth);
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);
            
            _connectionLines.Add(lineObj);
            lineObj.name = "ConnectionLine";
        }
        
        /// <summary>
        /// Clear all skill tree UI elements
        /// </summary>
        public void ClearSkillTree()
        {
            foreach (var nodeUI in _skillNodeUIs.Values)
            {
                if (nodeUI != null)
                    Destroy(nodeUI.gameObject);
            }
            _skillNodeUIs.Clear();
            
            foreach (var line in _connectionLines)
            {
                if (line != null)
                    Destroy(line);
            }
            _connectionLines.Clear();
        }
        
        /// <summary>
        /// Handle skill node click
        /// </summary>
        public void OnSkillNodeClicked(string skillId)
        {
            var skill = treeData.GetSkillById(skillId);
            if (skill == null) return;
            
            if (CanUnlockSkill(skillId))
            {
                UnlockSkill(skillId);
            }
            else
            {
                Debug.Log($"Cannot unlock skill: {skill.skillName}");
            }
        }
        
        /// <summary>
        /// Check if a skill can be unlocked
        /// </summary>
        public bool CanUnlockSkill(string skillId, bool ignorePoints = false)
        {
            var skill = treeData.GetSkillById(skillId);
            if (skill == null) 
                return false;
            
            return ignorePoints ? skill.CanUnlockIgnoringPoints(_unlockedSkills) : skill.CanUnlock(gameplayManager.heartsCount, _unlockedSkills);
        }
        
        /// <summary>
        /// Unlock a skill
        /// </summary>
        public void UnlockSkill(string skillId)
        {
            var skill = treeData.GetSkillById(skillId);
            if (skill == null || !CanUnlockSkill(skillId)) return;
            
            // Get the required points for the next level
            int requiredPoints = skill.GetRequiredPointsForNextLevel();
            
            // Spend points
            gameplayManager.heartsCount -= requiredPoints;
            
            // Unlock the skill
            skill.currentLevel++;
            if (skill.currentLevel > 0)
            {
                skill.isUnlocked = true;
                _unlockedSkills.Add(skillId);
            }
            
            // Apply the effect for this level
            var levelData = skill.GetCurrentLevel();
            if (levelData != null && levelData.effect != null)
            {
                levelData.effect.Apply(skill.currentLevel);
                Debug.Log($"Applied effect for {skill.skillName} level {skill.currentLevel}");
            }
            
            // Update visuals
            if (_skillNodeUIs.TryGetValue(skillId, out var nodeUI))
            {
                nodeUI.UpdateVisuals();
                nodeUI.PlayUnlockAnimation();
            }
            
            UpdateAllNodeVisuals();
            UpdateSkillPointsDisplay();
            
            Debug.Log($"Unlocked skill: {skill.skillName} (Level {skill.currentLevel}/{skill.maxLevel})");
        }
        
        /// <summary>
        /// Update all node visuals
        /// </summary>
        public void UpdateAllNodeVisuals()
        {
            foreach (var nodeUI in _skillNodeUIs.Values)
            {
                nodeUI.UpdateVisuals();
            }
        }
        
        /// <summary>
        /// Show tooltip for a skill
        /// </summary>
        public void ShowSkillTooltip(SkillNode skill, Vector3 nodePosition)
        {
            if (tooltipPanel == null) return;
            
            tooltipPanel.SetActive(true);
            
            // Update tooltip content
            if (tooltipTitle != null)
                tooltipTitle.text = skill.skillName;
            
            if (tooltipDescription != null)
                tooltipDescription.text = skill.description;
            
            if (tooltipRequirements != null)
            {
                string reqText = $"Level: {skill.currentLevel}/{skill.maxLevel}\n";
                
                // Show cost for next level if not maxed
                if (skill.currentLevel < skill.maxLevel)
                {
                    int nextLevelCost = skill.GetRequiredPointsForNextLevel();
                    reqText += $"Cost: {nextLevelCost} hearts\n";
                }
                else
                {
                    reqText += "MAX LEVEL\n";
                }
                
                if (skill.HasPrerequisites())
                {
                    reqText += "\nPrerequisites:\n";
                    foreach (var prereqId in skill.prerequisiteIds)
                    {
                        var prereq = treeData.GetSkillById(prereqId);
                        if (prereq != null)
                        {
                            bool isUnlocked = _unlockedSkills.Contains(prereqId);
                            reqText += $"  {(isUnlocked ? "✓" : "✗")} {prereq.skillName}\n";
                        }
                    }
                }
                
                tooltipRequirements.text = reqText;
            }
            
            // Position tooltip (simple positioning, can be improved)
            RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();
            if (tooltipRect != null)
            {
                tooltipRect.position = nodePosition + Vector3.right * 150f;
            }
        }
        
        /// <summary>
        /// Hide the skill tooltip
        /// </summary>
        public void HideSkillTooltip()
        {
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// Update skill points display
        /// </summary>
        private void UpdateSkillPointsDisplay()
        {
            var oldNumber = int.Parse(UIManager.Instance.heartsCountText.text);
            DoTweenUtils.PlayNumberCountAnimation(UIManager.Instance.heartsCountText, oldNumber, gameplayManager.heartsCount);
        }
        
        /// <summary>
        /// Reset all skills (for testing)
        /// </summary>
        public void ResetSkillTree()
        {
            foreach (var skill in treeData.skills)
            {
                // Remove all effects before resetting
                for (int i = skill.currentLevel; i > 0; i--)
                {
                    var levelData = skill.GetLevel(i - 1);
                    if (levelData != null && levelData.effect != null)
                    {
                        levelData.effect.Remove(i);
                    }
                }
                
                skill.isUnlocked = false;
                skill.currentLevel = 0;
            }
            
            _unlockedSkills.Clear();
            
            UpdateAllNodeVisuals();
            UpdateSkillPointsDisplay();
            
            Debug.Log("Skill tree reset");
        }
    }
}
