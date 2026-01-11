using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UI;

namespace Editor
{
    /// <summary>
    /// Editor tool to build the skill tree UI in the scene
    /// </summary>
    public class SkillTreeBuilder : EditorWindow
    {
        private GameObject skillTreePanel;
        
        [MenuItem("Tools/Skill Tree/Build UI")]
        public static void ShowWindow()
        {
            GetWindow<SkillTreeBuilder>("Skill Tree Builder");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Skill Tree UI Builder", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            skillTreePanel = (GameObject)EditorGUILayout.ObjectField("Skill Tree Panel", skillTreePanel, typeof(GameObject), true);
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Build Complete Skill Tree UI", GUILayout.Height(40)))
            {
                BuildSkillTreeUI();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Create Sample Skill Tree Data", GUILayout.Height(30)))
            {
                CreateSampleSkillTreeData();
            }
            
            GUILayout.Space(20);
            GUILayout.Label("Instructions:", EditorStyles.boldLabel);
            GUILayout.Label("1. Select the SkillTreePanel GameObject", EditorStyles.wordWrappedLabel);
            GUILayout.Label("2. Click 'Build Complete Skill Tree UI'", EditorStyles.wordWrappedLabel);
            GUILayout.Label("3. Create sample data if needed", EditorStyles.wordWrappedLabel);
        }
        
        private void BuildSkillTreeUI()
        {
            if (skillTreePanel == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign the SkillTreePanel GameObject!", "OK");
                return;
            }
            
            // Clear existing children
            while (skillTreePanel.transform.childCount > 0)
            {
                DestroyImmediate(skillTreePanel.transform.GetChild(0).gameObject);
            }
            
            // Note: SkillTreePanel should be a child of an existing Canvas
            // We don't add Canvas components here
            
            // Create main container
            GameObject mainContainer = CreateUIObject("MainContainer", skillTreePanel.transform);
            RectTransform mainRect = mainContainer.GetComponent<RectTransform>();
            mainRect.anchorMin = Vector2.zero;
            mainRect.anchorMax = Vector2.one;
            mainRect.sizeDelta = Vector2.zero;
            
            Image mainBg = mainContainer.AddComponent<Image>();
            mainBg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
            
            // Create header
            GameObject header = CreateHeader(mainContainer.transform);
            
            // Create scroll view for skill tree
            GameObject scrollView = CreateScrollView(mainContainer.transform);
            
            // Create skill nodes container inside scroll view
            GameObject content = scrollView.transform.Find("Viewport/Content").gameObject;
            content.name = "SkillNodesContainer";
            
            // Create tooltip panel
            GameObject tooltip = CreateTooltipPanel(mainContainer.transform);
            
            // Create skill node prefab
            GameObject nodePrefab = CreateSkillNodePrefab();
            
            // Create connection line prefab
            GameObject linePrefab = CreateConnectionLinePrefab();
            
            // Add SkillTreeManager component
            SkillTreeManager manager = skillTreePanel.GetComponent<SkillTreeManager>();
            if (manager == null)
            {
                manager = skillTreePanel.AddComponent<SkillTreeManager>();
            }
            
            // Assign references
            manager.skillNodesContainer = content.GetComponent<RectTransform>();
            manager.tooltipPanel = tooltip;
            manager.tooltipTitle = tooltip.transform.Find("Title").GetComponent<TextMeshProUGUI>();
            manager.tooltipDescription = tooltip.transform.Find("Description").GetComponent<TextMeshProUGUI>();
            manager.tooltipRequirements = tooltip.transform.Find("Requirements").GetComponent<TextMeshProUGUI>();
            //manager.skillPointsText = header.transform.Find("SkillPointsText").GetComponent<TextMeshProUGUI>();
            manager.skillNodePrefab = nodePrefab;
            manager.connectionLinePrefab = linePrefab;
            
            EditorUtility.SetDirty(skillTreePanel);
            
            Debug.Log("Skill Tree UI built successfully!");
            EditorUtility.DisplayDialog("Success", "Skill Tree UI has been built!\n\nPrefabs created:\n- SkillNodePrefab\n- ConnectionLinePrefab\n\nAssign a SkillTreeData asset to the SkillTreeManager component.", "OK");
        }
        
        private GameObject CreateHeader(Transform parent)
        {
            GameObject header = CreateUIObject("Header", parent);
            RectTransform rect = header.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0, 80);
            
            Image bg = header.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.2f, 1f);
            
