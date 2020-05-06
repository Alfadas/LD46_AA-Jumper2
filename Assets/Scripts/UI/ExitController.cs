using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] WeaponController weaponController;
    [SerializeField] GameObject exitPanel;
    bool blocked = false;

    void Update()
    {
        if (!blocked && Input.GetButtonDown("Cancel"))
        {
            player.setMouseVisible(!exitPanel.activeSelf);
            weaponController.toggleSafety(!exitPanel.activeSelf);
            exitPanel.SetActive(!exitPanel.activeSelf);
        }
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void BlockExitControll()
    {
        blocked = true;
    }
}