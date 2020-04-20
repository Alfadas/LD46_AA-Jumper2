using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] WeaponController weaponController;
    [SerializeField] GameObject infoPanel;
    [SerializeField] Text infoText;
    [SerializeField] string introText;
    [SerializeField] int zTriggerWeapon;
    [SerializeField] string weaponInfo;
    [SerializeField] int zTriggerEnemies;
    [SerializeField] string enemyInfo;
    [SerializeField] Airship airship;
    [SerializeField] GameObject blocker;
    [SerializeField] int zTriggerTurrets;
    [SerializeField] string turretInfo;
    [SerializeField] BuildingBase buildingBase;
    [SerializeField] GameObject toMenuButton;
    [SerializeField] string endInfo;
    [SerializeField] GameObject goBackQuestion;

    // Start is called before the first frame update
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

    IEnumerator Tutorial()
    {
        if (player.transform.position.z < zTriggerWeapon)
        {
            yield return new WaitUntil(() => player.transform.position.z >= zTriggerWeapon);
        }
        infoPanel.SetActive(true);
        infoText.text = weaponInfo;
        if (player.transform.position.z < zTriggerEnemies)
        {
            yield return new WaitUntil(() => player.transform.position.z >= zTriggerEnemies);
        }
        infoPanel.SetActive(true);
        infoText.text = enemyInfo;
        if (airship != null)
        {
            yield return new WaitUntil(() => airship == null);
        }
        Object.DestroyImmediate(blocker);
        if (player.transform.position.z < zTriggerTurrets)
        {
            yield return new WaitUntil(() => player.transform.position.z >= zTriggerTurrets);
        }
        infoPanel.SetActive(true);
        infoText.text = turretInfo;
        if (!buildingBase.IsFilled)
        {
            yield return new WaitUntil(() => buildingBase.IsFilled == true);
        }
        player.setMouseVisible(true);
        weaponController.BlockShooting(true);
        infoPanel.SetActive(true);
        toMenuButton.SetActive(true);
        infoText.text = endInfo;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if(infoPanel.activeSelf == true)
            {
                infoPanel.SetActive(false);
                player.setMouseVisible(false);
                weaponController.BlockShooting(false);
            }
            else
            {
                player.setMouseVisible(!goBackQuestion.activeSelf);
                weaponController.BlockShooting(!goBackQuestion.activeSelf);
                goBackQuestion.SetActive(!goBackQuestion.activeSelf);
            }
        }
    }

    public void BackToMenu()
    {

    }
}
