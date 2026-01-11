using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace UI
{
    /// <summary>
    /// UI component for a single skill node in the skill tree
    /// </summary>
    public class SkillNodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("References")]
        public Image iconImage;
        public Image backgroundImage;
        public Image lockOverlay;
        public TextMeshProUGUI levelText;
        public Button button;
        
        [Header("Visual States")]
        public Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        public Color unlockedColor = Color.white;
        public Color availableColor = new Color(1f, 1f, 0.5f, 1f);
        public Color maxLevelColor = new Color(0.5f, 1f, 0.5f, 1f);
        public Color hoverScale = new Color(1.1f, 1.1f, 1.1f, 1f);
        
        [Header("Animation")]
        public float hoverScaleFactor = 1.1f;
        public float scaleAnimationSpeed = 10f;
        
        private SkillNode _skillData;
        private SkillTreeManager _treeManager;
        private Vector3 _originalScale;
        private Vector3 _targetScale;
        private bool _isHovering;
        
        public SkillNode SkillData => _skillData;
        
        private void Awake()
        {
            _originalScale = transform.localScale;
            _targetScale = _originalScale;
            
            if (button == null)
                button = GetComponent<Button>();
        }
        
        private void Update()
        {
            // Smooth scale animation
            if (transform.localScale != _targetScale)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.deltaTime * scaleAnimationSpeed);
            }
        }
        
        public void Initialize(SkillNode skillData, SkillTreeManager treeManager)
        {
            _skillData = skillData;
            _treeManager = treeManager;
            
            UpdateVisuals();
        }
        
        public void UpdateVisuals()
        {
            if (_skillData == null) return;
            
            // Update icon
            if (iconImage != null && _skillData.icon != null)
            {
                iconImage.sprite = _skillData.icon;
            }
            
            // Update level text
            if (levelText != null)
            {
                if (_skillData.maxLevel > 1)
                {
                    levelText.text = $"{_skillData.currentLevel}/{_skillData.maxLevel}";
                    levelText.gameObject.SetActive(true);
                }
                else
                {
                    levelText.gameObject.SetActive(_skillData.isUnlocked);
                    levelText.text = _skillData.isUnlocked ? "✓" : "";
                }
            }
            
            // Update colors based on state
            UpdateColorState();
            
            // Update lock overlay
            if (lockOverlay != null)
            {
                lockOverlay.gameObject.SetActive(!_skillData.isUnlocked && _skillData.currentLevel == 0);
            }
        }
        
        private void UpdateColorState()
        {
            if (backgroundImage == null) return;
            
            Color targetColor;
            
            if (_skillData.currentLevel >= _skillData.maxLevel)
            {
                // Max level
                targetColor = maxLevelColor;
            }
            else if (_skillData.isUnlocked || _skillData.currentLevel > 0)
            {
                // Unlocked but not max level
                targetColor = unlockedColor;
            }
            else if (_treeManager != null && _treeManager.CanUnlockSkill(_skillData.id))
            {
                // Available to unlock
                targetColor = availableColor;
            }
            else
            {
                // Locked
                targetColor = lockedColor;
            }
            
            // Apply node's custom color tint
            backgroundImage.color = targetColor * _skillData.nodeColor;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;
            _targetScale = _originalScale * hoverScaleFactor;
            
            if (_treeManager != null)
            {
                _treeManager.ShowSkillTooltip(_skillData, transform.position);
            }
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;
            _targetScale = _originalScale;
            
            if (_treeManager != null)
            {
                _treeManager.HideSkillTooltip();
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_treeManager != null)
            {
                _treeManager.OnSkillNodeClicked(_skillData.id);
            }
        }
        
        /// <summary>
        /// Play unlock animation
        /// </summary>
        public void PlayUnlockAnimation()
        {
            // Simple pulse animation
            StartCoroutine(PulseAnimation());
        }
        
        private System.Collections.IEnumerator PulseAnimation()
        {
            float duration = 0.5f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float scale = 1f + Mathf.Sin(elapsed / duration * Mathf.PI) * 0.3f;
                transform.localScale = _originalScale * scale;
                yield return null;
            }
            
            transform.localScale = _originalScale;
        }
    }
}
