using UnityEngine;
using System.Collections;

public class OpenableLid : MonoBehaviour, IInteractable
{
    [SerializeField] private float openAngle = 90f;      // Lid opens upward
    [SerializeField] private float duration = 0.7f;       // Opening speed

    private bool opened = false;

    public void Interact()
    {
        if (opened) return;

        opened = true;
        StartCoroutine(OpenLid());
    }

    private IEnumerator OpenLid()
    {
        Quaternion startRot = transform.localRotation;
        Quaternion endRot = Quaternion.Euler(openAngle, 
                                             transform.localEulerAngles.y,
                                             transform.localEulerAngles.z);

        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / duration;

            transform.localRotation = Quaternion.Lerp(startRot, endRot, t);

            yield return null;
        }

        transform.localRotation = endRot;
    }
}

