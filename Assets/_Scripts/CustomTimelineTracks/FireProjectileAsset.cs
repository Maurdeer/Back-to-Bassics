using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class FireProjectileAsset : PlayableAsset
{
    public FireProjectileBehaviour template;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<FireProjectileBehaviour>.Create(graph, template);
        return playable;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FireProjectileAsset))]
public class FireProjectileAssetEditor : Editor
{
    private readonly string[] directions = { "North", "South", "East", "West" };
    public override void OnInspectorGUI()
    {
        FireProjectileAsset fpa = target as FireProjectileAsset;
        fpa.template.useDefault = EditorGUILayout.Toggle("Use Default Projectile Settings", fpa.template.useDefault);
        if (!fpa.template.useDefault)
        {
            fpa.template.projectileRef = EditorGUILayout.ObjectField("Projectile Prefab", fpa.template.projectileRef, typeof(GameObject), false) as GameObject;  
        }
        fpa.template.fireDirection = (Direction)EditorGUILayout.EnumPopup("Slash Direction", fpa.template.fireDirection);
        fpa.template.fireDistance = EditorGUILayout.FloatField("Fire Distance", fpa.template.fireDistance);
        EditorUtility.SetDirty(fpa);
    }
}
#endif
