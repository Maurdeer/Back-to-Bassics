using System;
using System.Collections;
using UnityEngine;

public class IndicatorLine : Conductable
{
    [SerializeField][Range(0, 1f)] private float fadeDuration = 0.75f;

    [NonSerialized] public bool isRight;

    private Rigidbody2D rb;
    private CanvasGroup canvasGroup;

    private float spd = 50f;
    private float originalX;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0; // Start fully transparent
        StartCoroutine(FadeIn());

        originalX = transform.position.x;
    }

    void Update()
    {
        try
        {
            spd = (Math.Abs(originalX) + (isRight ? -1 : 1) * 400f) * Conductor.Instance.spb;
        }
        catch (Exception e)
        {
            Destroy(gameObject);
        }
        
        
        rb.velocity = new Vector3((isRight ? -1 : 1) * spd, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<BeatIndicator>(out _))
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0;
        float timer = 1f;
        try
        {
            timer = Conductor.Instance.spb * fadeDuration;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Destroy(gameObject);
        }
        

        while (elapsedTime < timer)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / timer);
            yield return null;
        }

        canvasGroup.alpha = 1;
    }
}
