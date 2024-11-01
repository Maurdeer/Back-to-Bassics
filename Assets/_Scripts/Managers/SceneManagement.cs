using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    [SerializeField] private UnityEvent OnFirstTime;
    private static bool reloaded;
    private void Start()
    {
        if (!reloaded)
        {
            OnFirstTime.Invoke();
            reloaded = false;
        }
    }
    public void LoadScene(string scene)
    {
        if (SceneManager.GetActiveScene().name == scene) 
        {
            reloaded = true;
        }
        SceneManager.LoadSceneAsync(scene);
    }
    public void ReloadCurrentScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }
}
