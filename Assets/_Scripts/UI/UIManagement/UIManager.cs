using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
[DisallowMultipleComponent]
public partial class UIManager : Singleton<UIManager>
{
    [field: Header("UI General")]
    [field: SerializeField] public PauseMenuCode pauseMenu { get; private set; }
    [field: SerializeField] public TextMeshProUGUI interactableText { get; private set; }
    [SerializeField] private Animator credits_animator;
    private Coroutine _sic;
    private Coroutine _hic;
    public Transform SavePanel;
    public Animator PauseButtonAnimator;

    [Header("Wreckcon")]
    public PersistentDataTracker PersistentDataTracker;
    public WreckconQuests WreckconQuests;
    public void Awake()
    {
        InitializeSingleton();
    }
    public void ShowInteractableUI()
    {
        if (_hic != null) StopCoroutine(_hic);
        _sic = StartCoroutine(ShowInteractableCoroutine(0.2f));
    }
    public void HideInteractableUI()
    {
        if (_sic != null) StopCoroutine(_sic);
        _hic = StartCoroutine(HideInteractableCoroutine(0.2f));
    }
    public void BeginCredits()
    {
        credits_animator.Play("BeginCreditsScroll");
    }
    private IEnumerator ShowInteractableCoroutine(float duration)
    {
        Color alphaChange;
        float count = 0;
        while (interactableText.color.a < 1)
        {
            alphaChange = interactableText.color;
            count += Time.deltaTime;
            alphaChange.a = count / duration;
            interactableText.color = alphaChange;
            yield return null;
        }
    }
    private IEnumerator HideInteractableCoroutine(float duration)
    {
        Color alphaChange;
        float count = duration;
        while (interactableText.color.a > 0)
        {
            alphaChange = interactableText.color;
            count -= Time.deltaTime;
            alphaChange.a = count / duration;
            interactableText.color = alphaChange;
            yield return null;
        }
    }
}
