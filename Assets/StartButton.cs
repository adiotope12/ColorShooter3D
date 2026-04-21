using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField] private string sceneName = "__Scene_0";

    public void OpenStartScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
