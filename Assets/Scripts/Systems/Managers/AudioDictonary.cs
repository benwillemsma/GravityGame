using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioDictonary : MonoBehaviour
{
    public List<AudioClip> AudioClips = new List<AudioClip>();

    [SerializeField]
    private Dictionary<string, AudioClip> AudioClipDictionary = new Dictionary<string, AudioClip>();

    private AudioSource levelSource;
    public AudioSource LevelSource
    {
        get { return levelSource; }
        set { levelSource = value; }
    }

    private AudioSource playerSource;
    public AudioSource PlayerSource
    {
        get { return playerSource; }
        set { playerSource = value; }
    }

    void Start()
    {
        if (GameManager.Instance)
        {
            GameManager.AudioManager = this;

            for (int i = 0; i < AudioClips.Count; i++)
            {
                AudioClipDictionary.Add(Rename(AudioClips[i].name), AudioClips[i]);
            }
            if (GameManager.player)
                playerSource = GameManager.player.SFXSource;

            levelSource = GetComponent<AudioSource>();
            levelSource.spatialBlend = 0;
        }
    }

    public void playAudio(string name)
    {
        levelSource.PlayOneShot(AudioClipDictionary[name]);
    }
    public void playAudio(CharacterData character, string name)
    {
        character.SFXSource.PlayOneShot(AudioClipDictionary[name]);
    }

    public void playAudio(AudioSource ap, string name)
    {
        if(ap) ap.PlayOneShot(AudioClipDictionary[name]);
    }
    public void playAudio(AudioSource ap, AudioClip clip)
    {
        if (ap) ap.PlayOneShot(clip);
    }

    public void playAudioPlayerSFX(string name)
    {
        playAudio(playerSource, name);
    }
    public void playAudioPlayerSFX(AudioClip clip)
    {
        playAudio(playerSource, clip);
    }

    public void AudioChance(AudioSource ap, AudioClip ac, float chance)
    {
        if (chance < Random.Range(0.0f, 100.0f))
            playAudio(ap, ac);
    }

    private IEnumerator BlendIntoSource(AudioSource a, AudioSource b, float blendTime)
    {
        while (a.volume > 0)
        {
            a.volume -= Time.deltaTime / 2;
            b.volume += Time.deltaTime / 2;
            yield return null;
        }
        a.Stop();
    }

    //function for remaning the audio file name for the dictionary
    private string Rename(string name)
    {
        string newName = "";

        string[] temp = name.Split('_');

        for (int i = 0; i < temp.Length; i++)
        {
            newName += temp[i].ToLower();
        }

        return newName;
    }
}
