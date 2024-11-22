using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;


public class SceneManagement : MonoBehaviour
{
    [SerializeField] private UnityEvent OnFirstTime;
    private static bool reloaded;

    //screen loading
    public GameObject loadSceneParent;
    private GameObject loadingScreen;
    private TMP_Text loadText;
    private CanvasGroup canvasScreen;
    private bool fadeIn = false;
    private bool fadeOut = false;
    [Range(1, 20f)] public float timeToFade = 10f;



    private void Start()
    {
        if (!reloaded)
        {
            OnFirstTime.Invoke();
            reloaded = false;
        }

        loadText = loadSceneParent.transform.Find("LoadText").GetComponent<TMPro.TextMeshProUGUI>();
        

        loadingScreen = loadSceneParent.transform.Find("Panel").gameObject;
        canvasScreen = loadingScreen.GetComponent<CanvasGroup>();

        loadSceneParent.SetActive(true);
        loadingScreen.SetActive(true);
        canvasScreen.alpha = 1.0f;
        fadeIn = true;
        loadText.SetText("loading...100%");

        StartCoroutine(fadingIn(10f));
    }
    public void ChangeScene(string scene)
    {
        if (SceneManager.GetActiveScene().name == scene) 
        {
            reloaded = true;
        }

        //SceneManager.LoadSceneAsync(scene);
        loadSceneParent.SetActive(true);
        loadText.SetText("");
        loadingScreen.SetActive(false);
        StartCoroutine(LoadSceneAsynchronously(scene));
    }
    public void ReloadCurrentScene()
    {
        //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        ChangeScene(SceneManager.GetActiveScene().name);
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }


    IEnumerator LoadSceneAsynchronously(string scene)
    {
        loadingScreen.SetActive(true);
        fadeOut = true;
        yield return fadingOut(10f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene); 

        while (!operation.isDone)
        {

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            loadText.SetText("loading..." + ((int)progress * 100) + " %");

            yield return null;

        }
    }

    private IEnumerator fadingIn(float speed)
    {

        while (fadeIn)
        {
            canvasScreen.alpha -= timeToFade * Time.deltaTime;
            if (canvasScreen.alpha <= 0)
            {
                fadeIn = false;
                fadeOut = true;
                loadText.SetText("");
                loadingScreen.SetActive(false);
                loadSceneParent.SetActive(false);
                canvasScreen.alpha = 0;
            }
            yield return new WaitForSeconds(1 / (speed * 10));
        }
    }

    private IEnumerator fadingOut(float speed)
    {
        while (fadeOut)
        {
          
            canvasScreen.alpha += timeToFade * Time.deltaTime;
            if (canvasScreen.alpha >= 1)
            {
                fadeOut = false;
                fadeIn = true;
                canvasScreen.alpha = 1;
            }
            yield return new WaitForSeconds(1 / (speed * 10));
        }
        
    }
}
