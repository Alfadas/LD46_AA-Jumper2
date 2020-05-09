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

    // Potential for Bugs if someday Building starts using Update(), too, any Keywords/Strategy to defuse this? Calling base.Update() is a bad Idea because Magic Methods should not be called directly, right?
    private void Update()
    {
        baseHealthText.text = "Base: " + health;
    }
}