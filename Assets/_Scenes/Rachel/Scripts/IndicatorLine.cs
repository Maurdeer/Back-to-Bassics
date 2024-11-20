using System;
using System.Collections;
using UnityEngine;

public class IndicatorLine : MonoBehaviour
{
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
        spd = (Math.Abs(originalX) + (isRight ? -1 : 1) * 400f) * Conductor.Instance.spb;

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
        float timer = Conductor.Instance.spb * 0.75f;

        while (elapsedTime < timer)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / timer);
            yield return null;
        }

        canvasGroup.alpha = 1;
    }
}
