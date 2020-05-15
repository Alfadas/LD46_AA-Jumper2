using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("General")]
    [Tooltip("PlayerController on Player")]
    [SerializeField] PlayerController player;
    [Tooltip("WeaponController on player weapon")]
    [SerializeField] WeaponController weaponController;
    [Tooltip("exitController for cancel management")]
    [SerializeField] ExitController exitController;
    [Tooltip("Tutorial info panel")]
    [SerializeField] GameObject infoPanel;
    [Tooltip("Tutorial info text object")]
    [SerializeField] Text infoText;
    [Header("Intro")]
    [Tooltip("Displayed text, '/n' for new line")]
    [SerializeField] string introText;
    [Header("Weapon Info")]
    [Tooltip("Info display position")]
    [SerializeField] int zTriggerWeapon;
    [Tooltip("Displayed text, '/n' for new line")]
    [SerializeField] string weaponInfo;
    [Header("Enemy Info")]
    [Tooltip("Info display position")]
    [SerializeField] int zTriggerEnemies;
    [Tooltip("Displayed text, '/n' for new line")]
    [SerializeField] string enemyInfo;
    [Tooltip("Airship to kill")]
    [SerializeField] Airship airship;
    [Tooltip("Blocker to delete after airship kill")]
    [SerializeField] GameObject blocker;
    [Header("Turret Info")]
    [Tooltip("Info display position")]
    [SerializeField] int zTriggerTurrets;
    [Tooltip("Displayed text, '/n' for new line")]
    [SerializeField] string turretInfo;
    [Tooltip("BuildingBase for turret ro be build on")]
    [SerializeField] BuildingBase buildingBase;
    [Header("End")]
    [Tooltip("Tutorial info to menu button")]
    [SerializeField] GameObject toMenuButton;
    [Tooltip("Displayed text, '/n' for new line")]
    [SerializeField] string endInfo;

    void Start()
    {
        infoPanel.SetActive(true);
        introText = introText.Replace("/n ", "\n");
        infoText.text = introText;
        weaponInfo = weaponInfo.Replace("/n ", "\n");
        enemyInfo = enemyInfo.Replace("/n ", "\n");
        turretInfo = turretInfo.Replace("/n ", "\n");
        endInfo = endInfo.Replace("/n ", "\n");
        StartCoroutine(Tutorial());
    }

    private void Update()
    {
        exitController.BlockExitControll(infoPanel.activeSelf);
        if (Input.GetButtonDown("Cancel"))
        {
            if (infoPanel.activeSelf == true)
            {
                infoPanel.SetActive(false);
                player.setMouseVisible(false);
                weaponController.toggleSafety(false);
            }
        }
    }

    IEnumerator Tutorial()
    {
        // Weapon Info
        if (player.transform.position.z < zTriggerWeapon)
        {
            yield return new WaitUntil(() => player.transform.position.z >= zTriggerWeapon); //Wait until player reaches zPoint
        }
        infoPanel.SetActive(true);
        infoText.text = weaponInfo;
        // Enemy Info
        if (player.transform.position.z < zTriggerEnemies)
        {
            yield return new WaitUntil(() => player.transform.position.z >= zTriggerEnemies);//Wait until player reaches zPoint
        }
        infoPanel.SetActive(true);
        infoText.text = enemyInfo;
        if (airship != null)
        {
            yield return new WaitUntil(() => airship == null); //Wait until player kills airship
        }
        Object.DestroyImmediate(blocker);
        if (player.transform.position.z < zTriggerTurrets)
        {
            yield return new WaitUntil(() => player.transform.position.z >= zTriggerTurrets);//Wait until player reaches zPoint
        }
        infoPanel.SetActive(true);
        infoText.text = turretInfo;
        if (!buildingBase.IsFilled)
        {
            yield return new WaitUntil(() => buildingBase.IsFilled == true); // Wait until player builds turret
        }
        player.setMouseVisible(true);
        weaponController.toggleSafety(true);
        infoPanel.SetActive(true);
        toMenuButton.SetActive(true);
        infoText.text = endInfo;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
