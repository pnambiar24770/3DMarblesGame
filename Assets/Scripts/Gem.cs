using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GemCount gemCount = other.GetComponent<GemCount>();

        if (gemCount != null)
        {
            gemCount.GemCollected();
            gameObject.SetActive(false);
        }
    }
}