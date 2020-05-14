using UnityEngine;

public class TurretInteractionManager : MonoBehaviour
{
    [Tooltip("Selection Menu")]
    [SerializeField] GameObject buttonPanel;
    [Tooltip("TurretPanel for information")]
    [SerializeField] TurretPanel turretInfo;
    Turret currentTurret;

    public bool GetUiStatus()
    {
        return buttonPanel.activeSelf || turretInfo.isActiveAndEnabled;
    }

    public void OpenTurretInteraction(Turret turret)
    {
        currentTurret = turret;
        buttonPanel.SetActive(true);
    }

    public void CloseTurretInteraction()
    {
        buttonPanel.SetActive(false);
        turretInfo.gameObject.SetActive(false);
    }

    public void OpenTurretInfo()
    {
        buttonPanel.SetActive(false);
        turretInfo.gameObject.SetActive(true);
        turretInfo.SetTurret(currentTurret, false);
    }

}
