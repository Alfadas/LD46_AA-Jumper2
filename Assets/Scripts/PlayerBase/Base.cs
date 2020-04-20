using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Base : Building
{
    [SerializeField] Text text;
    [SerializeField] Text scoreText;
    [SerializeField] GameObject loseScreen;
    [SerializeField] SpawnManager spawnManager;
    [SerializeField] GameObject canvas;
    [SerializeField] MetalManager metalManager;
    protected override void DestroyBuilding()
    {
        base.DestroyBuilding();
        spawnManager.StopAllCoroutines();
        loseScreen.SetActive(true);
        scoreText.text = "Score: " + metalManager.GetScore();
    }

    private void Update()
    {
        text.text = "Base: " + health;
    }
}
