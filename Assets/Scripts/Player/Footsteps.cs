using UnityEngine;

public class Footsteps : MonoBehaviour
{
  public AudioSource audioSource;
  public float stepInterval = 0.5f;
  public CharacterController controller;

  [Header("Footstep Sounds")]
  public AudioClip[] groundSteps;
  public AudioClip[] woodSteps;
  
  float stepTimer = 0f;

  void Update()
  {
    if (controller.isGrounded && controller.velocity.magnitude > 2f)
    {
      stepTimer += Time.deltaTime;
      if (stepTimer > stepInterval)
      {
        PlayFootstep();
        stepTimer = 0f;
      }
    }
  }

  void PlayFootstep()
  {
    string surface = DetectSurface();
    AudioClip clip = null;
    switch (surface)
    {
      case "Ground":
        clip = groundSteps[Random.Range(0, groundSteps.Length)];
        break;

      case "Wood":
        clip = woodSteps[Random.Range(0, woodSteps.Length)];
        break;

      default:
        clip = groundSteps[Random.Range(0, groundSteps.Length)];
        break;
    }
    audioSource.PlayOneShot(clip);

  }

  string DetectSurface()
  {
    RaycastHit hit;

    Vector3 origin = transform.position + Vector3.down * 0.2f;
    int layerMask = ~LayerMask.GetMask("Player");

      if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, layerMask))
      {
        return hit.collider.tag;
      }

      return "Ground";
  }

  void OnDrawGizmosSelected()
  {
    if (!Application.isPlaying) return;

    Gizmos.color = Color.green;

    Vector3 origin = transform.position + Vector3.down * 0.2f;
    Vector3 direction = Vector3.down * 2f;

    Gizmos.DrawLine(origin, origin + direction);
  }

}
