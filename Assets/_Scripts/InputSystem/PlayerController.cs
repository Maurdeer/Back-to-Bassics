using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerinput;
    private PlayerBattlePawn _battlepawn;
    private PlayerTraversalPawn _traversalpawn;
    public PlayerTraversalPawn TraversalPawn => _traversalpawn;
    public PlayerBattlePawn BattlePawn => _battlepawn;
    private void Awake()
    {
        // References
        _playerinput = GetComponent<PlayerInput>();
        _battlepawn = GetComponent<PlayerBattlePawn>();
        _traversalpawn = GetComponent<PlayerTraversalPawn>();

        // Input Battle Actions
        _playerinput.SwitchCurrentActionMap("PlayerBattlePawn");
        _playerinput.actions["Dodge"].performed += OnDodge;
        _playerinput.actions["Jump"].performed += OnDodge;
        _playerinput.actions["Slash"].performed += OnBattleSlash;

        // Input World Traversal Actions
        // This might just have to keep updating on fixed update tbh
        _playerinput.SwitchCurrentActionMap("PlayerTraversalPawn");
        _playerinput.actions["Slash"].performed += OnTraversalSlash;
        _playerinput.actions["Interact"].performed += OnInteractAction;
        _playerinput.actions["Pause"].performed += OnPauseAction;

        // Input UI
        //_playerinput.SwitchCurrentActionMap("UI");

        // Input Dialogue
        //_playerinput.SwitchCurrentActionMap("Dialogue");

        // Input Cutscene
        //_playerinput.SwitchCurrentActionMap("Cutscene");
    }
    public void DisableControl()
    {
        _playerinput.currentActionMap.Disable();
    }
    public void EnableControl()
    {
        _playerinput.currentActionMap.Enable();
    }
    public void SwitchToBattleActions()
    {
        _playerinput.currentActionMap.Disable();
        _playerinput.SwitchCurrentActionMap("PlayerBattlePawn");
        _playerinput.currentActionMap.Enable();
    }
    public void SwitchToTraversalActions()
    {
        _playerinput.currentActionMap.Disable();
        _playerinput.SwitchCurrentActionMap("PlayerTraversalPawn");
        _playerinput.currentActionMap.Enable();
    }
    public void SwitchToDialogueActions()
    {
        _playerinput.currentActionMap.Disable();
        _playerinput.SwitchCurrentActionMap("Dialogue");
        _playerinput.currentActionMap.Enable();
    }
    public void SwitchToCutsceneActions()
    {
        _playerinput.currentActionMap.Disable();
        _playerinput.SwitchCurrentActionMap("Cutscene");
        _playerinput.currentActionMap.Enable();
    }
    public void SwitchToUIActions()
    {
        _playerinput.currentActionMap.Disable();
        _playerinput.SwitchCurrentActionMap("UI");
        _playerinput.currentActionMap.Enable();
    }
    #region Battle Pawn Actions
    public void OnDodge(InputAction.CallbackContext context)
    {
        _battlepawn.Dodge(context.ReadValue<Vector2>());
    }
    public void OnBattleSlash(InputAction.CallbackContext context)
    {
        _battlepawn.Slash(context.ReadValue<Vector2>());
    }
    #endregion
    #region Traversal Pawn Actions
    private void Update()
    {
        Vector2 direction = _playerinput.actions["Move"].ReadValue<Vector2>();
        _traversalpawn.Move(new Vector3(direction.x, 0, direction.y));
    }
    public void OnTraversalSlash(InputAction.CallbackContext context)
    {
        _traversalpawn.Slash(context.ReadValue<Vector2>());
    }
    public void OnInteractAction(InputAction.CallbackContext context)
    {
        _traversalpawn.Interact();
    }
    public void OnPauseAction(InputAction.CallbackContext context)
    {
        UIManager.Instance.pauseMenu.TogglePause();
    }
    #endregion
}
