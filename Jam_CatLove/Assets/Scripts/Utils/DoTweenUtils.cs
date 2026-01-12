using DG.Tweening;
using TMPro;
using UnityEngine;

public static class DoTweenUtils
{
    public static void PlayNumberCountAnimation(TextMeshProUGUI numberText, int startValue, int targetValue)
    {
        const float scaleUpTime = 0.1f;
        const float scaleDownTime = 0.15f;
        const float countDuration = 0.5f;
        const float punchScale = 1.5f;

        // Kill any previous tweens
        numberText.DOKill();

        // Reset scale
        numberText.rectTransform.localScale = Vector3.one;

        // Sequence for combined animation
        Sequence seq = DOTween.Sequence();

        // Scale up
        seq.Append(numberText.rectTransform.DOScale(punchScale, scaleUpTime).SetEase(Ease.OutBack));

        // Count number while slightly scaled up
        //var startValue = int.Parse(numberText.text);
        seq.Append(DOTween.To(() => startValue, x => { numberText.text = x.ToString(); }, targetValue, countDuration));

        // Scale back down
        seq.Append(numberText.rectTransform.DOScale(1f, scaleDownTime).SetEase(Ease.InBack));
        
        seq.OnKill(() =>
        {
            numberText.text = targetValue.ToString();
            numberText.rectTransform.localScale = Vector3.one;
        });

        seq.Play();
    }
}