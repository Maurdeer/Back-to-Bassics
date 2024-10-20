using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using static Unity.Burst.Intrinsics.X86;

public class SlashAsset : PlayableAsset
{
    public SlashBehaviour template;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SlashBehaviour>.Create(graph, template); 
        return playable;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SlashAsset))]
public class SlashAssetEditor : Editor
{
    private readonly string[] directions = { "North", "South", "East", "West"};

    public override void OnInspectorGUI()
    {
        SlashAsset sa = target as SlashAsset;
        sa.template.node.slashDirection = (Direction)EditorGUILayout.EnumPopup("Slash Direction", sa.template.node.slashDirection);
        sa.template.node.isCharged = EditorGUILayout.Toggle("Is Charged", sa.template.node.isCharged);
        sa.template.node.staggersParent = EditorGUILayout.Toggle("Staggers Parent", sa.template.node.staggersParent);
        sa.template.node.dmg = EditorGUILayout.IntField("Damage", sa.template.node.dmg);
        //string[] clipName = sa.template.node.preHitClip.name.Split('_');
        //sa.template.node.slashDirection = DirectionHelper.GetDirectionFromString(clipName[clipName.Length - 1]);
        EditorGUILayout.LabelField("Dodge Directions");
        if (sa.template.node.dodgeDirections == null
            ||  sa.template.node.dodgeDirections.Length < directions.Length)
        {
            sa.template.node.dodgeDirections = new Direction[directions.Length];
        }
        EditorGUI.indentLevel++;
        for (int i = 0; i < directions.Length; i++)
        {
            bool dodgeDirection = EditorGUILayout.Toggle(directions[i], sa.template.node.dodgeDirections[i] != Direction.None);
            sa.template.node.dodgeDirections[i] = dodgeDirection ? (Direction)(i + 1) : Direction.None; 
        }
        EditorGUI.indentLevel--;
        EditorUtility.SetDirty(sa);
    }
}
#endif

