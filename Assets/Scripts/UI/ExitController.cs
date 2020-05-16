using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] WeaponController weaponController;
    [SerializeField] GameObject exitPanel;
    [SerializeField] TurretInteractionManager turretInteractionManager;
    [SerializeField] BuildingManager buildingManager;
    bool blocked = false;

    void Update()
    {
        if (!blocked && Input.GetButtonDown("Cancel"))
        {
            if(!CloseMenuIfOpen())
            {
                //toggle back to menu window
                player.setMouseVisible(!exitPanel.activeSelf);
                weaponController.toggleSafety(!exitPanel.activeSelf);
                exitPanel.SetActive(!exitPanel.activeSelf);
            }
        }
    }

    public bool CloseMenuIfOpen() // returns if a menu was open
    {
        if (buildingManager.GetUiStatus())
        {
            buildingManager.QuitBuildingMenu();
            player.setMouseVisible(false);
            weaponController.toggleSafety(false);
            return true;
        }
        else if (turretInteractionManager.GetUiStatus())
        {
            turretInteractionManager.QuitTurretInteraction();
            player.setMouseVisible(false);
            weaponController.toggleSafety(false);
            return true;
        }
        return false;
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void BlockExitControll(bool blocked)
    {
        this.blocked = blocked;
    }
}