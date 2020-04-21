using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] GameObject text;
    [SerializeField] BuildingPanelBuilder buildingUi;
    [SerializeField] PlayerController playerController;
    [SerializeField] WeaponController weaponController;
    BuildingBase buildingBase;

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 5))
        {
            buildingBase = hit.collider.gameObject.GetComponent<BuildingBase>();
            if (buildingBase != null && !buildingBase.IsFilled)
            {
                if (!buildingUi.isActiveAndEnabled)
                {
                    text.SetActive(true);
                    if (Input.GetButton("Interact"))
                    {
                        buildingUi.gameObject.SetActive(true);
                        buildingUi.SetupTurretPanels(this);
                        playerController.setMouseVisible(true);
                        weaponController.BlockShooting(true);
                    }
                }
            }
            else
            {
                text.SetActive(false);
            }
        }
        else
        {
            text.SetActive(false);
        }
        if (Input.GetButtonDown("Cancel"))
        {
            if (buildingUi.isActiveAndEnabled)
            {
                QuitBuildingMenu();
            }
        }
    }

    public void QuitBuildingMenu(Turret turret = null)
    {
        if (turret != null)
        {
            buildingBase.PlaceTurret(turret);
        }
        buildingUi.ClearList();
        buildingUi.gameObject.SetActive(false);
        playerController.setMouseVisible(false);
        weaponController.BlockShooting(false);
    }
}
