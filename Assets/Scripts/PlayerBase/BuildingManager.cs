using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [Tooltip("Panel to sigalize interactivity")]
    [SerializeField] GameObject interactHelp;
    [Tooltip("Building Menu Panel")]
    [SerializeField] BuildingPanelBuilder buildingUi;
    [SerializeField] PlayerController playerController;
    [Tooltip("Controller of the player Weapon")]
    [SerializeField] WeaponController weaponController;
    BuildingBase buildingBase; //buildingBase used to interact with

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 5)) //search for BuildingBases
        {
            buildingBase = hit.collider.gameObject.GetComponent<BuildingBase>();
            if (buildingBase != null && !buildingBase.IsFilled) //if free buildingBase is in front of the player
            {
                if (!buildingUi.isActiveAndEnabled)
                {
                    interactHelp.SetActive(true);//show interact help only if there is a BuildingBase and no Menu shown
                    if (Input.GetButton("Interact"))
                    {
                        buildingUi.gameObject.SetActive(true);
                        buildingUi.SetupTurretPanels(this);
                        //stop userinput to normal controlls to properly use the menu
                        playerController.setMouseVisible(true);
                        weaponController.toggleSafety(true);
                    }
                }
                // Maybe use a Buffer Variable bool deactivateInteractHelp which is on by default and gets turned off in the If-Branch to avoid 3 identical Else-Branches or alternatively combine everything in a single If, which could get messy
                else
                {
                    interactHelp.SetActive(false); // Does this disable interactHelp when there is an empty buildingBase and the interactHelp is enabled? Why does this work? Aren't you disabling the interactHelp at least every second Frame this Way?
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
        if (Input.GetButtonDown("Cancel"))
        {
            if (buildingUi.isActiveAndEnabled)
            {
                QuitBuildingMenu();
            }
        }
    }

    // Why does QuitBuildingMenu() get a Function and EnterBuildingMenu not?
    public void QuitBuildingMenu(Turret turret = null) //close Building menu and resume to normal game, if there is a turret to build, tell the buildingBase to build the turret
    {
        if (turret != null)
        {
            buildingBase.PlaceTurret(turret);
        }
        buildingUi.ClearList();
        buildingUi.gameObject.SetActive(false);
        playerController.setMouseVisible(false);
        weaponController.toggleSafety(false);
    }
}