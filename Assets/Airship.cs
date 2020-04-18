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

    void OnDrawGizmosSelected()
    {
        Renderer rend = gameObject.GetComponent<MeshRenderer>();
        // A sphere that fully encloses the bounding box.
        Vector3 center = rend.bounds.center;
        float radius = rend.bounds.extents.magnitude;

        

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(center, radius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, rend.bounds.extents.x);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(center, rend.bounds.extents.y);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, rend.bounds.extents.z);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, rend.bounds.extents * 2);
    }
}
