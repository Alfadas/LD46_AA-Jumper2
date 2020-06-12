using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezLocalPosition : MonoBehaviour
{
    Rigidbody objectRigidbody = null;
    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        objectRigidbody.velocity = Vector3.zero;
        objectRigidbody.angularVelocity = Vector3.zero;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
