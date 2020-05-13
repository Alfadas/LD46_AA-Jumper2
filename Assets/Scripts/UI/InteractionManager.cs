using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [Tooltip("Panel to sigalize interactivity")]
    [SerializeField] GameObject interactHelp;
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
        }
    }
}
