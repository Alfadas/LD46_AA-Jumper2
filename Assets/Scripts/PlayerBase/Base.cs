using UnityEngine;
using UnityEngine.UI;

public class Base : Building
{
    [Tooltip("Text Ui object to display base health")]
    [SerializeField] Text baseHealthText;
    [Tooltip("GameOverHandler ref to handle lose")]
    [SerializeField] GameOverHandler gameOverHandler;

    protected override void DestroyBuilding()
    {
        //lose game
        base.DestroyBuilding();
        gameOverHandler.EndGame();
    }

    private void Update()
    {
        baseHealthText.text = "Base: " + health;
    }
}