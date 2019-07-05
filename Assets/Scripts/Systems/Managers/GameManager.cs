using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Static Variables
    public static GameManager Instance;
    public static AudioDictonary AudioManager;
    public static Player player;

    private string m_mainMenuScene = "MainMenu";
    private string m_playScene = "PlayScene";

    private static bool m_gameOver = false;
    public bool GameOver
    {
        get { return m_gameOver; }
        set { m_gameOver = value; }
    }
    private static bool m_paused = false;
    public bool IsPaused
    {
        get { return m_paused; }
        set { m_paused = value; }
    }
    #endregion

    #region Pause Functions
    public Canvas pauseCanvas;
    public void OnApplicationFocus(bool focus)
    {
        if (this != Instance)
            return;

        if (SceneManager.GetActiveScene().buildIndex > 5)
        {
            if (!focus && !m_paused && !Application.isEditor)
                TogglePause(true);
        }
    }

    public void TogglePause()
    {
        TogglePause(!m_paused);
    }

    public void TogglePause(bool paused)
    {
        m_paused = paused;
        Time.timeScale = paused ? 0 : 1;
        pauseCanvas.gameObject.SetActive(paused);
        ToggleCursor(paused);
        ToggleStateMachinesPause();
    }

    public void ToggleCursor(bool visible)
    {
        if (visible)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ToggleStateMachinesPause()
    {
        StateManager[] managers = FindObjectsOfType<StateManager>();
        for (int i = 0; i < managers.Length; i++)
        {
            managers[i].IsPaused = m_paused;
        }
    }
    #endregion

    #region Player Managment
    public GameObject playerPrefab;
    public static SpawnManager levelSpawn;


    private void SpawnPlayer()
    {
        Transform spawnTransfrom = levelSpawn.GetNextSpawn();
        Instantiate(playerPrefab, spawnTransfrom.position, spawnTransfrom.rotation).GetComponent<Player>();
        Debug.Log("Spawned Player");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion

    #region Scene Managment
    public void MainMenu()
    {
        SceneManager.LoadScene(m_mainMenuScene);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void AddScene(string name)
    {
        Debug.Log("GameManager:AddScene (" + name + ")", this);
        SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
    }
    public void AddScene(int index)
    {
        SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
    }

    public void UnloadScene(string name)
    {
        if(SceneManager.GetSceneByName(name).isLoaded)
            SceneManager.UnloadSceneAsync(name);
    }
    public void UnloadScene(int index)
    {
        if (SceneManager.GetSceneAt(index).isLoaded)
            SceneManager.UnloadSceneAsync(index);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == m_mainMenuScene)
        {
            m_paused = false;
            ToggleCursor(true);
            Time.timeScale = 1;
        }

        else if (scene.name == m_playScene)
        {
            if (!player) SpawnPlayer();
            ToggleCursor(false);
        }
    }
    private void OnSceneUnloaded(Scene scene) { }
    #endregion

    #region Main
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            StopAllCoroutines();
            DestroyImmediate(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause") && SceneManager.GetActiveScene().name == m_playScene)
        {
            TogglePause();
        }
    }
    #endregion
}
