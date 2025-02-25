using UnityEngine;
using UnityEngine.UI;

public class BeatIndicator : Conductable
{
    [SerializeField] private GameObject centerIndicator;
    private Animator m_animator;

    private Vector3 originalScale;
    private bool isBeat = false;
    private float elapsedTime = 0f;
    private float halfSpb;
    private bool hidden;
    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        hidden = true;
    }

    private void Start()
    {
        originalScale = centerIndicator.transform.localScale;

        Enable();
    }

    protected override void OnFullBeat()
    {
        elapsedTime = 0f;

        halfSpb = Conductor.Instance.spb / 2f;

        isBeat = true;
    }

    private void Update()
    {
        if (isBeat)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime < halfSpb)
            {
                float t = elapsedTime / halfSpb;
                float curvedT = Mathf.SmoothStep(0.9f, 1f, t);

                float scalePulse = Mathf.Lerp(1f, 1.25f, curvedT);
                centerIndicator.transform.localScale = originalScale * scalePulse;
            }
            else
            {
                isBeat = false;
                elapsedTime = 0f;
            }
        }
        else
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime < halfSpb)
            {
                float t = elapsedTime / halfSpb;
                float curvedT = Mathf.SmoothStep(0f, 1f, t);

                float scalePulse = Mathf.Lerp(1.25f, 1f, curvedT);
                centerIndicator.transform.localScale = originalScale * scalePulse;
            }
            else
            {
                isBeat = true;
                elapsedTime = 0f;
            }
        }
    }
    public void Show()
    {
        if (!hidden) return;
        hidden = false;
        m_animator.Play("show");
    }
    public void Hide()
    {
        if (hidden) return;
        hidden = true;
        m_animator.Play("hide");
    }
}
