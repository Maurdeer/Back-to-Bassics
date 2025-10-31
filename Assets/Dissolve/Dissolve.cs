using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// I'm assuming you can just attatch this to the note prefab and make sure this finishes before you destroy it
/// </summary>
public class Dissolve : MonoBehaviour
{
    [SerializeField] private float dissolveTime = 0.75f;

    private SpriteRenderer[] _spriteRenderers;
    private Material[] _materials;

    private int _dissolveAmount = Shader.PropertyToID("_DissolveAmount");

    void Start()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        _materials = new Material[_spriteRenderers.Length];
        for (int i = 0; i < _spriteRenderers.Length; ++i)
        {
            _materials[i] = _spriteRenderers[i].material;
        }

        // Play();
    }
    
    public void  Reset()
    {
        for (int i = 0; i < _materials.Length; ++i)
        {
            _materials[i].SetFloat(_dissolveAmount, 0);
        }
    }

    public IEnumerator Play() {
        yield return StartCoroutine(Vanish());
        // yield return null;
    }

    private IEnumerator Vanish() {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveTime) {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(0, 1.3f, (elapsedTime / dissolveTime));

            for (int i = 0; i < _materials.Length; ++i) {
                _materials[i].SetFloat(_dissolveAmount, lerpedDissolve);
            }
            yield return null;
        }
    }
}
