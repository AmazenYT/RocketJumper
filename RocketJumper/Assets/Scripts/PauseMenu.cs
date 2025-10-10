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
            paused = true;
        }

        else if (Input.GetKeyDown(KeyCode.Escape) && paused == true)
        {
            closePauseMenu.Invoke();
            paused = false;
        }

    }

    public void resumeGame()
    {
        paused = false;
    }

    public void reloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        paused = false;
    }


}
