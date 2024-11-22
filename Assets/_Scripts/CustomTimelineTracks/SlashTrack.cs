#if UNITY_EDITOR
using UnityEditor.Timeline;
#endif
using UnityEngine.Timeline;

[TrackClipType(typeof(SlashAsset))]
[TrackBindingType(typeof(SlashAction))]
public class SlashTrack : TrackAsset
{

#if UNITY_EDITOR
    // TODO: Using the CreateTrackMixer method, you can limit the length that someone can make a clip in each track.
    //protected override void OnCreateClip(TimelineClip clip)
    //{
    //    var director = TimelineEditor.inspectedDirector;
    //    SlashAction action = director.GetGenericBinding(this) as SlashAction;
    //    if (action != null)
    //    {
    //        clip.duration = action.minSlashTillHitInBeats;
    //    }
    //    base.OnCreateClip(clip);
    //}

    //private void OnValidate()
    //{
    //    var director = TimelineEditor.inspectedDirector;
    //    SlashAction action = director.GetGenericBinding(this) as SlashAction;
    //    foreach (clip)
    //    if (duration < action.minSlashTillHitInBeats)
    //    {
    //        duration = action.minSlashTillHitInBeats;
    //    }
    //}
#endif
}