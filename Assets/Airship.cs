using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airship : MonoBehaviour
{
    [SerializeField] int speed = 1;
    [SerializeField] int health = 100;
    private int notWaiting = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (-speed * Time.deltaTime * notWaiting));
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
