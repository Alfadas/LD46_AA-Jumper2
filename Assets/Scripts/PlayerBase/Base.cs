using UnityEngine;
using UnityEngine.UI;

public class Base : Hittable
{
    [Tooltip("Text Ui object to display base health")]
    [SerializeField] Text baseHealthText = null;
    [Tooltip("GameOverHandler ref to handle lose")]
    [SerializeField] GameOverHandler gameOverHandler = null;

    protected override void DestroyHittable()
    {
        //lose game
        gameOverHandler.EndGame();
    }

    public override void GetDamage(int damage)
    {
        health -= (Mathf.FloorToInt(damage * damageMulti)); // airship gets damage * damage multi
        if (health <= 0)
        {
            TryDestroyHittable();
        }
    }

    private void Update()
    {
        baseHealthText.text = "Base: " + health;
    }
}