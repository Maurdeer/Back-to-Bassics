using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killbox : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "PlayerPoncho") {
            GameManager.Instance.GSM.Transition<GameStateMachine.Death>();
        }
    }
}
