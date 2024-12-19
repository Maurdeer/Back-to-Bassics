using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combo/SizzleCombo"), System.Serializable]
public class SizzleCombo : Combo
{
    [SerializeField] private int damage;
    [SerializeField] private int traversalRange;
    public int Damage => damage;
    private HashSet<Burnable> burnables;
    private PlayerController player;
    public GameObject light;

    public override void InBattle()
    {
        BattleManager.Instance.Enemy.ApplyStatusAilment<BurnAilment>(); //ailment
        BattleManager.Instance.Enemy.Damage(damage);
    }
    public override void InTraversal()
    {
        if (player == null)
        {
            burnables = Burnable.Instances;
            player = FindObjectOfType<PlayerController>();
        }
        Vector3 currentPos = player.gameObject.transform.position;
        //Debug.Log(burnables);
        Instantiate(light, currentPos, Quaternion.identity);
        foreach (Burnable burnable in burnables)
        {
            float dist = Vector3.Distance(burnable.transform.position, currentPos);
            if (dist < traversalRange)
            {
                burnable.GetComponent<Burnable>().Burn();
            }
        }
    }
}
