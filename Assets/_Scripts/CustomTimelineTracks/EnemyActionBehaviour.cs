using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyActionBehaviour : PlayableBehaviour
{
    private bool _performed;
    private EnemyAction _cachedActionRef;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
        if (_performed) return;
        _performed = true;
        _cachedActionRef = info.output.GetUserData() as EnemyAction;
        if (_cachedActionRef == null)
        {
            // Debug Error
            return;
        }
        _cachedActionRef.timelineDurationInBeats = (float)playable.GetDuration();
        _cachedActionRef.StartAction();
    }
}
