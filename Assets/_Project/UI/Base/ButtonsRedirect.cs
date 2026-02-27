using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsRedirect : MonoBehaviour
{
    // scene name
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // build index
    public void GoToScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}
