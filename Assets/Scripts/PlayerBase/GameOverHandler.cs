using UnityEngine;
using UnityEngine.UI;

public class GameOverHandler : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Text scoreText = null;
    [Tooltip("Canvas")]
    [SerializeField] GameObject loseScreen = null;
    [Tooltip("Canvas")]
    [SerializeField] GameObject mainCanvas = null;
    [Header("Manager")]
    [SerializeField] SpawnManager spawnManager = null;
    [SerializeField] EnemyList enemyList = null;
    [SerializeField] MetalManager metalManager = null;
    [Header("Player")]
    [SerializeField] PlayerController playerController = null;
    [SerializeField] Weapon weaponController = null;
    [SerializeField] ExitController exitController = null;

    public void EndGame()
    {
        spawnManager.StopAllCoroutines(); //stop spawning enemies
        enemyList.KillAllAndClear(); //kill all ships and clear the list to prevent a higher score;
        metalManager.StopAllCoroutines(); //stop automatic income

        exitController.BlockExitControll(true); //prevent reseting mouse and weapon controll
        playerController.setMouseVisible(true); //stop movement and set mouse visibel
        weaponController.Safety = true; //stop shooting

        mainCanvas.SetActive(false); // deactivate main Canvas
        loseScreen.SetActive(true); // activate GameOver Endscreen
        scoreText.text = "Score: " + metalManager.Score; // display Score
    }
}