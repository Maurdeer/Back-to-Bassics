using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeHitBox : MonoBehaviour, IAttackRequester
{
    [SerializeField] private int damage;
    [field: SerializeField] public int health { get; private set; }
    [SerializeField] private Spinning spinner;
    private float resetSpeed = 0f;
    [SerializeField] private float fakeOutChance = 0.2f;
    public bool OnRequestDeflect(IAttackReceiver receiver)
    {
        PlayerBattlePawn player = receiver as PlayerBattlePawn;

        // Did player deflect in correct direction?
        if (player == null 
            || !DirectionHelper.MaxAngleBetweenVectors(spinner.ccw ? Vector2.left : Vector2.right, player.SlashDirection, 5f)) 
            return false;

        // (TEMP) Manual DEBUG UI Tracker -------
        UIManager.Instance.IncrementParryTracker();
        //---------------------------------------
        health -= 1;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            // Limit max speed of spinner
            if (spinner.speed < spinner.maxSpeed) 
            {
                spinner.speed += resetSpeed;
                spinner.speed = Mathf.Min(spinner.speed, spinner.maxSpeed);
            }
            // Randomize fake out chance
            float rand = Random.Range(0f, 1f);
            if (rand <= fakeOutChance)
            {
                spinner.FakeOut(resetSpeed);
            }
            else
            {
                spinner.ChangeDirection(resetSpeed);
            }

        }
        resetSpeed += 0.2f;
        // (TEMP)----------- This is dumb IK---------------------
        BattleManager.Instance.Enemy.Damage(1);
        //-------------------------------------------------------
        player.CompleteAttackRequest(this);
        return true;
    }
    public bool OnRequestBlock(IAttackReceiver receiver)
    {
        //// (TEMP) Manual DEBUG UI Tracker -------
        //UIManager.Instance.IncrementBlockTracker();
        ////---------------------------------------
        ////_hitPlayerPawn.Lurch(_dmg);
        //_hitPlayerPawn.CompleteAttackRequest(this);
        //Destroy();
        return true;
    }
    public bool OnRequestDodge(IAttackReceiver receiver)
    {
        // Nothing Happens Here :o
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerBattlePawn pawn))
        {
            if (pawn.ReceiveAttackRequest(this))
            {
                pawn.Damage(damage);
                pawn.CompleteAttackRequest(this);
                // Decrease spinner speed if player is hit
                if (spinner.speed > spinner.minSpeed) {
                    spinner.speed /= 2;
                    spinner.speed = Mathf.Max(spinner.speed, spinner.minSpeed);
                    spinner.ReduceSpeed(spinner.speed / 2);
                    resetSpeed = spinner.speed / 2;
                }
            }
        }
    }
}
