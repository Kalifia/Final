using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider playerHealth;
    public GameObject gameOverPanel;

    void Start()
    {
        Player.Instance.OnHealthChange += UpdateHealth;
        //Player.Instance.OnDeath += ShowGameOver;

        playerHealth.maxValue = Player.Instance.health;
        playerHealth.value = Player.Instance.health;
    }

    //private void ShowGameOver()
    //{
    //    StartCoroutine(ShowGameOverWithDelay(1)); 
    //}

    //IEnumerator ShowGameOverWithDelay(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    gameOverPanel.SetActive(true);
    //}

    //када умрет

    private void UpdateHealth()
    {
        playerHealth.value = Player.Instance.health;
    }
}
