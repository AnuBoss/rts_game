using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // เพิ่ม namespace สำหรับ Button

public class SelectMap : MonoBehaviour
{
    [SerializeField] private string mapScene;
    [SerializeField] private Button startGameButton;

    private void Start()
    {
       
        if (startGameButton != null)
        {
            startGameButton.interactable = false;
        }
    }

    public void ChooseMap(string mapName)
    {
        mapScene = mapName;

        
        if (startGameButton != null && !string.IsNullOrEmpty(mapName))
        {
            startGameButton.interactable = true;
        }
    }

    public void StartGame()
    {
        
        if (string.IsNullOrEmpty(mapScene))
        {
            return;
        }

        Settings.currentScene = mapScene;
        SceneManager.LoadScene(mapScene);
    }
}