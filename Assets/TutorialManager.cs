using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private BossAI _bassicsAI;
    [SerializeField] private EnemyBattlePawn _enemyBattlePawn;
    [SerializeField] private GameObject comboList;
    [SerializeField] private int numFailuresForTutorial = 3;
    private int _currentStage = -1;
    public bool CheckForSlash
    {
        get => _currentStage <= 1;
    }
    public bool CheckForDodge
    {
        get => _currentStage == 2;
    }
    private bool _inTutorialState = false;
    private int tutorialMisses;
    private bool _inTutorialPause;
    private Vector2 _expectedSlashDirection;
    public Vector2 ExpectedDirection
    {
        get => _expectedSlashDirection;
    }
    public bool PausedForTutorial
    {
        get => _inTutorialPause;
    }
    public bool TutorialEnabled
    {
        get => _inTutorialState;
    }

    private void Awake()
    {
        bool wasInitialized = InitializeSingleton(this);
        if (!wasInitialized)
        {
            Debug.LogError("Tutorial Manager Already Initialized!");
        }
    }

    void Start()
    {
        _enemyBattlePawn.OnEnemyStaggerEvent += ShowComboList;
        _enemyBattlePawn.OnEnemyUnstaggerEvent += HideComboList;
        _bassicsAI.OnEnemyStageTransition += delegate
        {
            tutorialMisses = 0;
            _currentStage++;
        };
    }

    public void ShowComboList()
    {
        comboList.SetActive(true);
    }

    public void HideComboList()
    {
        comboList.SetActive(false);
    }
    
    public void Pause(Vector2 slashDirection)
    {
        Conductor.Instance.PauseCondcutor();
        _enemyBattlePawn.PawnSprite.Animator.speed = 0f;
        _expectedSlashDirection = slashDirection;
        _inTutorialPause = true;
    }

    public void Unpause()
    {
        Conductor.Instance.ResumeConductor();
        // Make sure this isn't messing things up
        _enemyBattlePawn.PawnSprite.Animator.speed = 1f;
        DialogueManager.Instance.customDialogueRunner.Stop();
        _inTutorialPause = false;
    }

    public void ModifyNumOfMisses(int x)
    {
        tutorialMisses += x;
        if (tutorialMisses >= numFailuresForTutorial) _inTutorialState = true;
        else if (tutorialMisses <= 0) {
            _inTutorialState = false;
            tutorialMisses = 0;
        }
    }

    void Update()
    {
        // if (Input.GetKeyDown("space")) Debug.Log($"Info: We expect {_expectedSlashDirection}, Tutorial is enabled: {_inTutorialState}, We're paused for that tutorial: {_inTutorialPause}");
        // if (!_inTutorialPause) return;
        // if ((Input.GetKeyDown("right") && _expectedSlashDirection.x > 0) ||
        // (Input.GetKeyDown("left") && _expectedSlashDirection.x < 0) ||
        // (Input.GetKeyDown("up") && _expectedSlashDirection.y > 0))
        // {
        //     Unpause();
        // }
    }

}
