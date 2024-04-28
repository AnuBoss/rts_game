using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static string currentScene;

    public static Nation mySide;
    public static Nation EnemySide;

    public static void SelectSide(int side)
    {
        switch (side)
        {
            case 0:
                mySide = Nation.Britain;
                EnemySide = Nation.France;
                break;
            case 1:
                mySide = Nation.France;
                EnemySide = Nation.Britain;
                break;
        }
    }

}
