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
    private string m_pauseMenuScene = "PauseMenu";
    private string m_playScene = "PlayScene";
    private string m_hudScene = "HudScene";

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
    public void OnApplicationFocus(bool focus)
    {
        if (this != Instance)
            return;

        if (SceneManager.GetActiveScene().buildIndex > 5)
        {
            if (!focus && !m_paused && !Application.isEditor)
                AddScene(m_pauseMenuScene);
        }
    }

    public void TogglePause()
    {
        if (this != Instance)
            return;

        m_paused = !m_paused;
        Time.timeScale = m_paused ? 0 : 1;
        ToggleCursor(m_paused);
        ToggleStateMachinesPause();

    }

    public void TogglePause(bool pause)
    {
        if (this != Instance)
            return;

        m_paused = pause;
        Time.timeScale = pause ? 0 : 1;
        ToggleCursor(pause);
        ToggleStateMachinesPause();
    }

    public void ToggleCursor(bool value)
    {
        if (value)
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

    public void Continue()
    {
        int sceneIndex = PlayerPrefs.GetInt("ContinueScene");
        LoadScene(sceneIndex);
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
        if (this != Instance)
            return;

        if (scene.name == m_pauseMenuScene)
            TogglePause();

        else if (scene.name == m_hudScene)
        {
            ToggleCursor(m_paused);
        }

        else if (scene.name == m_mainMenuScene)
        {
            m_paused = false;
            ToggleCursor(true);
            Time.timeScale = 1;
            if (player) Destroy(player.gameObject);
        }

        else if (scene.name == m_playScene)
        {
            if (!player) SpawnPlayer();

            ToggleCursor(false);

            if (!SceneManager.GetSceneByName(m_hudScene).isLoaded)
                SceneManager.LoadScene(m_hudScene, LoadSceneMode.Additive);
        }

        else
        {
            //Scene is LevelScene
            string currentLevel = scene.name;
            PlayerPrefs.SetString("ContinueScene", currentLevel);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("PlayScene"));
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == m_pauseMenuScene)
            TogglePause();
    }
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
        if (this != Instance)
            return;

        if (Input.GetButtonDown("Pause") && SceneManager.GetActiveScene().name == m_playScene)
        {
            if (m_paused) UnloadScene(m_pauseMenuScene);
            else AddScene(m_pauseMenuScene);
        }
    }
    #endregion
}
