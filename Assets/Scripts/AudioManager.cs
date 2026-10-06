using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip hitSound;
    public AudioClip deathSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip , float volume =  1f)
    {
        StartCoroutine(PlaySFXCoroutine(clip, volume));
    }
   
   IEnumerator PlaySFXCoroutine(AudioClip clip, float volume =1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();


        yield return new WaitForSeconds(clip.length * 2f);
        Destroy(audioSource);
        
    }
    public void PlayHitSound()
    {
        audioSource.PlayOneShot(hitSound);
    }

    public void PlayDeathSound()
    {
        audioSource.PlayOneShot(deathSound);
    }
}