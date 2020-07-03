using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    bool right = true;
    private Rigidbody rb = null;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(right)
		{
            if(transform.position.x < 10.0f)
			{
                rb.AddForce(Vector3.right * 0.02f, ForceMode.VelocityChange);
			}
            else
			{
                right = false;
			}
		}
        else
		{
            if(transform.position.x > -10.0f)
			{
                rb.AddForce(Vector3.left * 0.02f, ForceMode.VelocityChange);
			}
            else
			{
                right = true;
			}
		}
    }
}
