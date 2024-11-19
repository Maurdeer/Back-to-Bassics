using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class LoadScreenScript : MonoBehaviour
{
    public GameObject loadingScreen; //panel
    public TMP_Text loadText;
    private CanvasGroup canvasScreen; // of panel


    public bool fadeIn = false;
    public bool fadeOut = false;
    public float timeToFade;

    // Start is called before the first frame update
    void Start()
    {
        canvasScreen = loadingScreen.GetComponent<CanvasGroup>();
        this.gameObject.SetActive(false);


        //LoadScreen(0);

        
    }


    // Update is called once per frame
    void Update()
    {
        

    }

    public void LoadScreen(int sceneId)
    {
        this.gameObject.SetActive(true);
        loadText.SetText("");
        loadingScreen.SetActive(false);
        StartCoroutine(LoadSceneAsynchronously(sceneId));
    }

    IEnumerator LoadSceneAsynchronously(int sceneId)
    {

        yield return new WaitForSeconds(2);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        loadingScreen.SetActive(true);
        fadeIn = true;
        
        


        while (!operation.isDone)
        {

            if (fadeIn)
            {

                if (canvasScreen.alpha < 1)
                {
                    canvasScreen.alpha += timeToFade * Time.deltaTime;
                    if (canvasScreen.alpha >= 1)
                    {
                        fadeIn = false;
                        fadeOut = true;

                    }
                }
            }
            else if (fadeOut)
            {
                if (canvasScreen.alpha > 0)
                {
                    canvasScreen.alpha -= timeToFade * Time.deltaTime;
                    if (canvasScreen.alpha <= 0)
                    {
                        fadeOut = false;
                    }
                }
            }
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            
            loadText.SetText("loading..." + progress * 100 + " %");
            
            yield return null;
            
        }
        loadText.SetText("");
        loadingScreen.SetActive(false);
        this.gameObject.SetActive(false);
        

    }

}

