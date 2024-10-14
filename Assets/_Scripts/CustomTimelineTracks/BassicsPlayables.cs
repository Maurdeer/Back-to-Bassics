using UnityEngine;
using UnityEngine.Playables;

public class BassicsPlayable<T> : PlayableAsset where T: BassicsPlayableBehaviour
{
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        throw new System.NotImplementedException();
    }
}

public abstract class BassicsPlayableBehaviour : PlayableBehaviour
{
    protected abstract void OnStart();
    protected abstract void OnUpdate();
    protected abstract void OnComplete();
    protected abstract void OnAbort();
}