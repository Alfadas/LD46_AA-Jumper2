using System.Collections;
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
    BuildingBase buildingBase;
    MetalManager metalManager;
    ExitController exitController;

    private void Awake()
    {
        StartCoroutine(FillPanel());
    }

    private void OnDisable()
    {
        turret = null;
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
        headText.text = turret.Name;
    }

    void SetInfo()
    {
        infoText.text = "Fire rate: " + turret.FireRate + "\n" +
                        "Spread on 100m: " + turret.GetMeterAutoSpread() + "m \n" +
                        "Size: " + turret.Size + "\n" +
                        "Damage per s: " + turret.Damage + "\n" +
                        "Range: " + turret.Range + "\n" +
                        "Cost: " + turret.Cost;
    }

    void SetButton()
    {
        if (metalManager.Metal - turret.Cost >= 0)
        {
            button.enabled = true;
            buttonText.text = "Place for " + turret.Cost + " Metal";
            buttonText.color = green;
        }
        else
        {
            button.enabled = false;
            buttonText.text = "Can´t afford it";
            buttonText.color = Color.red;
        }
    }

    public void SetBuildingProperties(BuildingBase buildingBase, ExitController exitController)
    {
        this.buildingBase = buildingBase;
        this.exitController = exitController;
    }

    public void SetTurret(Turret turret, bool buyable = true) // used as a start methode
    {
        if (!buyable)
        {
            button.gameObject.SetActive(false);
        }
        this.turret = turret;
    }

    public void OnButtonPressed()
    {
        if(metalManager.Metal - turret.Cost >= 0)
        {
            metalManager.DeductMetal(turret.Cost);
            buildingBase.PlaceTurret(turret);
            exitController.CloseMenuIfOpen();
        }
        else
        {
            exitController.CloseMenuIfOpen();
        }
    }
}