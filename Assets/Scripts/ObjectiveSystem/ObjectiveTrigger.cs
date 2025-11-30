using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
  private bool triggered = false;
  private void OnTriggerEnter(Collider other)
  {
    if (triggered) return;
    if (other.CompareTag("Player"))
    {
      FindObjectOfType<ObjectiveManager>().AdvanceObjective();
      triggered = true;
    }
  }
}
