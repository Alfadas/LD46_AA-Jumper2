using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MetalManager : MonoBehaviour
{
    [Header("UI Objects")]
    [Tooltip("UI Text Object for the score")]
    [SerializeField] Text scoreText = null;
    [Tooltip("UI Text Object for metal")]
    [SerializeField] Text metalText = null;
    [Header("Metal gain")]
    [Tooltip("Metal count at the beginning of the game")]
    [SerializeField] int startingMetal = 20;
    [Tooltip("Intervall in sec when metal is gained")]
    [SerializeField] int gainIntervall = 5;
    [Tooltip("Metal gain per intervall")]
    [SerializeField] int metalGain = 2;

    public int Metal { get; private set; } //current Metal
    public int Score { get; private set; } //current Score

    void Start()
    {
        StartCoroutine(GainIntervall());
        Metal = startingMetal;
    }

    private void Update()
    {
        scoreText.text = "Score: " + Score;
        metalText.text = "Metal: " + Metal;
    }
    public void AddMetalAndScore(int value) //used for kill rewards
    {
        Metal += value;
        Score += value;
    }

    public void AddMetal(int value)
    {
        Metal += value;
    }

    public void DeductMetal (int value)
    {
        Metal -= value;
    }

    IEnumerator GainIntervall()
    {
        while (true)
        {
            yield return new WaitForSeconds(gainIntervall);
            Metal += metalGain;
        }
    }
}