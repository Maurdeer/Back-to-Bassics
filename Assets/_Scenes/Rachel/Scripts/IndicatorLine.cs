using System;
using UnityEngine;

public class IndicatorLine : MonoBehaviour
{
    [NonSerialized] public bool isRight;

    private Rigidbody2D rb;

    private float spd = 50f;
    private float originalX;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalX = transform.position.x;
    }

    void Update()
    {
        spd = (Math.Abs(originalX) - 400f) * Conductor.Instance.spb;

        rb.velocity = new Vector3((isRight ? -1 : 1) * spd, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<BeatIndicator>(out _))
        {
            Destroy(gameObject);
        }
    }
}
