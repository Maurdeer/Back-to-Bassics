using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SyncDemoController : MonoBehaviour
{
    [SerializeField]
    private EnemyBattlePawn demoEnemy;
    
    [SerializeField]
    private TextMeshProUGUI tempoText;
    
    // Start is called before the first frame update
    void Start()
    {
        Conductor.Instance.BeginConducting(demoEnemy);
        OnUpdateTempo(null);
    }
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space down");
        }
    }

    void OnUpdateTempo(string tempo)
    {
        tempoText.text = $"Current Tempo: { tempo ?? "N/A" }";
    }
}
