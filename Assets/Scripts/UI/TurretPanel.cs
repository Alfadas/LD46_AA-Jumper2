using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretPanel : MonoBehaviour
{
    [SerializeField] Text headText;
    [SerializeField] Text infoText;
    [SerializeField] Image image;
    [SerializeField] Button button;
    [SerializeField] Text buttonText;
    [SerializeField] Color green;
    Turret turret;
    BuildingManager buildingManager;
    MetalManager metalManager;
    // Start is called before the first frame update

    private void Start()
    {
        StartCoroutine(FillPanel());
    }

    IEnumerator FillPanel()
    {
        metalManager = FindObjectOfType<MetalManager>();
        if (turret == null)
        {
            yield return new WaitWhile(() => turret == null);
        }
        SetName();
        SetInfo();
        SetButton();
    }

    void SetName()
    {
        headText.text = turret.GetName();
    }

    void SetInfo()
    {
        infoText.text = "Fire rate: " + turret.GetFireRate() + "\n" +
                        "Spread on 100m: " + turret.GetMeterAutoSpread() + "m \n" +
                        "Size: " + turret.GetSize() + "\n" +
                        "Damage per s: " + turret.GetDamage() + "\n" +
                        "Range: " + turret.GetRange() + "\n" +
                        "Cost: " + turret.GetCost();
    }

    void SetButton()
    {
        if (metalManager.Metal - turret.GetCost() >= 0)
        {
            button.enabled = true;
            buttonText.text = "Place for " + turret.GetCost() + " Metal";
            buttonText.color = green;
        }
        else
        {
            button.enabled = false;
            buttonText.text = "Can´t afford it";
            buttonText.color = Color.red;
        }
    }

    public void SetBuildingManager(BuildingManager buildingManager)
    {
        this.buildingManager = buildingManager;
    }

    public void SetTurret(Turret turret)
    {
        this.turret = turret;
    }

    public void OnButtonPressed()
    {
        if(metalManager.Metal - turret.GetCost() >= 0)
        {
            metalManager.DeductMetal(turret.GetCost());
            buildingManager.QuitBuildingMenu(turret);
        }
        else
        {
            buildingManager.QuitBuildingMenu();
        } 
    }
}
