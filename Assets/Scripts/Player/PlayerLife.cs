using UnityEngine;

public class PlayerLife : Hittable
{
    [SerializeField] Transform respawnPoint = null;
    [Header("Movement boundaries")]
    [SerializeField] int zMax = 300;
    [SerializeField] int zMin = -300;
    [SerializeField] int yMax = 200;
    [SerializeField] int yMin = -50;

    void Update()
    {
        if (Input.GetButtonDown("Kill")
            || transform.position.z < zMin
            || transform.position.z > zMax
            || transform.position.y < yMin
            || transform.position.y > yMax)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        transform.position = respawnPoint.position;
        health = maxHealth;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero; // reset velocity to stop pre respawn movement
        GrapplingHook grapplingHook = GetComponentInChildren<GrapplingHook>();
        if (grapplingHook)
        {
            grapplingHook.StopGrapple();
        }
    }

    protected override void DestroyHittable()
    {
        Respawn();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 10) // 10 = forcefield Layer
        {
            Respawn();
        }
    }
}