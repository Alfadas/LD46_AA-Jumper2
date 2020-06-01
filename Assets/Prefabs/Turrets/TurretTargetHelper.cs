using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTargetHelper : MonoBehaviour
{
    readonly Vector3 gravity = new Vector3(0, 10f, 0);
    Turret turret = null;
    private void Start()
    {
        turret = gameObject.GetComponent<Turret>();
        if (turret == null)
        {
            Debug.LogWarning("missing turret component");
        }
    }

    public Vector3 CalcTargetLeadPoint()
    {
        Vector3 targetRelativePosition = turret.Target.transform.position - turret.TurretAimCenter;
        float t = FirstOrderInterceptTime(turret.WeaponController.MuzzleVelocity, targetRelativePosition, turret.Target.Velocity);
        float timeGravity = FirstOrderInterceptTime(turret.WeaponController.MuzzleVelocity, targetRelativePosition, gravity);
        Vector3 targetLead = turret.Target.transform.position + turret.Target.Velocity * t + 0.5f * gravity * Mathf.Pow(timeGravity, 2);
        Vector3 dir = targetLead - turret.TurretAimCenter;
        return dir;
    }

    //first-order intercept using relative target position
    public static float FirstOrderInterceptTime
    (
        float shotSpeed,
        Vector3 targetRelativePosition,
        Vector3 targetRelativeVelocity
    )
    {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if (velocitySquared < 0.001f)
            return 0f;

        float a = velocitySquared - shotSpeed * shotSpeed;

        //handle similar velocities
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude /
            (
                2f * Vector3.Dot
                (
                    targetRelativeVelocity,
                    targetRelativePosition
                )
            );
            return Mathf.Max(t, 0f); //don't shoot back in time
        }

        float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0f)
        { //determinant > 0; two intercept paths (most common)
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                    t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
            if (t1 > 0f)
            {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both are positive
                else
                    return t1; //only t1 is positive
            }
            else
                return Mathf.Max(t2, 0f); //don't shoot back in time
        }
        else if (determinant < 0f) //determinant < 0; no intercept path
            return 0f;
        else //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
    }
}
