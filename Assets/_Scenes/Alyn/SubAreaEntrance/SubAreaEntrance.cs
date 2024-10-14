using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubAreaEntrance : MonoBehaviour
{

    public string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger
        if (other.CompareTag("Player")) 
        {
            // Load scene
            StartCoroutine(LoadSceneAsync(sceneToLoad));
        }
/*        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger!"); // Debug log
            StartCoroutine(LoadSceneAsync(sceneToLoad));
        }
 */   }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Load scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null; // Wait for the next frame
        }
    }
}