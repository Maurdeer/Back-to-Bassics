using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinning : MonoBehaviour
{
    public float minSpeed;
    public float maxSpeed;
    public float speed;
    public bool ccw;
    private float speedUp;
    private const float accelerate = 0.05f;
    private float halfMaxSpeed;
    private bool fakeOut;
    private Quaternion initialRotation;

    private void Start()
    {
        halfMaxSpeed = maxSpeed / 2f;
    }

    private void FixedUpdate()
    {
        if (speedUp < speed)
        {
            speedUp += accelerate;
        }
        else if (speed < maxSpeed)
        {
            speedUp = speed;
        }
        else
        {
            speedUp = maxSpeed;
        }
        Quaternion rotateTo = transform.rotation * Quaternion.AngleAxis((ccw ? -1 : 1) * 90, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotateTo, Time.deltaTime * speedUp);
        // Fake out attack
        if (fakeOut)
        {
            float angle = Quaternion.Angle(initialRotation, transform.rotation);
            if (Mathf.Abs(angle - 180f) < 5f)
            {
                speedUp /= 2;
                ccw = !ccw;
                fakeOut = false;
            }
        }
    }

    public void ChangeDirection(float newSpeedUp)
    {
        speedUp = newSpeedUp;
        ccw = !ccw;
    }

    public void ChangeDirectionRandomSpeed()
    {
        speedUp = Random.Range(0f, halfMaxSpeed);
        speed = Random.Range(halfMaxSpeed, maxSpeed);
        ccw = !ccw;
    }

    public void ReduceSpeed(float newSpeedUp)
    {
        speedUp = newSpeedUp;
    }

    public void FakeOut(float newSpeedUp)
    {
        fakeOut = true;
        initialRotation = transform.rotation;
        speedUp = newSpeedUp;
        ccw = !ccw;
    }
}
