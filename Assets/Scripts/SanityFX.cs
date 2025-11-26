using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SanityPostFX : MonoBehaviour
{
    [Header("References")]
    public Volume globalVolume;

    [Header("Post FX Ranges")]
    public float chromaticMin = 0.3f;
    public float chromaticMax = 1.0f;

    public float lensMin = 0.6f;
    public float lensMax = 1.0f;

    [Header("Smoothing")]
    public float smoothSpeed = 5f;

    // cached post-fx overrides
    private ChromaticAberration chroma;
    private LensDistortion lens;

    void Start()
    {
        if (globalVolume == null)
        {
            Debug.LogError("SanityPostFX: globalVolume is not assigned!");
            return;
        }

        globalVolume.profile.TryGet(out chroma);
        globalVolume.profile.TryGet(out lens);
    }

    void Update()
    {
        if (SanityManager.Instance == null) return;

        float sanity = SanityManager.Instance.CurrentSanity;
        float maxSanity = SanityManager.Instance.fullSanity;

        // how low sanity is (0 = healthy, 1 = insane)
        float t = 1f - (sanity / maxSanity);

        // lerp target values
        float targetChromatic = Mathf.Lerp(chromaticMin, chromaticMax, t);
        float targetLens = Mathf.Lerp(lensMin, lensMax, t);

        // smooth interpolation for better feeling
        if (chroma != null)
            chroma.intensity.value = Mathf.Lerp(chroma.intensity.value, targetChromatic, Time.deltaTime * smoothSpeed);

        if (lens != null)
            lens.intensity.value = Mathf.Lerp(lens.intensity.value, targetLens, Time.deltaTime * smoothSpeed);
    }
}
