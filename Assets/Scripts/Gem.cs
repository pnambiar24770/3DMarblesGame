using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private AudioManager audioManager;

    private void Start()
    {
        // Find the AudioManager instance in the scene
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GemCount gemCount = other.GetComponent<GemCount>();

        if (gemCount != null)
        {
            gemCount.GemCollected();

            // Check if AudioManager is found
            if (audioManager != null)
            {
                audioManager.PlaySound("GetGem");
            }
            else
            {
                Debug.LogWarning("AudioManager not found in the scene");
            }

            gameObject.SetActive(false);
        }
    }
}