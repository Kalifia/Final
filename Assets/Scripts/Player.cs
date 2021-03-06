using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public int health = 100;
    public bool isDead = false;
    public Text healthDeleteLater;

    public Action OnHealthChange = delegate { };
    public Action OnDeath = delegate { };

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        print("this");
    }

    public void UpdateHealth(int amount)
    {
        health += amount;
        OnHealthChange();
        healthDeleteLater.text = health.ToString();

        if (health <= 0)
        {
            isDead = true;
            OnDeath();
            Destroy(gameObject);
        }
    }
}
