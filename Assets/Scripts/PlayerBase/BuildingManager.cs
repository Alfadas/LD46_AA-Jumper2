using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [Tooltip("Building Menu Panel")]
    [SerializeField] BuildingPanelBuilder buildingUi;
    BuildingBase buildingBase; //buildingBase used to interact with

    public bool GetUiStatus() //return status of the building ui
    {
        return buildingUi.isActiveAndEnabled;
    }

    public void EnterBuildingMenu(BuildingBase buildingBase)
    {
        this.buildingBase = buildingBase;
        buildingUi.gameObject.SetActive(true);
        buildingUi.SetupTurretPanels(this);
    }

    public void QuitBuildingMenu(Turret turret = null) //close Building menu and resume to normal game, if there is a turret to build, tell the buildingBase to build the turret
    {
        if (turret != null)
        {
            buildingBase.PlaceTurret(turret);
        }
        buildingUi.ClearList();
        buildingUi.gameObject.SetActive(false);
    }
}