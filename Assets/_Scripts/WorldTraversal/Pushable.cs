using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pushable : MonoBehaviour, Interactable
{
    private Vector3 direction;
    private Vector3 pushPosition;
    private bool isPushing;


    public int pushLength = 1;
    public float speed = 1f;
    
    public void Interact()
    {
        isPushing = true;
        pushPosition = transform.position + direction * pushLength;
    }

    void Update()
    {
        if (isPushing)
        {
            transform.position = Vector3.MoveTowards(transform.position, pushPosition, speed * Time.fixedDeltaTime);
            if (transform.position == pushPosition) isPushing = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //We only want Poncho's Interaction
        if (!isPushing && collision.collider.name == "PlayerPoncho")
        {
            ContactPoint contact = collision.contacts[0];
            direction = contact.normal.normalized;
        }
    }

}
