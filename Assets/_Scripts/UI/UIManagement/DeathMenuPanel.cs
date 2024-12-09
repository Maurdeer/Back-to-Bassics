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
        Debug.Log("Opening Death Menu");
        deathMenuPanelAnimator.Play("ShowDeathMenuPanel");
    }
    public void HideDeathMenuPanel()
    {
        Debug.Log("Hiding Death Menu");
        deathMenuPanelAnimator.Play("HideDeathMenuPanel");
    }
}
