﻿using UnityEngine;

public class AirshipPropeller : AirshipPart
{
    [Tooltip("percent of Max speed, reduced, if the propeller is destroyed")]
    [Range(0,1)][SerializeField] float speedReductionPerc = 0.5f;
    protected override void Start()
    {
        base.Start();
    }

    protected override void DestroyHittable()
    {
        airship.ChangeMaxSpeedModifier(-speedReductionPerc);
        base.DestroyHittable();
    }
}