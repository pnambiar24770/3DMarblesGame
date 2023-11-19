using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "BallSizeAbility", menuName = "Abilities/MegaBall")]
public class MegaBall : AbilityBase
{
    public float growthRate = 0.1f;
    public float originalScale = 0.5f;

    public override void Activate(GameObject parent)
    {
        PlayerSizeManager sizeManager = parent.GetComponent<PlayerSizeManager>();

        if (sizeManager != null)
        {
            float targetScale = GetTargetScale(sizeManager.currentSize);
            parent.GetComponent<MonoBehaviour>().StartCoroutine(GrowOverTime(parent.transform, targetScale));

            // Add any other size-specific activation logic here
        }
    }

    float GetTargetScale(PlayerSizeManager.BallSize size)
    {
        switch (size)
        {
            case PlayerSizeManager.BallSize.Small:
                return originalScale * 0.25f;
            case PlayerSizeManager.BallSize.Medium:
                return originalScale;
            case PlayerSizeManager.BallSize.Large:
                return originalScale * 4f;
            default:
                return originalScale;
        }
    }

    IEnumerator GrowOverTime(Transform targetTransform, float targetScale)
    {
        Vector3 initialScale = targetTransform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            targetTransform.localScale = Vector3.Lerp(initialScale, new Vector3(targetScale, targetScale, targetScale), elapsedTime);
            elapsedTime += Time.deltaTime * growthRate;
            yield return null;
        }

        // Ensure that the scale is exactly the target scale when the coroutine ends
        targetTransform.localScale = new Vector3(targetScale, targetScale, targetScale);
    }
}
