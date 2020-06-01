using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TurretPanel : MonoBehaviour
{
    [SerializeField] Text headText = null;
    [SerializeField] Text infoText = null;
    [SerializeField] RawImage image = null;
    [SerializeField] Button button = null;
    [SerializeField] Text buttonText = null;
    [SerializeField] Color green = Color.green;
    Turret turret = null;
    BuildingBase buildingBase = null;
    MetalManager metalManager = null;
    ExitController exitController = null;

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
        infoText.text = "Rounds per Minute: " + turret.FireRate + "\n" +
                        "Magazine capacity: " + turret.MagazineCapacity + "\n" +
                        "Damage per bullet: " + turret.Damage + "\n" +
                        "Accuracy on 100m: " + turret.GetAccuracyOnDistance(100, false) + "% \n" +
                        "Accuracy on 500m: " + turret.GetAccuracyOnDistance(500, false) + "% \n" +
                        "\n" +
                        "When firing automatically:" + "\n" +
                        "Range: " + turret.Range + "m \n" +
                        "Accuracy on 100m: " + turret.GetAccuracyOnDistance(100, true)  + "% \n" +
                        "Accuracy on max Range: " + turret.GetAccuracyOnDistance(turret.Range, true) + "% \n" +
                        "\n" +
                        "Size: " + turret.Size + "\n" +
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
        image.texture = turret.TurretImage;
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