using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicAttack : EnemyAction
{
    [SerializeField] private Direction[] directions;
    [SerializeField] private GameObject musicNoteRef;
    int currIdx;
    private void Start()
    {
        Pooler.Instance.PoolGameObject(musicNoteRef, 20);
    }
    protected override void OnStartAction()
    {
        currIdx = 0;
        parentPawnSprite.Animator.SetFloat("speed", 1 / Conductor.Instance.spb);
        parentPawnSprite.Animator.Play("music");
    }
    protected override void OnFullBeat()
    {
        if (currIdx < directions.Length && IsActive)
        {
            Projectile proj = Pooler.Instance.Pool(musicNoteRef).GetComponent<Projectile>();
            Vector3 directionVector = -(Vector3)DirectionHelper.GetVectorFromDirection(directions[currIdx]);
            parentPawnSprite.Animator.SetFloat("xdir", directionVector.x);
            proj.transform.position = BattleManager.Instance.Player.transform.position + (directionVector) * 6;
            // Take One Beat to fire
            var disp = (BattleManager.Instance.Player.transform.position - proj.transform.position);
            // /(2*Conductor.Instance.spb)
            proj.Fire(disp, 2);
            currIdx++;
            if (currIdx >= directions.Length)
            {
                StartCoroutine(WaitForProjectileToDestroy(proj));
                StopAction();
            }
        }
    }
    private IEnumerator WaitForProjectileToDestroy(Projectile proj)
    {
        yield return new WaitUntil(() => proj.isDestroyed);
        StopAction();
    }
}
