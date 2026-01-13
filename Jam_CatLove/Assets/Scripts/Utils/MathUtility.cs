using UnityEngine;

public static class MathUtility
{
    /// <summary>
    /// Returns a multiplier if speed is within tolerancePercentage of optimalSpeed. Else lerps between maxMultiplier and 1f.
    /// </summary>
    public static float GetSpeedMultiplier(float speed, float optimalSpeed = 5f, float minSpeed = 0.01f, float maxSpeed = 10f, float minMultiplier = 1f, float maxMultiplier = 4f)
    {
        //const float minMultiplier = 1f;
    
        // If outside the speed range, return min multiplier
        if (speed < minSpeed || speed > maxSpeed)
        {
            return minMultiplier;
        }

        // Map speed to 0-1 range
        var distanceFromMin = (speed - minSpeed) / (optimalSpeed - minSpeed);
        var distanceFromMax = (maxSpeed - speed) / (maxSpeed - optimalSpeed);
        var t = Mathf.Min(distanceFromMin, distanceFromMax);

        // Clamp and smooth
        t = Mathf.Clamp01(t);
        t = t * t * (3f - 2f * t); // Smoothstep

        return Mathf.Lerp(minMultiplier, maxMultiplier, t);
    }
}
