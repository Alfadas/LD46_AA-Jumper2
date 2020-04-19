using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollector : MonoBehaviour
{
    [SerializeField] float damageMulti;
    Airship airship;
    // Start is called before the first frame update
    void Start()
    {
        airship = GetComponentInParent<Airship>();
    }

    public void GetDamage(int damage)
    {
        airship.GetDamage(Mathf.RoundToInt(damage * damageMulti));
    }
}
