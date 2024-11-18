using UnityEngine;
/// <summary>
/// Attach this to any gameobject with a collider that will destroy projectiles
/// </summary>
[RequireComponent(typeof(Collider))]
public class ProjectileDestroyer : MonoBehaviour 
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Projectile>()?.Reset();
    }

    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.GetComponent<Projectile>().Reset();
    }
}