            // Title
            GameObject title = CreateTextObject("Title", header.transform, "SKILL TREE", 36);
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0);
            titleRect.anchorMax = new Vector2(0.5f, 1);
            titleRect.sizeDelta = Vector2.zero;
            
            // Skill Points
            GameObject skillPoints = CreateTextObject("SkillPointsText", header.transform, "Skill Points: 0", 28);
            RectTransform spRect = skillPoints.GetComponent<RectTransform>();
            spRect.anchorMin = new Vector2(0.5f, 0);
            spRect.anchorMax = new Vector2(1, 1);
            spRect.sizeDelta = Vector2.zero;
            
            // Close button
            GameObject closeBtn = CreateButton("CloseButton", header.transform, "X");
            RectTransform closeBtnRect = closeBtn.GetComponent<RectTransform>();
            closeBtnRect.anchorMin = new Vector2(1, 0.5f);
            closeBtnRect.anchorMax = new Vector2(1, 0.5f);
            closeBtnRect.pivot = new Vector2(1, 0.5f);
            closeBtnRect.anchoredPosition = new Vector2(-20, 0);
            closeBtnRect.sizeDelta = new Vector2(60, 60);
            
            return header;
        }
        
        private GameObject CreateScrollView(Transform parent)
        {
            GameObject scrollView = CreateUIObject("ScrollView", parent);
            RectTransform rect = scrollView.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = new Vector2(20, 20);
            rect.offsetMax = new Vector2(-20, -100);
            
            ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
            Image scrollBg = scrollView.AddComponent<Image>();
            scrollBg.color = new Color(0.05f, 0.05f, 0.1f, 0.5f);
            
            // Viewport
            GameObject viewport = CreateUIObject("Viewport", scrollView.transform);
            RectTransform vpRect = viewport.GetComponent<RectTransform>();
            vpRect.anchorMin = Vector2.zero;
            vpRect.anchorMax = Vector2.one;
            vpRect.sizeDelta = Vector2.zero;
            
            Mask mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            Image vpImage = viewport.AddComponent<Image>();
            
            // Content
            GameObject content = CreateUIObject("Content", viewport.transform);
            RectTransform contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.5f, 0.5f);
            contentRect.anchorMax = new Vector2(0.5f, 0.5f);
            contentRect.pivot = new Vector2(0.5f, 0.5f);
            contentRect.sizeDelta = new Vector2(2000, 1500);
            
            scrollRect.viewport = vpRect;
            scrollRect.content = contentRect;
            scrollRect.horizontal = true;
            scrollRect.vertical = true;
            
            return scrollView;
        }
        
        private GameObject CreateTooltipPanel(Transform parent)
        {
            GameObject tooltip = CreateUIObject("TooltipPanel", parent);
            RectTransform rect = tooltip.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0, 0.5f);
            rect.sizeDelta = new Vector2(350, 250);
            
            Image bg = tooltip.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
            
            VerticalLayoutGroup layout = tooltip.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(15, 15, 15, 15);
            layout.spacing = 10;
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            
            // Title
            GameObject title = CreateTextObject("Title", tooltip.transform, "Skill Name", 24);
            title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            
            // Description
            GameObject desc = CreateTextObject("Description", tooltip.transform, "Skill description goes here.", 16);
            RectTransform descRect = desc.GetComponent<RectTransform>();
            descRect.sizeDelta = new Vector2(0, 80);
            
            // Requirements
            GameObject req = CreateTextObject("Requirements", tooltip.transform, "Requirements", 14);
            req.GetComponent<TextMeshProUGUI>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
            RectTransform reqRect = req.GetComponent<RectTransform>();
            reqRect.sizeDelta = new Vector2(0, 60);
            
            tooltip.SetActive(false);
            
            return tooltip;
        }
        
        private GameObject CreateSkillNodePrefab()
        {
            GameObject prefab = CreateUIObject("SkillNodePrefab", null);
            RectTransform rect = prefab.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 100);
            
            // Background
            Image bg = prefab.AddComponent<Image>();
            bg.color = Color.white;
            bg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            bg.type = Image.Type.Sliced;
            
            Button button = prefab.AddComponent<Button>();
            
            // Icon
            GameObject icon = CreateUIObject("Icon", prefab.transform);
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.1f, 0.1f);
            iconRect.anchorMax = new Vector2(0.9f, 0.9f);
            iconRect.sizeDelta = Vector2.zero;
            
            Image iconImage = icon.AddComponent<Image>();
            iconImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            iconImage.preserveAspect = true;
            
            // Lock overlay
            GameObject lockOverlay = CreateUIObject("LockOverlay", prefab.transform);
            RectTransform lockRect = lockOverlay.GetComponent<RectTransform>();
            lockRect.anchorMin = Vector2.zero;
            lockRect.anchorMax = Vector2.one;
            lockRect.sizeDelta = Vector2.zero;
            
            Image lockImage = lockOverlay.AddComponent<Image>();
            lockImage.color = new Color(0, 0, 0, 0.7f);
            
            // Level text
            GameObject levelText = CreateTextObject("LevelText", prefab.transform, "0/1", 18);
            RectTransform levelRect = levelText.GetComponent<RectTransform>();
            levelRect.anchorMin = new Vector2(0, 0);
            levelRect.anchorMax = new Vector2(1, 0.3f);
            levelRect.sizeDelta = Vector2.zero;
            
            TextMeshProUGUI levelTMP = levelText.GetComponent<TextMeshProUGUI>();
            levelTMP.fontStyle = FontStyles.Bold;
            levelTMP.alignment = TextAlignmentOptions.Center;
            
            // Add SkillNodeUI component
            SkillNodeUI nodeUI = prefab.AddComponent<SkillNodeUI>();
            nodeUI.iconImage = iconImage;
            nodeUI.backgroundImage = bg;
            nodeUI.lockOverlay = lockImage;
            nodeUI.levelText = levelTMP;
            nodeUI.button = button;
            
            // Save as prefab
            string path = "Assets/Prefabs/UI/SkillNodePrefab.prefab";
            System.IO.Directory.CreateDirectory("Assets/Prefabs/UI");
            PrefabUtility.SaveAsPrefabAsset(prefab, path);
            DestroyImmediate(prefab);
            
            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
        
        private GameObject CreateConnectionLinePrefab()
        {
            GameObject prefab = CreateUIObject("ConnectionLinePrefab", null);
            RectTransform rect = prefab.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 3);
            rect.pivot = new Vector2(0.5f, 0.5f);
            
            Image image = prefab.AddComponent<Image>();
            image.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            
            // Save as prefab
            string path = "Assets/Prefabs/UI/ConnectionLinePrefab.prefab";
            System.IO.Directory.CreateDirectory("Assets/Prefabs/UI");
            PrefabUtility.SaveAsPrefabAsset(prefab, path);
            DestroyImmediate(prefab);
            
            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
        
        private void CreateSampleSkillTreeData()
        {
            SkillTreeData data = ScriptableObject.CreateInstance<SkillTreeData>();
            data.treeName = "Sample Skill Tree";
            
            // Create sample skills in a tree structure
            // Tier 0 (Starting skills)
            data.skills.Add(new SkillNode
            {
                id = "skill_001",
                skillName = "Basic Attack",
                description = "Increases basic attack damage by 10%",
                position = new Vector2(-200, 200),
                tier = 0,
                requiredPoints = 1,
                maxLevel = 3
            });
            
            data.skills.Add(new SkillNode
            {
                id = "skill_002",
                skillName = "Health Boost",
                description = "Increases maximum health by 20",
                position = new Vector2(200, 200),
                tier = 0,
                requiredPoints = 1,
                maxLevel = 3
            });
            
            // Tier 1
            data.skills.Add(new SkillNode
            {
                id = "skill_003",
                skillName = "Power Strike",
                description = "Unlocks a powerful special attack",
                position = new Vector2(-200, 0),
                tier = 1,
                requiredPoints = 2,
                maxLevel = 1,
                prerequisiteIds = new System.Collections.Generic.List<string> { "skill_001" }
            });
            
            data.skills.Add(new SkillNode
            {
                id = "skill_004",
                skillName = "Defense Up",
                description = "Reduces damage taken by 15%",
                position = new Vector2(0, 0),
                tier = 1,
                requiredPoints = 2,
                maxLevel = 2,
                prerequisiteIds = new System.Collections.Generic.List<string> { "skill_001", "skill_002" }
            });
            
            data.skills.Add(new SkillNode
            {
                id = "skill_005",
                skillName = "Regeneration",
                description = "Slowly regenerate health over time",
                position = new Vector2(200, 0),
                tier = 1,
                requiredPoints = 2,
                maxLevel = 1,
                prerequisiteIds = new System.Collections.Generic.List<string> { "skill_002" }
            });
            
            // Tier 2 (Ultimate skills)
            data.skills.Add(new SkillNode
            {
                id = "skill_006",
                skillName = "Ultimate Power",
                description = "Unlocks the ultimate ability",
                position = new Vector2(-100, -200),
                tier = 2,
                requiredPoints = 3,
                maxLevel = 1,
                prerequisiteIds = new System.Collections.Generic.List<string> { "skill_003", "skill_004" },
                nodeColor = new Color(1f, 0.8f, 0.2f, 1f)
            });
            
            data.skills.Add(new SkillNode
            {
                id = "skill_007",
                skillName = "Master Healer",
                description = "Greatly enhances healing abilities",
                position = new Vector2(100, -200),
                tier = 2,
                requiredPoints = 3,
                maxLevel = 1,
                prerequisiteIds = new System.Collections.Generic.List<string> { "skill_004", "skill_005" },
                nodeColor = new Color(0.2f, 1f, 0.2f, 1f)
            });
            
            // Save the asset
            string path = "Assets/Data/SampleSkillTreeData.asset";
            System.IO.Directory.CreateDirectory("Assets/Data");
            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();
            
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = data;
            
            Debug.Log($"Sample Skill Tree Data created at: {path}");
            EditorUtility.DisplayDialog("Success", $"Sample Skill Tree Data created!\n\nPath: {path}\n\nAssign this to the SkillTreeManager component.", "OK");
        }
        
        private GameObject CreateUIObject(string name, Transform parent)
        {
            GameObject obj = new GameObject(name);
            obj.AddComponent<RectTransform>();
            if (parent != null)
                obj.transform.SetParent(parent, false);
            return obj;
        }
        
        private GameObject CreateTextObject(string name, Transform parent, string text, int fontSize)
        {
            GameObject obj = CreateUIObject(name, parent);
            TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            return obj;
        }
        
        private GameObject CreateButton(string name, Transform parent, string text)
        {
            GameObject obj = CreateUIObject(name, parent);
            Image image = obj.AddComponent<Image>();
            image.color = new Color(0.8f, 0.2f, 0.2f, 1f);
            
            Button button = obj.AddComponent<Button>();
            
            GameObject textObj = CreateTextObject("Text", obj.transform, text, 24);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            return obj;
        }
    }
}
