using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BeatIndicator : Conductable
{
    [Header("Vignette")]
    [SerializeField] private bool isVignette;
    [SerializeField] private Volume vignetteVolume;

    private VolumeProfile vignetteProfile;
    private UnityEngine.Rendering.Universal.Vignette vignette;

    [Header("Indicator")]
    [SerializeField] private Sprite beatSprite;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();

        vignetteProfile = vignetteVolume.sharedProfile;
        vignetteVolume.profile.TryGet(out vignette);
    }

    private void Start()
    {
        Enable();
    }

    protected override void OnFullBeat()
    {
        if (isVignette)
        {
            StartCoroutine(IVignette());
        }
        StartCoroutine(IBeat());
    }

    private IEnumerator IBeat()
    {
        Sprite prevSprite = image.sprite;

        image.sprite = beatSprite;

        yield return new WaitForSeconds(0.1f);

        image.sprite = prevSprite;
    }

    private IEnumerator IVignette()
    {
        if (vignette)
        {
            float originalIntensity = vignette.intensity.value;

            vignette.intensity.value += 0.2f;

            yield return new WaitForSeconds(0.1f);

            vignette.intensity.value = originalIntensity;
        }
    }
}
