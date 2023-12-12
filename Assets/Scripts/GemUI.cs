using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GemUI : MonoBehaviour
{
    private TextMeshProUGUI gemText;
    public TextMeshProUGUI congratulationsText;

    // Start is called before the first frame update
    void Start()
    {
        gemText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    public void UpdateGemText(GemCount gemCount)
    {
        gemText.text = "Gems: " + gemCount.gemsCollected.ToString();

        if (gemCount.gemsCollected >= 150 && congratulationsText != null)
        {
            congratulationsText.gameObject.SetActive(true);
        }
    }
}
