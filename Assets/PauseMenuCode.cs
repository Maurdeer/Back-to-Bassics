using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameStateMachine;

public class PauseMenuCode : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    private Coroutine pausingThread;
    public void TogglePause()
    {
        if (GameManager.Instance.GSM.IsOnState<Pause>())
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
    private void Pause ()
    {
        m_animator.Play("show");

        // Temp and bad!
        pausingThread = StartCoroutine(TimeBeforePause());
        GameManager.Instance.GSM.Transition<Pause>();
    }
    public void Resume()
    {
        if (pausingThread != null) StopCoroutine(pausingThread);
        m_animator.Play("hide");
        if (GameManager.Instance.GSM.PrevState.GetType() == typeof(Battle))
        {
            GameManager.Instance.GSM.Transition<Battle>();
        } 
        else
        { 
            GameManager.Instance.GSM.Transition<WorldTraversal>();
        }
        
    }

    public void LoadSettings ()
    {
        Debug.Log("Loading settings");
    }

    public void ExitGame ()
    {
        Application.Quit();
        Time.timeScale = 1f;
    }
    public void SaveGame()
    {

    }
    public void LoadGame()
    {

    }
    private IEnumerator TimeBeforePause()
    {
        yield return new WaitUntil(() => m_animator.GetCurrentAnimatorStateInfo(0).IsName("shown"));
        Time.timeScale = 0f;
    }
}
