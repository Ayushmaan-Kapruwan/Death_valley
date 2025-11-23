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
        // Use SetState instead of directly modifying isOn
        // This ensures the lamppost check is always performed
        SetState(!isOn);
    }

    private void ApplyState()
    {
        if (flashlightLight != null) flashlightLight.enabled = isOn;
    }

    // Public control methods
    public void SetState(bool on)
    {
        if (on)
        {
            var sm = SanityManager.Instance;
            if (sm != null && sm.isUnderLamp)
            {
                Debug.Log("[FlashlightController] Attempt to turn ON flashlight blocked: player is under a lamppost.");
                return;
            }
        }

        isOn = on;
        ApplyState();
    }

    public void TurnOff()
    {
        SetState(false);
    }

    public void TurnOn()
    {
        SetState(true);
    }

    public bool IsOn => isOn;

}
