using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image heartFill;
    [SerializeField] private TextMeshProUGUI healthText;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("No animator attached to " + gameObject.name);
            return;
        }
    }

    public void UpdateHealth(float curr, float max)
    {
        float fill = curr / max;
        heartFill.fillAmount = fill;
        healthText.text = $"{curr}";
        _animator.SetFloat("curr_health", fill);
    }
}
