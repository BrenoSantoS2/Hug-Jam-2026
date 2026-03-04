using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsRedirect : MonoBehaviour
{
    // scene name
    public void GoToScene(string sceneName)
    {
        Debug.Log("Tentando carregar a cena: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    // build index
    public void GoToScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}
