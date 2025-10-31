using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoverScreenAction : EnemyAction
{
    [Header("Cover Screen Action")]
    [SerializeField] private GameObject screenCoverPrefab;
    [SerializeField] private int maxNumOfBeats = 4;
    [SerializeField] private Vector3 initialScale = new Vector3(0.3f, 0.3f, 0.3f);
    [SerializeField] private Vector3 targetScale = new Vector3(0.8f, 0.8f, 0.8f);
    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private float fadeDuration = 0.5f;

    private int numOfBeats = 4;
    private GameObject prevInstance;
    private GameObject screenCoverInstance;
    private Image screenCoverImage;
    private bool isFadingOut = false;

    // WRECKON RELATED
    private float pastMisses;
    private float pastParries;
    /// <summary>
    /// Creates a screen cover if it does not exist
    /// </summary>
    protected override void OnStartAction()
    {
        if (screenCoverInstance != null)
        {
            prevInstance = screenCoverInstance;
            StopAllCoroutines();
            isFadingOut = false;
        }
        // [WRECKON] Start the check:
        pastParries = UIManager.Instance.parryCount;
        pastMisses = UIManager.Instance.missCount;
        //==========================
        numOfBeats = maxNumOfBeats;
        screenCoverInstance = Instantiate(screenCoverPrefab);
        screenCoverInstance.SetActive(false);
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

        // (10/14/25 Joseph) This is definitely jank af but I'll consult with Ryan on this one.
        EnemyBattlePawn enemyUser = this.gameObject.GetComponentInParent<EnemyBattlePawn>();
        if (enemyUser == null) return;
        Debug.Log("Found a parent");
        enemyUser.OnExitBattle += delegate {
            Destroy(screenCoverInstance);
        }; 
    }

    /// <summary>
    /// Fade out screen cover
    /// </summary>
    protected override Coroutine OnStopAction()
    {
        Conductor.Instance.OnFullBeat -= UpdateBeat;
        return !isFadingOut ? StartCoroutine(FadeOut()) : null;
    }

    /// <summary>
    /// Increases size of image
    /// </summary>
    private IEnumerator ScaleUp()
    {
        float duration = (timelineDurationInBeats - 1) * Conductor.Instance.spb;
        parentPawnSprite.Animator.SetFloat("speed", 1 / duration);
        parentPawnSprite.Animator.Play("cover_screen");
        yield return new WaitForSeconds(duration);
        screenCoverInstance.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            screenCoverInstance.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        screenCoverInstance.transform.localScale = targetScale;
        if (prevInstance != null)
        {
            Destroy(prevInstance);
        }
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
            if (screenCoverImage == null) StopCoroutine(FadeOut());
            screenCoverImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Lerp(1, 0, elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        screenCoverImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
        isFadingOut = false;
        // [WRECKON] CHECK IF Player did do it!!!
        TaskCheck();
        //==========================
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

    private void TaskCheck()
    {
        if (UIManager.Instance.parryCount >= pastParries + 2 && UIManager.Instance.missCount == pastMisses)
        {
            UIManager.Instance.WreckconQuests.MarkAchievement(5);
        }
    }
}