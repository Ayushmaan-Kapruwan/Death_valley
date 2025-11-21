using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("Input (optional)")]
    [SerializeField] private PlayerInputHandler inputHandler;

    [Header("Flashlight")]
    [SerializeField] private Light flashlightLight;

    [SerializeField] private bool startOn = false;

    private bool isOn;
    private bool lastInputState = false;

    private void Start()
    {
        isOn = startOn;
        ApplyState();
    }

    private void Update()
    {
        if (inputHandler != null)
        {
            bool current = inputHandler.FlashlightTriggered;
            if (current && !lastInputState)
            {
                ToggleFlashlight();
            }
            lastInputState = current;
            return;
        }
    }

    private void ToggleFlashlight()
    {
        isOn = !isOn;
        ApplyState();
    }

    private void ApplyState()
    {
        if (flashlightLight != null) flashlightLight.enabled = isOn;
    }
}
