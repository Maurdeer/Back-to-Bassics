using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject follower;

    private Vector3 prevTargetPos;

    private void Start()
    {
        prevTargetPos = GameManager.Instance.PC.transform.position;
    }
    private void FixedUpdate()
    {
        follower.transform.position = GameManager.Instance.PC.transform.position - prevTargetPos;
        prevTargetPos = GameManager.Instance.PC.transform.position;
    }
}
