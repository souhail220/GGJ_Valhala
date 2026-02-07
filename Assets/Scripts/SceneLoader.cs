using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;
    public int sceneIndex = -1;

    public void LoadScene()
    {
        if (sceneIndex >= 0)
            SceneManager.LoadScene(sceneIndex);
        else if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
        else
            Debug.LogError("No scene name or index assigned!");
    }

    public void ExitGame()
    {
        Debug.Log("Exit button pressed");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
