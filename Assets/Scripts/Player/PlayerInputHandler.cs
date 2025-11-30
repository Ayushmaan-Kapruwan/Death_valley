using UnityEngine; 
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
  [Header("Input Action Asset")]
  [SerializeField] private InputActionAsset playerControls;

  [Header("Action Map Name Reference")]
  [SerializeField] private string actionMapName = "Player";

  [Header("Action Name Reference")]
  [SerializeField] private string movement = "Movement";
  [SerializeField] private string rotation = "Rotation";
  [SerializeField] private string jump = "Jump";
  [SerializeField] private string sprint = "Sprint";
  [SerializeField] private string flashlight = "Flashlight"; //J

  private InputAction movementAction;
  private InputAction rotationAction;
  private InputAction jumpAction;
  private InputAction sprintAction;
  private InputAction flashlightAction;
  public bool FlashlightTriggered { get; private set; }

  public Vector2 MovementInput { get; private set; }
  public Vector2 RotationInput { get; private set; } 
  public bool JumpTriggered { get; private set; }
  public bool SprintTriggered { get; private set; }

  private void Awake()
  {
    InputActionMap mapReference = playerControls.FindActionMap(actionMapName);

    movementAction = mapReference.FindAction(movement);
    rotationAction = mapReference.FindAction(rotation);
    jumpAction = mapReference.FindAction(jump);
    sprintAction = mapReference.FindAction(sprint);
    flashlightAction = mapReference.FindAction(flashlight);

    SubscribeActionValueToInputEvents();
  }

  private void SubscribeActionValueToInputEvents()
  {
    movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
    movementAction.canceled += inputInfo => MovementInput = Vector2.zero;

    rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
    rotationAction.canceled += inputInfo => RotationInput = Vector2.zero; // changed movementAction to rotation for rotation issue

    jumpAction.performed += inputInfo => JumpTriggered = true;
    jumpAction.canceled += inputInfo => JumpTriggered = false;

    sprintAction.performed += inputInfo => SprintTriggered = true;
    sprintAction.canceled += inputInfo => SprintTriggered = false;

    flashlightAction.performed += _ => FlashlightTriggered = true;
    flashlightAction.canceled  += _ => FlashlightTriggered = false;

  }

  private void OnEnable()
  {
    playerControls.FindActionMap(actionMapName).Enable();
  }

  private void OnDisable()
  {
    playerControls.FindActionMap(actionMapName).Disable();
  }
}
