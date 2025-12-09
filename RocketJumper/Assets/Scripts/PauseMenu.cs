using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public UnityEvent openPauseMenu, closePauseMenu;
    public bool paused = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && paused == false)
        {
            openPauseMenu.Invoke();
            Time.timeScale = 0f;
            paused = true;
        }

        else if (Input.GetKeyDown(KeyCode.Escape) && paused == true)
        {
            closePauseMenu.Invoke();
            Time.timeScale = 1f;
            paused = false;
        }

    }

    public void resumeGame()
    {
        closePauseMenu.Invoke();
        Time.timeScale = 1f;
        paused = false;
    }

    public void reloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
        paused = false;
    }

    public void exitGame()
    {
        Application.Quit();
    }

}
