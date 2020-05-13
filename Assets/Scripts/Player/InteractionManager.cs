using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [Tooltip("Panel to sigalize interactivity")]
    [SerializeField] GameObject interactHelp;
    [Tooltip("UI Manager for turret interactions")]
    [SerializeField] TurretInteractionManager turretInteractionManager;
    [SerializeField] PlayerController playerController;
    [Tooltip("Controller of the player Weapon")]
    [SerializeField] WeaponController weaponController;
    BuildingManager buildingManager; //attatched BuildingManager
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
            if (buildingBase != null && !buildingBase.IsFilled) //if free buildingBase is in front of the player
            {
                if (!buildingManager.GetUiStatus())
                {
                    interactHelp.SetActive(true);//show interact help only if there is a BuildingBase and no Menu shown
                    if (Input.GetButton("Interact"))
                    {
                        buildingManager.EnterBuildingMenu(buildingBase);
                    }
                }
                else
                {
                    interactHelp.SetActive(false);
                }
            }
            else if(turret != null && !turretInteractionManager.GetUiStatus())
            {
                interactHelp.SetActive(true);//show interact help only if there is a turret and no turretinteraction
                if (Input.GetButton("Interact"))
                {
                    turretInteractionManager.OpenTurretInteraction();
                    playerController.setMouseVisible(true);
                    weaponController.toggleSafety(true);
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
            if (buildingManager.GetUiStatus())
            {
                buildingManager.QuitBuildingMenu();
            }
            else if(turretInteractionManager.GetUiStatus())
            {
                turretInteractionManager.CloseTurretInteraction();
                playerController.setMouseVisible(false);
                weaponController.toggleSafety(false);
            }
        }
    }
}
