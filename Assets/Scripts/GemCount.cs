using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GemCount : MonoBehaviour
{
    public int gemsCollected = 0;

    public UnityEvent<GemCount> OnGemCollected;

    public void GemCollected()
    {
        gemsCollected++;
        OnGemCollected.Invoke(this);
    }
}