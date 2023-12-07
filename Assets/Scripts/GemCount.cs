using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GemCount : MonoBehaviour
{
    public int gemsCollected { get; private set; }

    public UnityEvent<GemCount> OnGemCollected;

    public void GemCollected()
    {
        gemsCollected++;
        OnGemCollected.Invoke(this);
    }
}