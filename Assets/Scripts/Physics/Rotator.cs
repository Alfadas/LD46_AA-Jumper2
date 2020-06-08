using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] Vector3 rotationSpeed = Vector3.zero;

    void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + rotationSpeed * Time.deltaTime);
    }
}