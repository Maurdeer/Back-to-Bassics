#if UNITY_EDITOR
using UnityEditor.Timeline;
#endif
using UnityEngine.Timeline;

[TrackClipType(typeof(MinnowsSwarmAsset))]
[TrackBindingType(typeof(MinnowsSwarmAction))]
public class MinnowsSwarmTrack : TrackAsset
{
    
}
