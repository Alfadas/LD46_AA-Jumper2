using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Base : Building
{
    [SerializeField] Text text;
    [SerializeField] GameOverHandler gameOverHandler;

    protected override void DestroyBuilding()
    {
        base.DestroyBuilding();
        gameOverHandler.EndGame();
    }

    private void Update()
    {
        text.text = "Base: " + health;
    }
}