using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{

    [SerializeField] Transform respawnPoint;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Kill"))
        {
            Debug.Log("kill");
            Respawn();
        }
        if (transform.position.z > 300)
        {
            Respawn();
        }
        if (transform.position.z < -300)
        {
            Respawn();
        }
        if (transform.position.y < -50)
        {
            Respawn();
        }
        if (transform.position.y > 200)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        transform.position = respawnPoint.position;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
