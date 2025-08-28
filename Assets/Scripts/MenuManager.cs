using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public void LoadLevel(int scene)
    {
        SceneManager.LoadSceneAsync(scene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
