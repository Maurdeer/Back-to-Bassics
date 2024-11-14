using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoverScreenAction : EnemyAction
{
    [Header("Cover Screen Action")]
    [SerializeField] private GameObject screenCoverPrefab;
    [SerializeField] private int maxNumOfBeats = 4;
    [SerializeField] private Vector3 initialScale = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] private Vector3 targetScale = new Vector3(0.8f, 0.8f, 0.8f);
    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private float fadeDuration = 0.5f;

    private int numOfBeats = 4;
    private GameObject screenCoverInstance;
    private Image screenCoverImage;
    private bool isFadingOut = false;

    /// <summary>
    /// Creates a screen cover if it does not exist
    /// </summary>
    protected override void OnStartAction()
    {
        numOfBeats = maxNumOfBeats;
        screenCoverInstance = Instantiate(screenCoverPrefab);
        screenCoverInstance.transform.SetParent(UIManager.Instance.transform, false);
        screenCoverInstance.transform.localScale = initialScale;
        screenCoverImage = screenCoverInstance.GetComponent<Image>();
        if (screenCoverImage == null)
        {
            Debug.LogError("Screen cover does not have an Image component.");
            return;
        }
        StartCoroutine(ScaleUp());
        Conductor.Instance.OnFullBeat += UpdateBeat;
    }

    /// <summary>
    /// Fade out screen cover
    /// </summary>
    protected override void OnStopAction()
    {
        if (!isFadingOut)
        {
            StartCoroutine(FadeOut());
        }
        Conductor.Instance.OnFullBeat -= UpdateBeat;
    }

    /// <summary>
    /// Increases size of image
    /// </summary>
    private IEnumerator ScaleUp()
    {
        float elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            screenCoverInstance.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        screenCoverInstance.transform.localScale = targetScale;
    }

    /// <summary>
    /// Fades out image by decreasing alpha
    /// </summary>
    private IEnumerator FadeOut()
    {
        isFadingOut = true;
        float elapsedTime = 0f;
        Color initialColor = screenCoverImage.color;
        while (elapsedTime < fadeDuration)
        {
            screenCoverImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Lerp(1, 0, elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        screenCoverImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
        isFadingOut = false;
        Destroy(screenCoverInstance);
    }

    /// <summary>
    /// Updates beat count
    /// </summary>
    private void UpdateBeat()
    {
        numOfBeats--;
        if (numOfBeats <= 0)
        {
            StopAction();
        }
    }
}