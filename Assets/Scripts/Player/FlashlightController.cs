using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightController : MonoBehaviour
{
    [Header("Input (optional)")]
    [SerializeField] private PlayerInputHandler inputHandler;

    [Header("Flashlight")]
    [SerializeField] private Light flashlightLight;
    [SerializeField] private bool startOn = false;

    [Header("Overheat Settings")]
    [SerializeField] private float overheatThreshold = 10f;
    [SerializeField] private float overheatCooldownDuration = 5f;
    [SerializeField] private float cooldownRate = 2f;

    [Header("UI References")]
    [SerializeField] private Slider overheatSlider;

    private bool isOn;
    private bool lastInputState = false;
    private bool isOverheated = false;
    private float currentHeat = 0f;
    private Coroutine cooldownCoroutine;

    private void Start()
    {
        isOn = startOn;
        ApplyState();
        InitializeSlider();
    }

    private void InitializeSlider()
    {
        if (overheatSlider == null)
        {
            Debug.LogWarning("FlashlightController: No overheat Slider component assigned.");
            return;
        }

        overheatSlider.maxValue = overheatThreshold;
        overheatSlider.value = 0f;
    }

    private void Update()
    {
        // Handle overheat mechanics
        UpdateHeat();

        // Update UI slider (always update to show current state)
        UpdateSlider();

        // Handle input
        if (inputHandler != null)
        {
            bool current = inputHandler.FlashlightTriggered;
            if (current && !lastInputState)
            {
                ToggleFlashlight();
            }
            lastInputState = current; // checks the last frame F input
            return;
        }
    }

    private void UpdateHeat()
    {
        // Don't update heat during forced cooldown
        if (isOverheated)
        {
            return;
        }

        if (isOn)
        {
            // Increase heat while flashlight is ON
            currentHeat += Time.deltaTime;

            // Check if overheated
            if (currentHeat >= overheatThreshold)
            {
                // Clamp to exactly the threshold before triggering overheat
                currentHeat = overheatThreshold;
                OverheatFlashlight();
            }
        }
        else
        {
            // Decrease heat while flashlight is OFF (normal cooling)
            currentHeat -= cooldownRate * Time.deltaTime;
            currentHeat = Mathf.Max(0f, currentHeat);
        }
    }

    private void UpdateSlider()
    {
        if (overheatSlider != null)
        {
            overheatSlider.value = currentHeat;
            
            // Force fill to be completely hidden at zero
            if (currentHeat <= 0.001f)
            {
                // Get the Fill image
                Image fillImage = overheatSlider.fillRect?.GetComponent<Image>();
                if (fillImage != null)
                {
                    fillImage.enabled = false; // Hide completely
                }
            }
            else
            {
                // Ensure fill is visible when heat > 0
                Image fillImage = overheatSlider.fillRect?.GetComponent<Image>();
                if (fillImage != null)
                {
                    fillImage.enabled = true; // Show
                }
            }
        }
    }

    private void ToggleFlashlight()
    {
        // Prevent toggling if overheated
        if (isOverheated)
        {
            Debug.Log("[FlashlightController] Flashlight is overheated! Wait for cooldown.");
            return;
        }

        SetState(!isOn);
    }

    private void OverheatFlashlight()
    {
        if (isOn)
        {
            Debug.Log($"[FlashlightController] Flashlight overheated! Cooling down for {overheatCooldownDuration} seconds.");
            
            isOverheated = true;
            TurnOff();
            
            // Start gradual cooldown coroutine
            if (cooldownCoroutine != null)
            {
                StopCoroutine(cooldownCoroutine);
            }
            cooldownCoroutine = StartCoroutine(GradualCooldownCoroutine());
        }
    }

    private IEnumerator GradualCooldownCoroutine()
    {
        // Calculate cooldown speed to reach 0 in overheatCooldownDuration seconds
        float cooldownSpeed = overheatThreshold / overheatCooldownDuration;
        
        // Gradually decrease heat from threshold to 0
        while (currentHeat > 0f)
        {
            currentHeat -= cooldownSpeed * Time.deltaTime;
            currentHeat = Mathf.Max(0f, currentHeat);
            
            // Slider is updated in Update() method
            
            yield return null;
        }
        
        // Ensure heat is exactly 0
        currentHeat = 0f;
        
        // Re-enable flashlight
        isOverheated = false;
        Debug.Log("[FlashlightController] Flashlight cooled down and ready to use.");
    }

    private void ApplyState()
    {
        if (flashlightLight != null) flashlightLight.enabled = isOn;
    }

    // Public control methods
    public void SetState(bool on)
    {
        // Prevent turning ON if overheated
        if (on && isOverheated)
        {
            Debug.Log("[FlashlightController] Cannot turn ON: flashlight is overheated.");
            return;
        }

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
    public bool IsOverheated => isOverheated;
    public float HeatPercentage => Mathf.Clamp01(currentHeat / overheatThreshold);
}
