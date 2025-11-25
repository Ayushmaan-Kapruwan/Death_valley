using UnityEngine;
using UnityEngine.UI;

public class OverheatUI : MonoBehaviour
{
    [SerializeField] private FlashlightController flashlightController;
    [SerializeField] private Image fillImage; // Reference to slider's Fill image
    //[SerializeField] private Text heatText; // Optional: Show percentage as text

    [Header("Colors")]
    [SerializeField] private Color coldColor = Color.green;
    [SerializeField] private Color warmColor = Color.yellow;
    [SerializeField] private Color hotColor = Color.red;

    private void Update()
    {
        if (flashlightController == null) return;

        float heatPercent = flashlightController.HeatPercentage;

        // Update fill color
        if (fillImage != null)
        {
            if (heatPercent < 0.5f)
                fillImage.color = Color.Lerp(coldColor, warmColor, heatPercent * 2);
            else
                fillImage.color = Color.Lerp(warmColor, hotColor, (heatPercent - 0.5f) * 2);
        }

        // Update text (optional)
        //if (heatText != null)
        //{
        //    heatText.text = $"Heat: {Mathf.RoundToInt(heatPercent * 100)}%";
        //}
    }
}