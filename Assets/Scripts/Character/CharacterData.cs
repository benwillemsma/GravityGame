using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(StateManager))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public abstract class CharacterData : MonoBehaviour
{
    #region Components
    protected StateManager m_stateM;
    protected Animator m_anim;
    protected Rigidbody m_rb;

    public StateManager StateM
    {
        get { return m_stateM; }
        set { m_stateM = value; }
    }
    public Rigidbody RB
    {
        get { return m_rb; }
        set { m_rb = value; }
    }
    public Animator Anim
    {
        get { return m_anim; }
        set { m_anim = value; }
    }
    #endregion

    #region Pause Info
    private bool m_paused;

    public bool isPaused
    {
        get { return m_paused; }
    }
    public virtual void PauseController()
    {
        StateM.IsPaused = true;

        RB.velocity = Vector3.zero;
        m_paused = true;
    }
    public virtual void UnPauseController()
    {
        StateM.IsPaused = false;
        m_paused = false;
    }
    #endregion

    #region Health Info
    [Header("Health Info")]
    [SerializeField]
    protected Transform respawnPoint;
    protected Vector3 m_startLocation;
    
    [SerializeField]
    protected float m_maxHealth = 100;
    [SerializeField]
    protected float m_damageImmuneDuration = 0.5f;
    [SerializeField]
    protected float m_deathDelay = 3f;

    protected bool m_isDead = false;
    protected float m_currentHealth;
    protected float m_damageImmune;

    public float Health
    {
        get { return m_currentHealth; }
    }

    public virtual void TakeDamage(float damage)
    {
        if (m_damageImmune <= 0)
        {
            m_damageImmune = m_damageImmuneDuration;
            m_currentHealth -= damage;

            if (Health <= 0)
                GameManager.Instance.StartCoroutine(Die());
        }
    }

    protected virtual void SetUndamageable(float duration = Mathf.Infinity)
    {
        m_damageImmuneDuration = duration;
    }

    protected virtual void SetDamageable()
    {
        m_damageImmuneDuration = -1;
    }

    protected virtual void UpdateHealthStatus()
    {
        if (m_damageImmune > 0)
            m_damageImmune -= Time.deltaTime;
    }

    protected virtual IEnumerator Die()
    {
        m_isDead = true;
        yield return new WaitForSeconds(m_deathDelay);
        Destroy(transform.gameObject);
    }

    public void RespawnCharacter()
    {
        if (respawnPoint)
        {
            transform.position = respawnPoint.position;
            RB.velocity = Vector3.zero;
        }
        else Destroy(gameObject);
    }
    #endregion

    #region Audio
    protected AudioSource m_audioSource;

    public void PlaySound(string soundName)
    {
        GameManager.AudioManager.playAudio(this, soundName);
    }

    public AudioSource SFXSource
    {
        get { return m_audioSource; }
        set { m_audioSource = value; }
    }
    #endregion

    #region Main
    protected virtual void Awake()
    {
        m_stateM = GetComponentInChildren<StateManager>();
        m_rb = GetComponentInChildren<Rigidbody>();
        m_anim = GetComponentInChildren<Animator>();
        m_audioSource = GetComponentInChildren<AudioSource>();

        m_currentHealth = m_maxHealth;
        m_startLocation = transform.position;
    }

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    protected virtual void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected virtual void Update()
    {
        UpdateHealthStatus();
    }

    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }
    #endregion
}
