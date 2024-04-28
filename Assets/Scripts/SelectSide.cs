using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectSide : MonoBehaviour
{
    [SerializeField] private string mainMenuScene;
    [SerializeField] private string chooseMapScene;
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    private void GoToChooseMap()
    {
        SceneManager.LoadScene(chooseMapScene);

    }

    public void chooseSide(int i)
    {
       Settings.SelectSide(i);
        GoToChooseMap();
    }
}
