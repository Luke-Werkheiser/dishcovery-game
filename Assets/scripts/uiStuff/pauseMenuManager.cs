using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseMenuManager : MonoBehaviour
{

    public static pauseMenuManager instance { get; private set; }
    public GameObject pauseMenuHolder;

    public GameObject quitMenuHolder;
    public KeyCode pauseKey = KeyCode.Escape;
    public bool gameIsPaused;
    private bool isQuitting;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenuHolder.SetActive(false);
        quitMenuHolder.SetActive(false);
    }
    void Awake()
    {
        instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            togglePause();
        }
    }
    public void togglePause()
    {
        if (isQuitting)
        {
            isQuitting = false;
            quitMenuHolder.SetActive(false);
        }
        else
        {
            gameIsPaused = !gameIsPaused;
            pauseMenuHolder.SetActive(gameIsPaused);
        }
    }
    public void resumeGame()
    {
        gameIsPaused = false;
        pauseMenuHolder.SetActive(gameIsPaused);
    }

    public void quitGame()
    {
        Application.Quit();
    }
    public void reloadScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    public void startQuit()
    {
        isQuitting = true;
        quitMenuHolder.SetActive(true);

    }
    public void stopQuitting()
    {
        isQuitting = false;
        quitMenuHolder.SetActive(false);

    }
}
