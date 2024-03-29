﻿using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [Tooltip("Panel to sigalize interactivity")]
    [SerializeField] GameObject interactHelp = null;
    [Tooltip("UI Manager for turret interactions")]
    [SerializeField] TurretInteractionManager turretInteractionManager = null;
    [SerializeField] PlayerController playerController = null;
    [Tooltip("Controller of the player Weapon")]
    [SerializeField] Weapon weaponController = null;
    BuildingManager buildingManager = null; //attatched BuildingManager
    private void Awake()
    {
        buildingManager = gameObject.GetComponent<BuildingManager>();
    }
    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 5)) //search for BuildingBases
        {
            BuildingBase buildingBase = hit.collider.gameObject.GetComponent<BuildingBase>();
            Turret turret = hit.collider.gameObject.GetComponent<Turret>();
            if (buildingBase != null && !buildingBase.IsFilled && !buildingManager.GetUiStatus()) //if free buildingBase is in front of the player
            {
                interactHelp.SetActive(true);//show interact help only if there is a BuildingBase and no Menu shown
                if (Input.GetButton("Interact"))
                {
                    buildingManager.EnterBuildingMenu(buildingBase);
                    PlayerInteractWithUI(true);
                }
            }
            else if(turret != null && !turretInteractionManager.GetUiStatus())
            {
                interactHelp.SetActive(true);//show interact help only if there is a turret and no turretinteraction
                if (Input.GetButton("Interact"))
                {
                    turretInteractionManager.OpenTurretInteraction(turret, this);
                    PlayerInteractWithUI(true);
                }
            }
            else
            {
                interactHelp.SetActive(false);
            }
        }
        else
        {
            interactHelp.SetActive(false);
        }
    }

    public void PlayerInteractWithUI(bool canInteract)
    {
        playerController.setMouseVisible(canInteract);
        weaponController.Safety = canInteract;
    }
}