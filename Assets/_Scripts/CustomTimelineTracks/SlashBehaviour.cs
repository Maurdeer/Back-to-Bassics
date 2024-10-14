using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class SlashBehaviour : PlayableBehaviour
{
    public SlashNode node;
    private bool _performed;
    private SlashAction _cachedSlashRef;
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
        if (_performed) return;
        _performed = true;
        _cachedSlashRef = info.output.GetUserData() as SlashAction;
        if (_cachedSlashRef == null )
        {
            //Debug.LogError($"{this} Node did not perform slash animation, enemy reference was null");
            return;
        }
        node.slashLengthInBeats = (float)playable.GetDuration();
        _cachedSlashRef.Slash(node);
    }
}
