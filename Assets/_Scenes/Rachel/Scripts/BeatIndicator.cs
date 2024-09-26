using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BeatIndicator : Conductable
{
    [SerializeField] private Sprite beatSprite;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        Enable();
    }

    protected override void OnFullBeat()
    {
        StartCoroutine(IBeat());
    }

    private IEnumerator IBeat()
    {
        Sprite prevSprite = image.sprite;

        image.sprite = beatSprite;

        yield return new WaitForSeconds(0.15f);

        image.sprite = prevSprite;
    }
}
