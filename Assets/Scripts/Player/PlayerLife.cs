﻿using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] Transform respawnPoint;
    [Header("Movement boundaries")]
    [SerializeField] int zMax = 300;
    [SerializeField] int zMin = -300;
    [SerializeField] int yMax = 200;
    [SerializeField] int yMin = -50;

    void Update()
    {
        // if(Input.GetButtonDown("Kill") || transform.position.z < zMin || transform.position.z > zMax || transform.position.y < yMin || transform.position.y > yMax)

        if (Input.GetButtonDown("Kill"))
        {
            Respawn();
        }
        if (transform.position.z > zMax)
        {
            Respawn();
        }
        if (transform.position.z < zMin) // Min Check after Max Check? (Readability)
        {
            Respawn();
        }
        if (transform.position.y < yMin)
        {
            Respawn();
        }
        if (transform.position.y > yMax)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        transform.position = respawnPoint.position;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero; // reset velocity to stop pre respawn movement
    }
}