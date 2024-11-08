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
    public float timeToFade;



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


        fadingIn();
        loadText.SetText("");
        loadingScreen.SetActive(false);
        loadSceneParent.SetActive(false);


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

        yield return new WaitForSeconds(1);
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        //SceneManager.LoadScene(scene);
        loadingScreen.SetActive(true);
        fadeOut = true;


        fadingOut();

        

        while (!operation.isDone)
        {

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            loadText.SetText("loading..." + ((int)progress * 100) + " %");

            yield return null;

        }
        fadingIn();

        loadText.SetText("");
        loadingScreen.SetActive(false);
        loadSceneParent.SetActive(false);


    }

    void fadingIn()
    {

        while (fadeIn)
        {
            canvasScreen.alpha -= timeToFade * Time.deltaTime;
            if (canvasScreen.alpha <= 0)
            {
                fadeIn = false;
                fadeOut = true;
                canvasScreen.alpha = 0;

            }
        }
    }

    void fadingOut()
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
        }
        
    }


}
