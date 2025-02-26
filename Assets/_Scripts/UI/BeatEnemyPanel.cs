using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeatEnemyPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_enemyNameText;
    [SerializeField] private Animator m_animator;
    [SerializeField] private TextMeshProUGUI m_rankText;
    [SerializeField] private RankingGauge m_rankGauge;
    [SerializeField] private GameObject m_continueButton;
    public void PlayBattleVictory(string enemyName, ulong finalScore, ulong maxRankScore, string rank)
    {
        StartCoroutine(WinSequence(enemyName, finalScore, maxRankScore, rank));
    }
    private IEnumerator WinSequence(string enemyName, ulong finalScore, ulong maxRankScore, string rank)
    {
        m_enemyNameText.text = enemyName;   
        m_animator.Play("show_endscreen");
        yield return new WaitUntil(() => m_animator.GetCurrentAnimatorStateInfo(0).IsName("endscreen_shown"));
        UIManager.Instance.ScoreTracker.UpdateScore(0);

        yield return m_rankGauge.UpdateGauge(0, finalScore, maxRankScore);

        m_rankText.text = rank;
        m_rankText.gameObject.SetActive(true);
        m_continueButton.SetActive(true);
    }
    public void OnContinueButton()
    {
        m_rankText.gameObject.SetActive(false);
        m_continueButton.SetActive(false);
        m_animator.Play("hide_endscreen");
        BattleManager.Instance.EndBattleComplete();
    }
}
