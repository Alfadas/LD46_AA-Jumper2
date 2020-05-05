using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverHandler : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Text scoreText;
    [Tooltip("Canvas")]
    [SerializeField] GameObject loseScreen;
    [Tooltip("Canvas")]
    [SerializeField] GameObject mainCanvas;
    [Header("Manager")]
    [SerializeField] SpawnManager spawnManager;
    [SerializeField] EnemyList enemyList;
    [SerializeField] MetalManager metalManager;
    // Start is called before the first frame update
    public void EndGame()
    {
        spawnManager.StopAllCoroutines(); //stop spawning enemies
        enemyList.KillAllAndClear(); //kill all ships and clear the list to prevent a higher score;
        metalManager.StopAllCoroutines(); //stop automatic income

        mainCanvas.SetActive(false); // deactivate main Canvas
        loseScreen.SetActive(true); // activate GameOver Endscreen
        scoreText.text = "Score: " + metalManager.GetScore(); // display Score
    }
}
