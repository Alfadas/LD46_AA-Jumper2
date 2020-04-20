using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] WeaponController weaponController;
    [SerializeField] GameObject exitPanel;

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            player.setMouseVisible(!exitPanel.activeSelf);
            weaponController.BlockShooting(!exitPanel.activeSelf);
            exitPanel.SetActive(!exitPanel.activeSelf);
        }
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}