using UnityEngine;

public class AirshipCabin : AirshipPart
{
    [Header("Cabin values")]
    [Tooltip("Parent of all bomb bays")]
    [SerializeField] GameObject bombBayHolder = null;
    protected override void Start()
    {
        base.Start();
    }

    protected override void PreDestroyHittable()
    {
        BombBay[] bombBays = bombBayHolder.GetComponentsInChildren<BombBay>();
        foreach(BombBay bombBay in bombBays)
        {
            bombBay.SetDroped(true);
        }
        base.PreDestroyHittable();
    }
}
