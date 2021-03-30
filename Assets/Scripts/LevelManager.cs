using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    //[SerializeField] GameObject portal;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void HowManyCrystalsLeft()
    {
        Crystal[] crystals = FindObjectsOfType<Crystal>();

        if (crystals.Length == 1)
        {
            CreatePortal();
        }
        //выводить сколько осталось -1
    }

    public void LoadNextScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    void CreatePortal()
    {

    }
}