using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "BallSizeAbility", menuName = "Abilities/MegaBall")]
public class MegaBall : AbilityBase
{
    public float growthRate = 0.1f;
    public float originalScale = 0.5f;

    public override void Activate(GameObject parent)
    {
        PlayerManager sizeManager = parent.GetComponent<PlayerManager>();

        if (sizeManager != null)
        {
            float targetScale = GetTargetScale(sizeManager.currentSize);
            parent.GetComponent<MonoBehaviour>().StartCoroutine(GrowOverTime(parent.transform, targetScale));
        }
    }

    float GetTargetScale(PlayerManager.BallSize size)
    {
        switch (size)
        {
            case PlayerManager.BallSize.Small:
                return originalScale * 0.35f;
            case PlayerManager.BallSize.Medium:
                return originalScale;
            case PlayerManager.BallSize.Large:
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

        targetTransform.localScale = new Vector3(targetScale, targetScale, targetScale);
    }
}
