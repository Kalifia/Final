using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] GameObject seal;
    [SerializeField] GameObject gateMark;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        HowManyCrystalsLeft();
    }

    public void HowManyCrystalsLeft()
    {
        Crystal[] crystals = FindObjectsOfType<Crystal>();

        if (crystals.Length == 1)
        {
            SealDeactivate();
        }
        //выводить сколько осталось -1
    }

    public void LoadNextScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    void SealDeactivate()
    {
        seal.gameObject.SetActive(false);
        gateMark.gameObject.SetActive(true);
    }
}