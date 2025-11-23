using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SanityManager : MonoBehaviour
{
    public static SanityManager Instance { get; private set; }

    Slider sanitySlider;
    [Header("Sanity Settings")]
    public int fullSanity = 100;
    [Tooltip("Higher -> faster drain")]
    public int difficulty = 1;

    [Tooltip("Sanity points per second when under a lamp")]
    public float replenishRatePerSecond = 10f;

    [Tooltip("Sanity points per second when draining (base)")]
    public float drainRatePerSecond = 2f;

    [Header("Death & Respawn")]
    [Tooltip("Delay before respawning after sanity reaches 0")]
    [SerializeField] private float respawnDelay = 2f;

    [Header("References")]
    [SerializeField] private FlashlightController flashlightController; // optional

    public bool isUnderLamp = false;
    private Coroutine sanityCoroutine;
    private bool isDead = false;

    private void Awake()
    {
        // singleton pattern (simple)
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        sanitySlider = GetComponent<Slider>();
        if (sanitySlider == null)
        {
            Debug.LogError("SanityManager: No Slider component found on this GameObject.");
            return;
        }

        sanitySlider.maxValue = fullSanity;
        sanitySlider.value = fullSanity;

        sanityCoroutine = StartCoroutine(SanityLoop());
    }

    private IEnumerator SanityLoop()
    {
        float logTimer = 0f;
        while (true)
        {
            // Don't update sanity if player is dead
            if (isDead)
            {
                yield return null;
                continue;
            }

            float delta = Time.deltaTime;
            logTimer += delta;

            bool flashlightOn = (flashlightController != null) ? flashlightController.IsOn : false;
            bool underLamp = isUnderLamp;

            // compute what we will apply this frame (but don't apply yet)
            float change = 0f;
            if (flashlightOn)
            {
                change = 0f; // paused by flashlight
            }
            else if (underLamp)
            {
                change = replenishRatePerSecond * delta;
            }
            else
            {
                change = -drainRatePerSecond * difficulty * delta;
            }

            // snapshot before
            float before = sanitySlider != null ? sanitySlider.value : -1f;

            // apply change
            if (sanitySlider != null)
            {
                sanitySlider.value = Mathf.Clamp(sanitySlider.value + change, 0f, sanitySlider.maxValue);
            }

            // snapshot after
            float after = sanitySlider != null ? sanitySlider.value : -1f;

            // Check if sanity reached 0
            if (after <= 0f && !isDead)
            {
                StartCoroutine(HandleDeath());
            }

            // log every 1s to keep console readable, but you can shorten if needed
            if (logTimer >= 1f)
            {
                Debug.Log($"[SanityDbg] before={before:F2} change={change:F4} after={after:F2} underLamp={underLamp} flashlightOn={flashlightOn} drainRate={drainRatePerSecond} difficulty={difficulty} Time.timeScale={Time.timeScale}");
                logTimer = 0f;
            }

            yield return null;
        }
    }

    private IEnumerator HandleDeath()
    {
        isDead = true;
        Debug.Log("Player has lost all sanity!");

        // Optional: Disable player controls during death
        // You could disable the FirstPersonController here if needed

        // Wait for respawn delay
        yield return new WaitForSeconds(respawnDelay);

        // Respawn player
        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.RespawnPlayer();
        }
        else
        {
            Debug.LogError("CheckpointManager not found! Cannot respawn player.");
        }

        // Reset sanity
        if (sanitySlider != null)
        {
            sanitySlider.value = fullSanity;
        }

        isDead = false;
        Debug.Log("Player respawned with full sanity.");
    }

    /// <summary>
    /// Called by lamppost triggers to set whether player is under lamp.
    /// </summary>
    /// <param name="underLamp"></param>
    public void SetUnderLamp(bool underLamp)
    {
        isUnderLamp = underLamp;
    }

    // Allows other scripts (e.g. lamppost triggers) to force the flashlight off
    public void ForceFlashlightOff()
    {
        if (flashlightController != null)
        {
            flashlightController.SetState(false);
        }
    }

    /// Optional accessor for other scripts
    public float CurrentSanity => sanitySlider != null ? sanitySlider.value : 0f;
}
