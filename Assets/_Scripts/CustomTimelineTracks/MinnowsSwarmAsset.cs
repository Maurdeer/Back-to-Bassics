using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class MinnowsSwarmAsset : PlayableAsset
{
    public MinnowsSwarmBehaviour template;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<MinnowsSwarmBehaviour>.Create(graph, template);
        return playable;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MinnowsSwarmAsset))]
public class MinnowsSwarmAssetEditor : Editor
{
    private readonly string[] directions = { "North", "South", "East", "West" };

    public override void OnInspectorGUI()
    {
        MinnowsSwarmAsset msa = target as MinnowsSwarmAsset;
        msa.template.choice = (MinnowsSwarmActionChoice)EditorGUILayout.EnumPopup("MinnowsSwarmAction", msa.template.choice);

        switch (msa.template.choice)
        {
            case MinnowsSwarmActionChoice.Idle:
            case MinnowsSwarmActionChoice.EnterBattle:
                msa.template.minnowsDirection = (Direction)EditorGUILayout.EnumPopup("Go to Direction", msa.template.minnowsDirection);
                break;
            case MinnowsSwarmActionChoice.Slash:
                msa.template.slash_node.slashDirection = (Direction)EditorGUILayout.EnumPopup("Slash Direction", msa.template.slash_node.slashDirection);
                msa.template.slash_node.isCharged = EditorGUILayout.Toggle("Is Charged", msa.template.slash_node.isCharged);
                msa.template.slash_node.staggersParent = EditorGUILayout.Toggle("Staggers Parent", msa.template.slash_node.staggersParent);
                msa.template.slash_node.dmg = EditorGUILayout.IntField("Damage", msa.template.slash_node.dmg);
                EditorGUILayout.LabelField("Dodge Directions");
                if (msa.template.slash_node.dodgeDirections == null
                    || msa.template.slash_node.dodgeDirections.Length < directions.Length)
                {
                    msa.template.slash_node.dodgeDirections = new Direction[directions.Length];
                }
                EditorGUI.indentLevel++;
                for (int i = 0; i < directions.Length; i++)
                {
                    bool dodgeDirection = EditorGUILayout.Toggle(directions[i], msa.template.slash_node.dodgeDirections[i] != Direction.None);
                    msa.template.slash_node.dodgeDirections[i] = dodgeDirection ? (Direction)(i + 1) : Direction.None;
                }
                EditorGUI.indentLevel--;
                break;
            case MinnowsSwarmActionChoice.Fire:
                msa.template.fireDirection = (Direction)EditorGUILayout.EnumPopup("Fire Direction", msa.template.fireDirection);
                msa.template.fireDistance = EditorGUILayout.FloatField("Fire Distance", msa.template.fireDistance);
                break;
        }

        EditorUtility.SetDirty(msa);
    }
}
#endif
