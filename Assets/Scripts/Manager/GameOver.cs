using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void RePlay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GamePlay");
    }
}
