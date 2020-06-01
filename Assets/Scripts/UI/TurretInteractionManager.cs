using UnityEngine;

public class TurretInteractionManager : MonoBehaviour
{
    [Tooltip("Selection Menu")]
    [SerializeField] GameObject buttonPanel = null;
    [Tooltip("TurretPanel for information")]
    [SerializeField] TurretPanel turretInfo = null;
    [Tooltip("MetalManager on GameManager")]
    [SerializeField] MetalManager metalManager = null;
    [Tooltip("Percantage of metal cost that is returned on sell")]
    [SerializeField] float returnPerc = 0.5f;
    Turret currentTurret = null;
    InteractionManager currentInteractionManager = null;

    public bool GetUiStatus()
    {
        return buttonPanel.activeSelf || turretInfo.isActiveAndEnabled;
    }

    public void OpenTurretInteraction(Turret turret, InteractionManager interactionManager)
    {
        currentTurret = turret;
        currentInteractionManager = interactionManager;
        buttonPanel.SetActive(true);
    }

    public void QuitTurretInteraction()
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

    public void SellTurret()
    {
        metalManager.AddMetal(Mathf.RoundToInt(currentTurret.Cost * returnPerc));
        currentTurret.TryDestroyHittable();
        QuitTurretInteraction();
        currentInteractionManager.PlayerInteractWithUI(false);
    }
}