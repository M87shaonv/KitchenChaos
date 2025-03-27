using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public AudioClipSO audioClipSO;
    public AudioSource CutAudioSource;
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindObjectOfType<AudioManager>();
            if (instance != null) return instance;
            GameObject obj = new GameObject("AudioManager");
            instance = obj.AddComponent<AudioManager>();
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    public AudioClipSO GetAudioClip() => audioClipSO;
    public void PlaySound(AudioClip[] clips, float volume = 1f) =>
        AudioSource.PlayClipAtPoint(clips[Random.Range(0, clips.Length)], Camera.main.transform.position, volume);

    public void PlaySoundLoop(AudioClip[] clips, AudioSource source, float volume = 1f)
    {
        source.clip = clips[Random.Range(0, clips.Length)];
        source.volume = volume;
        source.loop = true;
        source.transform.position = Camera.main.transform.position;
        source.Play();
    }
    public void StopSound(AudioSource source) => source.Stop();
    public void StopAllSounds()
    {
        CutAudioSource.Stop();
    }
}