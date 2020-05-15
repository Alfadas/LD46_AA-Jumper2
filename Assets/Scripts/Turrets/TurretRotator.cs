using UnityEngine;

public class TurretRotator : MonoBehaviour
{
    [Header("Rotation parts")]
    [Tooltip("Part to rotate on y-axis (parent of gun)")]
    [SerializeField] Transform turretBase;
    [Tooltip("Part to rotate on local x-axis")]
    [SerializeField] Transform turretGuns;

    Quaternion qRotation;
    Turret turret;

    private void Start()
    {
        turret = gameObject.GetComponent<Turret>();
        if(turret == null)
        {
            Debug.LogWarning("missing turret component");
        }
    }

    public void RotateTurret(Quaternion lookRotation)
    {
        qRotation = Quaternion.Lerp(qRotation, lookRotation, Time.deltaTime * turret.TurnSpeed);
        Vector3 eulerRotation = qRotation.eulerAngles;
        turretBase.rotation = Quaternion.Euler(0f, eulerRotation.y, 0f);
        turretGuns.localRotation = Quaternion.Euler(eulerRotation.x, 0f, 0f);
    }

    public float GetRotationDiff(Quaternion lookRotation)
    {
        Vector3 eulerLookRotation = lookRotation.eulerAngles;
        return Quaternion.Angle(turretGuns.localRotation, Quaternion.Euler(eulerLookRotation.x, 0f, 0f)) +
                             Quaternion.Angle(turretBase.rotation, Quaternion.Euler(0f, eulerLookRotation.y, 0f));
    }
}