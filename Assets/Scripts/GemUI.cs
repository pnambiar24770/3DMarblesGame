using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GemUI : MonoBehaviour
{
    private TextMeshProUGUI gemText;

    // Start is called before the first frame update
    void Start()
    {
        gemText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    public void UpdateGemText(GemCount gemCount)
    {
        gemText.text = "Gems: " + gemCount.gemsCollected.ToString();
    }
}
