using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] GameObject text;
    [SerializeField] BuildingPanelBuilder buildingUi;
    [SerializeField] PlayerController playerController;
    BuildingBase buildingBase;

    private void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, 2))
        {
            buildingBase = hit.collider.gameObject.GetComponent<BuildingBase>();
            if ( buildingBase != null && !buildingBase.IsFilled && !buildingUi.isActiveAndEnabled)
            {
                text.SetActive(true);
                if (Input.GetButton("Interact"))
                {
                    buildingUi.gameObject.SetActive(true);
                    buildingUi.SetupTurretPanels(this);
                    playerController.setMouseVisible(true);
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
    }

    public void QuitBuildingMenu(Turret turret = null)
    {
        if (turret != null)
        {
            buildingBase.PlaceTurret(turret);
        }
        buildingUi.gameObject.SetActive(false);
        playerController.setMouseVisible(false);
    }
}
