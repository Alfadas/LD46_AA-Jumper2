using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPanelBuilder : MonoBehaviour
{
    [SerializeField] Turret[] turrets;
    [SerializeField] TurretPanel turretPanel;
    [SerializeField] int turretPanelWidth = 200;
    List<TurretPanel> turretPanels;
    BuildingManager buildingManager;

    public void SetupTurretPanels(BuildingManager buildingManager)
    {
        this.buildingManager = buildingManager;
        turretPanels = new List<TurretPanel>();
        int counter = -2;
        foreach (Turret turret in turrets)
        {
            TurretPanel newPanel = Instantiate(turretPanel, new Vector3(transform.position.x + turretPanelWidth * counter, transform.position.y, transform.position.z), Quaternion.identity, transform);
            turretPanels.Add(newPanel);

            newPanel.SetBuildingManager(buildingManager);
            newPanel.SetTurret(turret);
            counter++;
        }
    }

    public void CloseBuildingMenu()
    {
        buildingManager.QuitBuildingMenu();
    }

    private void OnDisable()
    {
        for(int i = turretPanels.Count -1; i >= 0; i--)
        {
            Object.Destroy(turretPanels[i]);
        }
        turretPanels.Clear();
    }
}
