using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAdmin : MonoBehaviour
{
    public static string typeCrash;

	public void ChangeScene(int idScene)
    {
        SceneManager.LoadSceneAsync(idScene);
    }

    public void SetCrashType(string crashType)
    {
        typeCrash = crashType;
    }

    public void ReturnToMain()
    {
        SceneManager.LoadScene(0);
    }

}
