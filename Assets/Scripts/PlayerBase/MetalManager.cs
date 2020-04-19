using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetalManager : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] Text metalText;
    [SerializeField] int startingMetal = 20;
    [SerializeField] int gainIntervall = 5;
    [SerializeField] int metalGain = 2;
    int score;
    int metal;

    public int GetMetal()
    {
        return metal;
    }

    public int GetScore()
    {
        return score;
    }

    public void AddMetalAndScore(int value)
    {
        metal += value;
        score += value;
    }

    public void DeductMetal (int value)
    {
        metal -= value;
    }

    void Start()
    {
        StartCoroutine(GainIntervall());
        metal = startingMetal;
    }

    private void Update()
    {
        scoreText.text = "Score: " + score;
        metalText.text = "Metal: " + metal;
    }

    IEnumerator GainIntervall()
    {
        while (true)
        {
            yield return new WaitForSeconds(gainIntervall);
            metal += metalGain;
        }
    }
}
