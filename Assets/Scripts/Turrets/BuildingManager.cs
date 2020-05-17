using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [Tooltip("Building Menu Panel")]
    [SerializeField] BuildingPanelBuilder buildingUi;

    public bool GetUiStatus() //return status of the building ui
    {
        return buildingUi.isActiveAndEnabled;
    }

    public void EnterBuildingMenu(BuildingBase buildingBase)
    {
        buildingUi.gameObject.SetActive(true);
        buildingUi.SetupTurretPanels(buildingBase);
    }

    public void QuitBuildingMenu() //close Building menu
    {
        buildingUi.ClearList();
        buildingUi.gameObject.SetActive(false);
    }
}