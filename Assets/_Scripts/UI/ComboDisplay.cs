using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ComboDisplay : MonoBehaviour
{
    [SerializeField] private Sprite slashArrow;
    [SerializeField] private Sprite dodgeArrow;
    private int _curridx = 0;
    private Animator _animator;
    private void Awake()
    {
        ResetCombo();
        _animator = GetComponent<Animator>();
    }

    private void RollCombo()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).SetSiblingIndex(i - 1);
        }
    }

    public void AddCombo(char combo) 
    {
        if (_curridx == 4)
        {
            RollCombo();
            _curridx = 3;
        }
        Transform child = transform.GetChild(_curridx);
        switch (combo)
        {
            case 'W':
                child.GetComponent<Image>().sprite = slashArrow;
                child.rotation = Quaternion.Euler(0, 0, 90f);
                break;
            case 'E':
                child.GetComponent<Image>().sprite = slashArrow;
                child.rotation = Quaternion.Euler(0, 0, -90f);
                break;
            case 'S':
                child.GetComponent<Image>().sprite = slashArrow;
                child.rotation = Quaternion.Euler(0, 0, 180f);
                break;
            case 'N':
                child.GetComponent<Image>().sprite = slashArrow;
                child.rotation = Quaternion.Euler(0, 0, 0f);
                break;
            case 'w':
                child.GetComponent<Image>().sprite = dodgeArrow;
                child.rotation = Quaternion.Euler(0, 0, 90f);
                break;
            case 'e':
                child.GetComponent<Image>().sprite = dodgeArrow;
                child.rotation = Quaternion.Euler(0, 0, -90f);
                break;
            case 's':
                child.GetComponent<Image>().sprite = dodgeArrow;
                child.rotation = Quaternion.Euler(0, 0, 180f);
                break;
            case 'n':
                child.GetComponent<Image>().sprite = dodgeArrow;
                child.rotation = Quaternion.Euler(0, 0, 0f);
                break;
            default:
                return;
        }
        child.GetComponent<Image>().color = Color.white;
        child.gameObject.SetActive(true);
        _animator.Play("FadeIn");
        _curridx = Mathf.Min(_curridx + 1, 4);
    }
    public void ValidCombo()
    {
        foreach (Transform child in transform)
        { 
            child.GetComponent<Image>().color = Color.green;
        }
    }
    public void ResetCombo()
    {  
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    public void HideCombo()
    {
        _curridx = 0;
        _animator.Play("FadeOut");
        foreach (Transform child in transform)
        {
            child.GetComponent<Image>().color = Color.red;
        }
        StartCoroutine(DelayResetCombo());
    }

    private IEnumerator DelayResetCombo()
    {
        //waits for animation to finish before resetting
        yield return new WaitForSeconds(.19f);

        ResetCombo();
        Debug.Log("coroutine done");

    }
}
