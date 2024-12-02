using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameStateMachine;

public class PauseMenuCode : MonoBehaviour
{
    public static bool GamePaused = false;

    public GameObject pauseMenuPanel;

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
        pauseMenuPanel.SetActive(true);
        GameManager.Instance.GSM.Transition<Pause>();
    }
    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
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
}
