using System.Collections.Generic;
using UnityEngine;

public class BuildingPanelBuilder : MonoBehaviour
{
    [SerializeField] Turret[] turrets;
    [SerializeField] RectTransform turretPanel;
    [SerializeField] int turretPanelWidth = 200;
    List<TurretPanel> turretPanels = new List<TurretPanel>();

    public void SetupTurretPanels(BuildingBase buildingBase)
    {
        ExitController exitController = FindObjectOfType<ExitController>();
        ClearList();
        int counter = -2;
        foreach (Turret turret in turrets)
        {
            RectTransform newDisplay = Instantiate(turretPanel, new Vector3(transform.position.x + turretPanelWidth * counter, transform.position.y, transform.position.z), Quaternion.identity, transform);
            TurretPanel newPanel = newDisplay.gameObject.GetComponentInChildren<TurretPanel>();

            newDisplay.transform.localPosition = new Vector3(turretPanelWidth * counter, 0, 0);
            turretPanels.Add(newPanel);

            newPanel.SetBuildingProperties(buildingBase, exitController);
            newPanel.SetTurret(turret);
            counter++;
        }
    }

    public void ClearList()
    {
        for (int i = turretPanels.Count - 1; i >= 0; i--)
        {
            Object.Destroy(turretPanels[i].gameObject);
        }
        turretPanels.Clear();
    }

    private void OnDisable()
    {
        ClearList();
    }
}