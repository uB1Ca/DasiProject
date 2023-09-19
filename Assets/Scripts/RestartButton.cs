using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RestartButton : MonoBehaviour
{
    public string sceneToReload = "MainScene";

    public void RestartGame()
    {
        SceneManager.LoadScene(sceneToReload);
    }
}
