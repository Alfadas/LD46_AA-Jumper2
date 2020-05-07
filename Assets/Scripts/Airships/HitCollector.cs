using UnityEngine;

public class HitCollector : MonoBehaviour
{
    [Tooltip("Damage multiplicator if this part is hit")]
    [SerializeField] float damageMulti;
    Airship airship; //airship this is attached to

    void Start()
    {
        airship = GetComponentInParent<Airship>(); //get attached airship
    }

    public void GetDamage(int damage) //Methode if this Part is damaged
    {
        airship.GetDamage(Mathf.RoundToInt(damage * damageMulti)); // airship gets damage * damage multi
    }
}