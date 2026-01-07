using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("Heart Spawn Settings")]
    [SerializeField] private GameObject catHeartPrefab;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float heartSpawnOffset = 50f; // Offset from cat's position in pixels
    
    [Header("Animation Settings")]
    [SerializeField] private float popDuration = 0.3f;
    [SerializeField] private float moveDuration = 0.8f;
    [SerializeField] private float popOvershoot = 1.2f;
    
    [Header("Hearts Bar")]
    [SerializeField] private TextMeshProUGUI heartsCountText;
    [SerializeField] private RectTransform heartsCounterImage; // Top left corner position

    private RectTransform canvasRectangle;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
        canvasRectangle = canvas.GetComponent<RectTransform>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetHeartsCount(int count)
    {
        heartsCountText.text = count.ToString();
    }
    
    public void SpawnCatHeart(Transform catTransform)
    {
        // Convert the cat's heart spawn position to canvas space
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(catTransform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPoint,
            canvas.worldCamera,
            out Vector2 canvasPosition
        );

        // Instantiate heart as child of canvas
        GameObject heart = Instantiate(catHeartPrefab, canvas.transform);
        RectTransform heartRect = heart.GetComponent<RectTransform>();
        heartRect.anchoredPosition = canvasPosition;// + new Vector2(0, heartSpawnOffset);
        
        // Start at scale 0
        heartRect.localScale = Vector3.zero;

        // Create animation sequence
        Sequence heartSequence = DOTween.Sequence();
        
        // 1. Pop out with overshoot
        heartSequence.Append(
            heartRect.DOScale(1f, popDuration).SetEase(Ease.OutBack, popOvershoot)
        );
        
        // 2. Move to target position with acceleration
        var targetWorldPosition = heartsCounterImage.position;
        var targetScreenPosition = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, targetWorldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectangle,
            targetScreenPosition, 
            canvas.worldCamera, 
            out var targetLocalCanvasPosition);
        
        heartSequence.Append(
            heartRect.DOAnchorPos(targetLocalCanvasPosition, moveDuration)
                .SetEase(Ease.InQuad)
        );
        
        // 3. Optional: scale down slightly as it reaches target
        heartSequence.Join(
            heartRect.DOScale(0.7f, moveDuration * 0.5f)
                .SetDelay(moveDuration * 0.5f)
        );
        heartSequence.AppendCallback(() => heartsCountText.text = (int.Parse(heartsCountText.text) + 1).ToString());

        // Destroy heart after animation completes
        heartSequence.OnComplete(() => Destroy(heart));
    }
}
