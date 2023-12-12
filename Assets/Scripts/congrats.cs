using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class congrats : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float speed = 1.0f;
    public float colorIntensity = 1.0f;

    private void Start()
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (textMesh != null)
        {
            for (int i = 0; i < textMesh.textInfo.characterCount; ++i)
            {
                var charInfo = textMesh.textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                float hue = (Time.time * speed + i * 0.05f) % 1.0f;
                Color color = Color.HSVToRGB(hue, 1.0f, colorIntensity);
                textMesh.textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[charInfo.vertexIndex] = color;
                textMesh.textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[charInfo.vertexIndex + 1] = color;
                textMesh.textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[charInfo.vertexIndex + 2] = color;
                textMesh.textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[charInfo.vertexIndex + 3] = color;
            }

            textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }
}
