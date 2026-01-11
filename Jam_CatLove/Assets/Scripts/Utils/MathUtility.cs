using UnityEngine;

public static class MathUtility
{
    /// <summary>
    /// Returns a multiplier if speed is within tolerancePercentage of optimalSpeed. Else lerps between maxMultiplier and 1f.
    /// </summary>
    public static float GetSpeedMultiplier(float speed, float optimalSpeed = 5f, float lowerTolerance = 0.25f, float upperTolerance = 0.25f, float maxMultiplier = 4f)
    {
        const float minMultiplier = 1f;
        
        // Calculate ratio from target speed
        var ratio = speed / optimalSpeed;
    
        // Calculate ratio limits from tolerances
        var minRatio = 1f - lowerTolerance;
        var maxRatio = 1f + upperTolerance;
    
        // If outside the ratio range, return min multiplier
        if (ratio < minRatio || ratio > maxRatio)
        {
            return minMultiplier;
        }
    
        // Map ratio to 0-1 range
        // Distance from the closest edge (0 = at edge, 1 = at center)
        var distanceFromMin = (ratio - minRatio) / lowerTolerance;
        var distanceFromMax = (maxRatio - ratio) / upperTolerance;
        var t = Mathf.Min(distanceFromMin, distanceFromMax);
    
        // Clamp and smooth
        t = Mathf.Clamp01(t);
        t = t * t * (3f - 2f * t); // Smoothstep
    
        return Mathf.Lerp(minMultiplier, maxMultiplier, t);
    }
}
