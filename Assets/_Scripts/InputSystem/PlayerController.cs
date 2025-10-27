using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour, IDataPersistence
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

        // Reset Player Input
        _playerinput.actions = new PlayerInputActions().asset;

        // Input Battle Actions
        _playerinput.SwitchCurrentActionMap("PlayerBattlePawn");
        _playerinput.actions["Dodge"].performed += OnDodge;
        _playerinput.actions["Jump"].performed += OnDodge;
        _playerinput.actions["Slash"].performed += OnBattleSlash;
        _playerinput.actions["Pause"].performed += OnPauseAction;

        // Input World Traversal Actions
        // This might just have to keep updating on fixed update tbh
        _playerinput.SwitchCurrentActionMap("PlayerTraversalPawn");
        _playerinput.actions["Slash"].performed += OnTraversalSlash;
        _playerinput.actions["Interact"].performed += OnInteractAction;
        _playerinput.actions["Pause"].performed += OnPauseAction;

        // Input UI
        _playerinput.SwitchCurrentActionMap("UI");
        _playerinput.actions["Pause"].performed += OnPauseAction;
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

    public void LoadData(GameData data)
    {
        //if (data.truthArray[3]) return; // (Ryan) WRECKON AHH CRAZY

        //change the player's position to match the saved data's player position

        //if the playerPosition dict has the scene in it already
        if (data.playerPosition.ContainsKey(SceneManager.GetActiveScene().name))
        {
            //update the position value for this scene
            this.transform.position = data.playerPosition[SceneManager.GetActiveScene().name]; 
        }
        else
        {
            //uh oh something went wrong
            Debug.Log("No saved data for the current scene.");
        }
    }

    public void SaveData(GameData data)
    {
        //get player transform and assign it to data's player position

        // (Ryan) WRECKCON LOGIC WOOO
        // No Concern over updating these in order since truthArray is updated by reference in GameData!!
        //if (data.truthArray[3]) return; // Beat King Sal

        //if (data.truthArray[1]) // Wack since there numbers are off lmao
        //{
        //    // Beat Small Fry
        //    data.playerPosition[SceneManager.GetActiveScene().name] = new Vector3(830.4658f, 100.043f, 442f);
        //}
        //else if (data.truthArray[2])
        //{
        //    // Beat Turbo Top
        //    data.playerPosition[SceneManager.GetActiveScene().name] = new Vector3(830.4658f, 100.043f, 394.100006f);
        //}
        //else if (data.truthArray[0])
        //{
        //    // Beat Bassics
        //    data.playerPosition[SceneManager.GetActiveScene().name] = new Vector3(830.4658f, 100.043f, 303.299988f);
        //}


        // [REAL AND BETTER LOGIC HERE]
        //if the scene is already in the playerPosition dict
        if (data.playerPosition.ContainsKey(SceneManager.GetActiveScene().name))
        {
            //save the current position as the value for the scene key
            data.playerPosition[SceneManager.GetActiveScene().name] = this.transform.position;
        }
        else //if the scene isn't already in there
        {
            //add the current scene & transform to the dict as a key-value pair
            data.playerPosition.Add(SceneManager.GetActiveScene().name, this.transform.position);
        }
    }
}
