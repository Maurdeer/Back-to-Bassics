using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Handle the showing and hiding the death menu panel
/// </summary>
public partial class UIManager
{
    [Header("Death Menu Panel")]
    [SerializeField] private Animator deathMenuPanelAnimator;

    public void ShowDeathMenuPanel()
    {
        deathMenuPanelAnimator.Play("ShowDeathMenuPanel");
    }
    public void HideDeathMenuPanel()
    {
        deathMenuPanelAnimator.Play("HideDeathMenuPanel");
    }
}
